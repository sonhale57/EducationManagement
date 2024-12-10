using SuperbrainManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperbrainManagement.Controllers.Helper
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreatePayment(decimal amount, string orderId)
        {
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string vnp_TmnCode = "U8XMJKHW";
            string vnp_HashSecret = "G1ZYXAMTPQPWC11ZEOGPEXRGLPN7ZGX5";
            //string vnp_Returnurl = "http://45.119.82.38:2808/Payment/VnPayReturn"; // URL trả về
            string vnp_Returnurl = "https://localhost:44338/Payment/VnPayReturn"; // URL trả về

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)(amount * 100)).ToString());  // amount phải nhân với 100
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_TxnRef", orderId);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanhtoandonhang" + orderId);
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Redirect(paymentUrl);
        }
        public ActionResult VnPayReturn()
        {
            var vnpay = new VnPayLibrary();
            string vnp_HashSecret = "G1ZYXAMTPQPWC11ZEOGPEXRGLPN7ZGX5";

            foreach (string key in Request.QueryString)
            {
                vnpay.AddResponseData(key, Request.QueryString[key]);
            }

            string vnpSecureHash = Request.QueryString["vnp_SecureHash"];
            bool checkSignature = vnpay.ValidateSignature(vnpSecureHash, vnp_HashSecret);

            if (checkSignature)
            {
                string responseCode = vnpay.GetResponseData("vnp_ResponseCode");
                if (responseCode == "00")
                {
                    ViewBag.Message = "Thanh toán thành công!";
                }
                else
                {
                    ViewBag.Message = $"Thanh toán không thành công! Mã lỗi: {responseCode}";
                }
            }
            else
            {
                ViewBag.Message = "Chữ ký không hợp lệ!";
            }

            return View();
        }
        [HttpPost]
        public ActionResult VnPayIPN()
        {
            var vnpay = new VnPayLibrary();
            string vnp_HashSecret = "G1ZYXAMTPQPWC11ZEOGPEXRGLPN7ZGX5";

            foreach (string key in Request.QueryString)
            {
                vnpay.AddResponseData(key, Request.QueryString[key]);
            }

            string vnpSecureHash = Request.QueryString["vnp_SecureHash"];
            bool checkSignature = vnpay.ValidateSignature(vnpSecureHash, vnp_HashSecret);

            if (checkSignature)
            {
                string responseCode = vnpay.GetResponseData("vnp_ResponseCode");
                if (responseCode == "00")
                {
                    // Cập nhật trạng thái giao dịch trong database
                    return Json(new { RspCode = "00", Message = "Giao dịch thành công" });
                }
                else
                {
                    return Json(new { RspCode = "01", Message = "Giao dịch không thành công" });
                }
            }
            else
            {
                return Json(new { RspCode = "97", Message = "Chữ ký không hợp lệ" });
            }
        }

    }
}