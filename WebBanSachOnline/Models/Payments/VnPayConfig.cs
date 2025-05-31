using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSachOnline.Models.Payments
{
    public class VnPayConfig
    {
        public const string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public const string vnp_Returnurl = "https://localhost:44360/CartItems/VNPayReturn"; // URL callback sau khi thanh toán
        public const string vnp_TmnCode = "2D9E1QO0"; // mã TMNCode từ VNPAY
        public const string vnp_HashSecret = "1SQJ63JNMTPVRR1JBP10BRZ12U6BOD6S"; // chuỗi bí mật
    }
}