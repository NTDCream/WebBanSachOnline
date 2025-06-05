using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebBanSachOnline.Models;

namespace WebBanSachOnline.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private Model1 db = new Model1();

        // GET: Admin/Home
        public ActionResult Index()
        {
            if(Session["role"] == null)
            {
                return Redirect("/Admin");
            }
            var totalUsers = db.Users.Count();
            var totalOrders = db.Orders.Count();
            var totalBooks = db.Books.Count();
            var totalRevenue = db.Orders.Sum(o => (decimal?)o.totalAmount) ?? 0;

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalBooks = totalBooks;
            ViewBag.TotalRevenue = totalRevenue;

            var recentOrders = db.Orders
                     .OrderByDescending(o => o.createdDate)
                     .Take(10)
                     .ToList();
            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View(db.Users.ToList());
        }

        public ActionResult Logout()
        {
            Session.Remove("user");
            Session.Remove("userId");
            Session.Remove("role");
            return Redirect("/Admin");
        }

        public ActionResult Statistic()
        {
            if (Session["role"] == null)
            {
                return Redirect("/Admin");
            }

            var revenueData = db.Orders
            .Where(o => o.status.ToLower() == "paid")
            .GroupBy(o => DbFunctions.TruncateTime(o.createdDate))
            .Select(g => new
            {
                Date = g.Key,
                Revenue = g.Sum(o => o.totalAmount)
            })
            .OrderBy(x => x.Date)
            .ToList();

            ViewBag.RevenueData = revenueData;

            var last7Days = revenueData.Skip(Math.Max(0, revenueData.Count - 7)).ToList();
            ViewBag.Last7Days = last7Days;
            return View();
        }


        [HttpPost]
        public ActionResult LogIn(string username, string password)
        {
            var a = db.Users.FirstOrDefault(x => x.username == username && x.password == password);
            if (a != null)
            {
                if (a.isActive == true)
                {
                    Session["userId"] = a.id;
                    Session["user"] = a.username;
                    if (a.role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    {
                        Session["role"] = a.role;
                        return Redirect("Index");
                    }
                    else
                    {
                        return Redirect("/");
                    }
                }
                ViewBag.error = "Tài khoản đã bị khóa";
            }
            else
            {
                ViewBag.error = "Sai tên đăng nhập hoặc mật khẩu";
            }
            return Redirect("/Admin");
        }
    }
}
