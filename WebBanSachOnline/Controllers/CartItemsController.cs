using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using WebBanSachOnline.Models.Payments;
using WebBanSachOnline.Models;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

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
        public ActionResult Checkout()
        {
            int userId = 1; /*(int)Session["userId"];*/

            var cartItems = db.CartItems
                      .Include(ci => ci.Book)
                      .Include(ci => ci.User)
                      .Where(ci => ci.userId == userId)
                      .ToList();

            return View(cartItems);
        }

        public ActionResult DeleteAllItems()
        {
            int userId = 1;
            var cartItems = db.CartItems
                              .Include(ci => ci.Book)
                              .Where(ci => ci.userId == userId)
                              .ToList();
            db.CartItems.RemoveRange(cartItems);
            db.SaveChanges();
            return RedirectToAction("Index", "CartItems");
        }

        public ActionResult ThanhToan(string fullName, string phone, string address, string paymentMethod)
        {
            string slug;
            do
            {
                slug = "DH" + Guid.NewGuid().ToString("N").Substring(0, 6);
            }
            while (db.Orders.Any(o => o.slug == slug));

            int userId = 1; // giả lập
            var cartItems = db.CartItems
                              .Include(ci => ci.Book)
                              .Where(ci => ci.userId == userId)
                              .ToList();



            int total = cartItems.Sum(ci => ci.Book.price * ci.quantity);
            string status = "";
            status = "pending";

            var order = new Order
            {
                userId = userId,
                slug = slug,
                status = status,
                totalAmount = total,
                customerName = fullName,
                phone = phone,
                address = address,
                paymentMethod = paymentMethod,
                createdDate = DateTime.Now
            };
            db.Orders.Add(order);
            db.SaveChanges();


            //db.Entry(order).Property(o => o.slug).IsModified = true;
            //db.SaveChanges();

            // Tạo chi tiết đơn hàng
            foreach (var item in cartItems)
            {
                var detail = new OrderDetail
                {
                    orderId = order.id,
                    bookId = item.bookId,
                    quantity = item.quantity,
                    price = item.Book.price
                };
                db.OrderDetails.Add(detail);

                var book = db.Books.FirstOrDefault(b => b.id == item.bookId);
                if (book != null)
                {
                    // Cập nhật số lượng đã bán
                    book.soldQuantity += item.quantity;
                    if (book.quantity < 0)
                    {
                        book.quantity = 0;
                    }
                }
            }
            //Xóa sách trong giỏ hàng
            db.CartItems.RemoveRange(cartItems);
            db.SaveChanges();

            if (paymentMethod == "BANK")
            {

                return RedirectToAction("CreatePayment", new { slug = slug, total = total });
            }

            return RedirectToAction("OrderSuccess", new { slug = order.slug });
        }


        //VNPay
        public ActionResult CreatePayment(string slug, int total)
        {


            string returnUrl = VnPayConfig.vnp_Returnurl + "?slug=" + slug;

            var vnPay = new VnPayLibrary();

            vnPay.AddRequestData("vnp_Version", "2.1.0");
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_TmnCode", VnPayConfig.vnp_TmnCode);
            vnPay.AddRequestData("vnp_Amount", (total * 100).ToString()); // số tiền * 100
            vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang");
            vnPay.AddRequestData("vnp_OrderType", "other");
            vnPay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnPay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            string paymentUrl = vnPay.CreateRequestUrl(VnPayConfig.vnp_Url, VnPayConfig.vnp_HashSecret);

            return Redirect(paymentUrl);
        }

        //public ActionResult CreatePayment(string slug)
        //{

        //    var vnPay = new VnPayLibrary();


        //    vnPay.AddRequestData("vnp_Version", "2.1.0");
        //    vnPay.AddRequestData("vnp_Command", "pay");
        //    vnPay.AddRequestData("vnp_TmnCode", VnPayConfig.vnp_TmnCode);
        //    vnPay.AddRequestData("vnp_Amount", (100000 * 100).ToString()); // số tiền * 100
        //    vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        //    vnPay.AddRequestData("vnp_CurrCode", "VND");
        //    vnPay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);
        //    vnPay.AddRequestData("vnp_Locale", "vn");
        //    vnPay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang");
        //    vnPay.AddRequestData("vnp_OrderType", "other");
        //    vnPay.AddRequestData("vnp_ReturnUrl", VnPayConfig.vnp_Returnurl);
        //    vnPay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

        //    string paymentUrl = vnPay.CreateRequestUrl(VnPayConfig.vnp_Url, VnPayConfig.vnp_HashSecret);

        //    return Redirect(paymentUrl);
        //}

        public ActionResult VNPayReturn()
        {

            var vnp_ResponseCode = Request.QueryString["vnp_ResponseCode"];
            var slug = Request.QueryString["slug"];

            if (vnp_ResponseCode == "00")
            {
                var order = db.Orders.FirstOrDefault(o => o.slug == slug);
                order.status = "paid";
                db.SaveChanges();
                return RedirectToAction("OrderSuccess", new { slug = slug });
            }
            return RedirectToAction("OrderFail");
        }

        public ActionResult OrderSuccess(string slug)
        {
            ViewBag.OrderSlug = slug;
            return View();

        }

        public ActionResult OrderFail()
        {
            return View();
        }

        //public ActionResult Return()
        //{
        //    var vnp_ResponseCode = Request.QueryString["vnp_ResponseCode"];
        //    if (vnp_ResponseCode == "00")
        //    {
        //        ViewBag.Message = "Giao dịch thành công!";
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Giao dịch thất bại!";
        //    }
        //    return View();
        //}

        [HttpGet]
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

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
