using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;
using WebBanSachOnline.Models;

namespace WebBanSachOnline.Controllers
{
    public class CategoriesController : Controller
    {
        private Model1 db = new Model1();

        // GET: Categories
        public ActionResult Index(string title, int ?page)
        {
            var categories = db.Categories.ToList();

            var books = db.Books.ToList();

            if (!String.IsNullOrEmpty(title))
            {
                books = books.Where(x => x.title.ToLower().Contains(title.ToLower())).ToList();
            }

            ViewBag.Categories = categories;
            ViewBag.Books = books;
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            ViewBag.Categories = categories;
            ViewBag.Books = books.ToPagedList(pageNumber, pageSize);
            return View();
        }

        public ActionResult Menu2()
        {
            return PartialView(db.Categories.ToList());
        }

        [Route("Categories/{slug}")]
        public ActionResult BySlug(string slug, string title, int ?page)
        {
            var category = db.Categories.FirstOrDefault(c => c.slug == slug);
            if (category == null)
            {
                return HttpNotFound();
            }

            var books = db.Books.Where(x => x.categoryId == category.id).ToList();
            if (!String.IsNullOrEmpty(title))
            {
                books = books.Where(x => x.title.ToLower().Contains(title.ToLower())).ToList();
            }
            int pageSize = 9; 
            int pageNumber = page ?? 1;

            var pagedBooks = books.ToPagedList(pageNumber, pageSize);

            ViewBag.CategoryId = category.id;
            ViewBag.Slug = slug;
            ViewBag.TitleSearch = title;

            return View("ById", pagedBooks);
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,title,slug")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,slug")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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
