using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebBanSachOnline.Models;
using WebBanSachOnline.Models.Tokens;

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
                if (a.isActive == true)
                {
                    Session["userId"] = a.id;
                    Session["user"] = a.username;
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.error = "Tài khoản đã bị khóa";
            }
            else
            {
                ViewBag.error = "Sai tên đăng nhập hoặc mật khẩu";

            }
            return View("SignIn");
        }

        public ActionResult Signout()
        {
            Session.Remove("user");
            Session.Remove("userId");
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
                return HttpNotFound();
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

        [HttpGet]
        public ActionResult ConfirmReset()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmReset(string email)
        {
            var user = db.Users.FirstOrDefault(u => u.email == email);
            if (user == null)
            {
                ViewBag.Message = "Email không tồn tại.";
                return View();
            }
            ViewBag.email = email;
            string code = GenerateUniqueCode();
            var token = new Token(email, code);
            Session["Reset_" + code] = token;

            string resetLink = Url.Action("ResetPassword", "Users", new { slug = code }, protocol: Request.Url.Scheme);
            string subject = "Đặt lại mật khẩu";
            string body = $@"
                        <h2>Quên mật khẩu</h2>
                        <p><strong>Nhà Sách Online</strong><br /></p>
                        <p>Truy cập <a href='{resetLink}'>liên kết này</a> để đặt lại mật khẩu</p>
                    ";
            SendEmail(email, subject, body);
            return View();
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.Send(mail);
            }
        }

        private string GenerateUniqueCode()
        {
            string code;
            do
            {
                code = new Random().Next(100000, 999999).ToString();
            } while (Session["Reset_" + code] != null);
            return code;
        }

        [HttpGet]
        public ActionResult ResetPassword(string slug)
        {
            var sessionKey = "Reset_" + slug;
            var tokenObj = Session[sessionKey] as Token;

            if (tokenObj == null)
            {
                ViewBag.Message = "Link không hợp lệ.";
                return View();
            }

            // Truyền slug để dùng cho form post
            ViewBag.Token = slug;
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string password, string confirmPassword, string slug)
        {
            var sessionKey = "Reset_" + slug;
            var tokenObj = Session[sessionKey] as Token;

            if (tokenObj == null)
            {
                ViewBag.Message = "Link đặt lại mật khẩu không hợp lệ.";
                ViewBag.Token = slug;
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Message = "Mật khẩu nhập lại không khớp.";
                ViewBag.Token = slug;
                return View();
            }

            var user = db.Users.FirstOrDefault(u => u.email == tokenObj.Email);
            if (user != null)
            {
                user.password = password; // Nên hash trước khi lưu
                db.SaveChanges();
            }

            Session.Remove(sessionKey);

            ViewBag.Message = "Đổi mật khẩu thành công. Bạn có thể đăng nhập lại.";
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
        public ActionResult Edit(string fullName, string email, string phone, string address)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("SignIn", "Users");
            }
            var userId = (int)Session["userId"];
            var existingUser = db.Users.Find(userId);
            if (existingUser == null)
            {
                return HttpNotFound();
            }

            // Cập nhật các trường
            existingUser.fullName = fullName;
            existingUser.email = email;
            existingUser.phone = phone;
            existingUser.address = address;

            db.SaveChanges();

            return RedirectToAction("Infor");
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
