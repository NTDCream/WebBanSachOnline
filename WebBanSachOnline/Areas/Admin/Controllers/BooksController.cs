using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WebBanSachOnline.Models;

namespace WebBanSachOnline.Areas.Admin.Controllers
{
    public class BooksController : Controller
    {

        private Model1 db = new Model1();

        // GET: Admin/Books
        public ActionResult Index(int ?page)
        {
            if (Session["role"] == null)
            {
                return Redirect("/Admin");
            }

            int pageSize = 10; 
            int pageNumber = (page ?? 1);
            var books = db.Books
                   .Include(b => b.Category)
                   .OrderBy(b => b.id)
                   .ToPagedList(pageNumber, pageSize);
            //var books = db.Books
            //.Include(b => b.Category) // Eager loading Category
            //.ToList(); // Thực hiện truy vấn

            return View(books);
        }

        // GET: Admin/Books/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["role"] == null)
            {
                return Redirect("/Admin");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Admin/Books/Create
        public ActionResult Create()
        {
            if (Session["role"] == null)
            {
                return Redirect("/Admin");
            }
            ViewBag.categoryId = new SelectList(db.Categories, "id", "title");
            return View();
        }

        public string GenerateSlug(string title)
        {
            string str = title.ToLower().Trim();
            str = RemoveVietnameseSigns(str);
            str = Regex.Replace(str, @"\s+", "-");
            str = Regex.Replace(str, @"[^a-z0-9\-]", "");
            return str;
        }

        public string RemoveVietnameseSigns(string str)
        {
            string[] vietnameseSigns = new string[]
            {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
            };

            for (int i = 1; i < vietnameseSigns.Length; i++)
            {
                for (int j = 0; j < vietnameseSigns[i].Length; j++)
                {
                    str = str.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
                }
            }
            return str;
        }

        // POST: Admin/Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,title,categoryId,author,description,image,price,quantity,soldQuantity,originalPrice")] Book book)
        {

            book.slug = GenerateSlug(book.title);
            book.rate = 0;
            book.reviewCount = 0;
            ModelState.Remove("slug");
            

            if (ModelState.IsValid)
            {
                var f = Request.Files["image"];
                if (f.ContentLength > 0)
                {
                    string tenfile = Path.GetFileName(f.FileName);
                    string duongdan = Path.Combine(Server.MapPath("~/BookImages/"), tenfile);
                    book.image = tenfile;
                }
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Admin/Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["role"] == null)
            {
                return Redirect("/Admin");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.categoryId = new SelectList(db.Categories, "id", "title", book.categoryId);
            return View(book);
        }

        // POST: Admin/Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,categoryId,author,description,image,price,quantity,soldQuantity,originalPrice")] Book book)
        {

            ModelState.Remove("slug");
            if (ModelState.IsValid)
            {
                var f = Request.Files["image"];
                if (f.ContentLength >= 0)
                {
                    string tenfile = Path.GetFileName(f.FileName);
                    string duongdan = Path.Combine(Server.MapPath("~/BookImages/"), tenfile);
                    book.image = tenfile;
                }
                var existingBook = db.Books.Find(book.id);
                if (existingBook == null)
                    return HttpNotFound();

                existingBook.title = book.title;
                existingBook.categoryId = book.categoryId;
                existingBook.description = book.description;
                existingBook.image = book.image;
                existingBook.price = book.price;
                existingBook.quantity = book.quantity;
                existingBook.soldQuantity = book.soldQuantity;
                existingBook.originalPrice = book.originalPrice;
                existingBook.rate = book.rate;
                existingBook.reviewCount = book.reviewCount;
                existingBook.slug = GenerateSlug(book.title);
                existingBook.author = book.author;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.categoryId = new SelectList(db.Categories, "id", "title", book.categoryId);
            return View(book);
        }

        // GET: Admin/Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Admin/Books/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var book = db.Books.FirstOrDefault(ci => ci.id == id);
            if (book == null)
                return HttpNotFound();

            db.Books.Remove(book);
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
