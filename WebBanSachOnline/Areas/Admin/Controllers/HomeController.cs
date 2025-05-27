using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSachOnline.Models;

namespace WebBanSachOnline.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private Model1 db = new Model1();
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View(db.Users.ToList());
        }

        //public ActionResult Logout()
        //{
        //    Session.Remove("user");
        //    return Redirect("/Admin");
        //}

        public ActionResult Statistic()
        {
            return View();
        }


        [HttpPost]
        public ActionResult LogIn(string username, string password)
        {
            var a = db.Users.FirstOrDefault(x => x.username == username && x.password == password);
            if (a != null && a.role == "Admin")
            {
                Session["userId"] = a.id;
                Session["user"] = username;
                Session["role"] = a.role;

                return Redirect("/Admin/Home/Index");
            }
            else
            {
                ViewBag.error = "Sai tên đăng nhập hoặc mật khẩu";
                return View("LogIn");
            }
        }
    }
}