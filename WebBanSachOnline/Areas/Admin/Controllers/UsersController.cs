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

namespace WebBanSachOnline.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private Model1 db = new Model1();

        // GET: Admin/Users
        public ActionResult Index(int ?page)
        {
            if (Session["role"] == null)
            {
                return Redirect("/Admin");
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var users = db.Users.OrderBy(u => u.id).ToPagedList(pageNumber, pageSize);
            return View(users);
        }

        // GET: Admin/Users/Details/5
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
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Admin/Users/Create
        public ActionResult Create()
        {
            if (Session["role"] == null)
            {
                return Redirect("/Admin");
            }
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,fullName,username,email,phone,address,password,role,isActive")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Admin/Users/Edit/5
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
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,fullName,username,email,phone,address,password,role,isActive")] User user)
        {
            if (ModelState.IsValid)
            {
                var userInDb = db.Users.Find(user.id);
                if (userInDb == null) return HttpNotFound();

                // Chỉ cập nhật những gì được phép
                userInDb.role = user.role;
                userInDb.isActive = user.isActive;

                db.SaveChanges();
                return RedirectToAction("Index");
                //db.Entry(user).State = EntityState.Modified;
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
