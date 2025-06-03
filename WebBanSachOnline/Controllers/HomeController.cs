using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSachOnline.Models;

namespace WebBanSachOnline.Controllers
{
    public class HomeController : Controller
    {
        private Model1 db = new Model1();
        public ActionResult Index()
        {
            
            //var books = db.Books.ToList();
            var latestBooks = db.Books
                                .OrderByDescending(b => b.id)
                                .Take(8)
                                .ToList();

            var topSellingBooks = db.Books
                                    .OrderByDescending(b => b.soldQuantity)
                                    .Take(8)
                                    .ToList();

            ViewBag.LatestBooks = latestBooks;
            ViewBag.TopSellingBooks = topSellingBooks;

            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}