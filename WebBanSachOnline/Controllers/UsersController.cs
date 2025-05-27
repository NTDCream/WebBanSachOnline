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
    public class UsersController : Controller
    {
        private Model1 db = new Model1();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        [HttpGet]
        public ActionResult Signin()
        {
            return View(db.Users.ToList());
        }



        [HttpPost]
        public ActionResult SignIn(string username, string password)
        {
            var a = db.Users.FirstOrDefault(x => x.username == username && x.password == password);
            if (a != null)
            {
                Session["userId"] = a.id;
                Session["user"] = username;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.error = "Sai tên đăng nhập hoặc mật khẩu";
                return View("SignIn");
            }
        }

        public ActionResult Signout()
        {
            Session.Remove("user");
            return RedirectToAction("Index", "Home");
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
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

        public ActionResult Infor()
        {
            int userId = (int)Session["userId"];
            var user = db.Users.FirstOrDefault(u => u.id == userId);

            if (user == null)
            {
                return HttpNotFound(); // hoặc xử lý khác nếu không tìm thấy user
            }
            return View(user);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Infor([Bind(Include = "id,fullName,phone,email,address")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Lấy user gốc từ db theo id (hoặc username từ session nếu có)
        //        var existingUser = db.Users.Find(user.id);
        //        if (existingUser == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        // Cập nhật các trường cần thiết
        //        existingUser.fullName = user.fullName;
        //        existingUser.phone = user.phone;
        //        existingUser.email = user.email;
        //        existingUser.address = user.address;

        //        db.SaveChanges();

        //        TempData["Success"] = "Cập nhật thông tin thành công.";
        //        return RedirectToAction("Infor");
        //    }

        //    return View(user);
        //}

        // GET: Users/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Users/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,fullName,username,email,phone,address,password,role,isActive")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Users.Add(user);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(user);
        //}

        // GET: Users/SignUp
        public ActionResult SignUp()
        {
            return View();
        }

        // POST: Users/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp([Bind(Include = "id,fullName,username,email,phone,address,password")] User user, string confirmPassword)
        {
            // Kiểm tra mật khẩu xác nhận
            if (user.password != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                // Thiết lập mặc định các trường không nhập từ form
                user.role = "Customer";
                user.isActive = true;

                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("SignIn");
            }

            return View(user);
        }


        public ActionResult ForgetPassword()
        {
            return View();
        }

        //GET: Users/Edit/5
        public ActionResult Edit(string slug)
        {
            // Tìm user theo username (slug)
            User user = db.Users.FirstOrDefault(u => u.username == slug);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id, username,fullName,email,phone,address")] User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.username == user.username);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                existingUser.fullName = user.fullName;
                existingUser.phone = user.phone;
                existingUser.email = user.email;
                existingUser.address = user.address;

                db.Entry(existingUser).State = EntityState.Modified; // thêm dòng này
                db.SaveChanges();

                return RedirectToAction("Infor");
            }

            return View(user);
        }

        // GET: Users/Delete/5
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

        // POST: Users/Delete/5
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
