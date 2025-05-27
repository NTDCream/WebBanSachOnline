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
    public class CartItemsController : Controller
    {
        private Model1 db = new Model1();

        // GET: CartItems
        public ActionResult Index()
        {
            var userId = /*User.Identity.GetUserId()*/ 1;
            var cartItems = db.CartItems
            .Include(c => c.Book.Category)
            .Where(c => c.userId == userId)
            .ToList();

            return View(cartItems);
        }

        public ActionResult ThanhToan()
        {
            return View();
        }

        [HttpGet]
        //public ActionResult AddToCart(int bookId)
        //{
        //    int userId = 1;/*GetCurrentUserId()*/; // Nếu có hệ thống đăng nhập

        //    var cartItem = db.CartItems.FirstOrDefault(x => x.userId == userId && x.bookId == bookId);
        //    if (cartItem != null)
        //    {
        //        cartItem.quantity += 1;
        //    }
        //    else
        //    {
        //        db.CartItems.Add(new CartItem
        //        {
        //            userId = userId,
        //            bookId = bookId,
        //            quantity = 1
        //        });
        //    }

        //    db.SaveChanges();

        //    return Redirect(Request.UrlReferrer?.ToString() ?? "/");
        //}

        public JsonResult AddToCart(int bookId)
        {
            int userId = 1; // GetCurrentUserId();

            var cartItem = db.CartItems.FirstOrDefault(x => x.userId == userId && x.bookId == bookId);
            if (cartItem != null)
            {
                cartItem.quantity += 1;
            }
            else
            {
                db.CartItems.Add(new CartItem
                {
                    userId = userId,
                    bookId = bookId,
                    quantity = 1
                });
            }

            db.SaveChanges();
            //return Json(new { success = true }, JsonRequestBehavior.AllowGet);

            return Json(new { success = true });
        }

        //private int GetCurrentUserId()
        //{
        //    // Nếu dùng Identity
        //    return int.Parse(User.Identity.GetUserId());
        //}

        // GET: CartItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartItem cartItem = db.CartItems.Find(id);
            if (cartItem == null)
            {
                return HttpNotFound();
            }
            return View(cartItem);
        }

        // GET: CartItems/Create
        public ActionResult Create()
        {
            ViewBag.bookId = new SelectList(db.Books, "id", "title");
            ViewBag.userId = new SelectList(db.Users, "id", "fullName");
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,userId,bookId,quantity")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                db.CartItems.Add(cartItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.bookId = new SelectList(db.Books, "id", "title", cartItem.bookId);
            ViewBag.userId = new SelectList(db.Users, "id", "fullName", cartItem.userId);
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartItem cartItem = db.CartItems.Find(id);
            if (cartItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.bookId = new SelectList(db.Books, "id", "title", cartItem.bookId);
            ViewBag.userId = new SelectList(db.Users, "id", "fullName", cartItem.userId);
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,userId,bookId,quantity")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cartItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.bookId = new SelectList(db.Books, "id", "title", cartItem.bookId);
            ViewBag.userId = new SelectList(db.Users, "id", "fullName", cartItem.userId);
            return View(cartItem);
        }

        // GET: CartItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartItem cartItem = db.CartItems.Find(id);
            if (cartItem == null)
            {
                return HttpNotFound();
            }
            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return HttpNotFound();

            var cartItem = db.CartItems.FirstOrDefault(ci => ci.Book.slug == slug);
            if (cartItem == null)
                return HttpNotFound();

            db.CartItems.Remove(cartItem);
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
