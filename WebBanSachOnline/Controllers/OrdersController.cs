using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanSachOnline.Models;

namespace WebBanSachOnline.Controllers
{
    public class OrdersController : Controller
    {
        private Model1 db = new Model1();

        // GET: Orders
        public ActionResult Index()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("SignIn", "Users");
            }
            var orders = db.Orders.Include(o => o.User);
            return View(orders.ToList());
        }

        
        // GET: Orders/Details/5
        public ActionResult Details(string slug)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("SignIn", "Users");
            }
            if (string.IsNullOrEmpty(slug))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = db.Orders
                        .Include(o => o.OrderDetails.Select(od => od.Book.Category))
                        .FirstOrDefault(o => o.slug == slug);
            if (order == null)
                return HttpNotFound();
            

            

            return View(order);
        }

        // GET: Orders/Create
        //public ActionResult Create()
        //{
        //    ViewBag.userId = new SelectList(db.Users, "id", "fullName");
        //    return View();
        //}

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,userId,slug,status,totalAmount,customerName,phone,address,price,paymentMethod,createdDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.Users, "id", "fullName", order.userId);
            return View(order);
        }

        // GET: Orders/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Order order = db.Orders.Find(id);
        //    if (order == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.userId = new SelectList(db.Users, "id", "fullName", order.userId);
        //    return View(order);
        //}

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,userId,slug,status,totalAmount,customerName,phone,address,price,paymentMethod,createdDate")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(order).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.userId = new SelectList(db.Users, "id", "fullName", order.userId);
        //    return View(order);
        //}

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
