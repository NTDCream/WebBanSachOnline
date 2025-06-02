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
    public class ReviewsController : Controller
    {
        private Model1 db = new Model1();

        // GET: Reviews
        public ActionResult Index()
        {
            var reviews = db.Reviews.Include(r => r.Book).Include(r => r.User);
            return View(reviews.ToList());
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        //GET: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bookId,rate,comment")] Review review)
        {
            if (Session["userId"] == null)
                return RedirectToAction("SignIn", "Users");

            review.userId = (int)Session["userId"];
            review.createdDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();


                // Lấy slug theo bookId để redirect
                var book = db.Books.Find(review.bookId);
                var reviews = db.Reviews.Where(r => r.bookId == review.bookId).ToList();
                book.reviewCount = reviews.Count;
                book.rate = reviews.Any() ? reviews.Average(r => r.rate) : 0;

                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Books", new { slug = book.slug });
                
            }

            // Nếu ModelState có lỗi, trả lại view
            ViewBag.bookId = review.bookId;
            ViewBag.userId = review.userId;
            return View(review);
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,userId,bookId,rate,comment,createdDate")] Review review)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        db.Reviews.Add(review);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.bookId = new SelectList(db.Books, "id", "title", review.bookId);
        //    ViewBag.userId = new SelectList(db.Users, "id", "fullName", review.userId);
        //    return View(review);
        //}

        //[HttpPost]
        //public JsonResult CreateReview(Review review)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        review.createdDate = DateTime.Now;
        //        review.userId = (int)Session["userId"];
        //        db.Reviews.Add(review);
        //        db.SaveChanges();
        //        return Json(new { success = true });
        //    }
        //    return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        //}



        // GET: Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.bookId = new SelectList(db.Books, "id", "title", review.bookId);
            ViewBag.userId = new SelectList(db.Users, "id", "fullName", review.userId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,userId,bookId,rate,comment,createdDate")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.bookId = new SelectList(db.Books, "id", "title", review.bookId);
            ViewBag.userId = new SelectList(db.Users, "id", "fullName", review.userId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
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
