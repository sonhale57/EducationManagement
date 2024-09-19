using SuperbrainManagement.Helpers;
using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SuperbrainManagement.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ModelDbContext db = new ModelDbContext();
        private readonly VNPayHelper _vnPayHelper = new VNPayHelper();

        public ActionResult TestPage()
        {
            return View();
        }
        public ActionResult Payment(int orderId, decimal amount)
        {
            string vnp_TmnCode = ConfigurationManager.AppSettings["VNPAY_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["VNPAY_HashSecret"];
            string vnp_Url = ConfigurationManager.AppSettings["VNPAY_Url"];
            string vnp_ReturnUrl = ConfigurationManager.AppSettings["VNPAY_ReturnUrl"];

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)amount * 100).ToString()); // Số tiền tính bằng đơn vị nhỏ nhất
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng #" + orderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", orderId.ToString());

            // Tạo URL yêu cầu thanh toán và chuyển hướng
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentReturn()
        {
            // Lấy dữ liệu từ query string của VNPAY trả về
            string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (string key in Request.QueryString)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, Request.QueryString[key]);
                }
            }

            string vnp_HashSecret = ConfigurationManager.AppSettings["VNPAY_HashSecret"];
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

            if (checkSignature)
            {
                // Xử lý khi chữ ký hợp lệ
                string vnp_ResponseCode = Request.QueryString["vnp_ResponseCode"];
                if (vnp_ResponseCode == "00")
                {
                    // Thanh toán thành công
                    ViewBag.Message = "Giao dịch thành công!";
                }
                else
                {
                    // Thanh toán thất bại
                    ViewBag.Message = "Giao dịch không thành công!";
                }
            }
            else
            {
                // Chữ ký không hợp lệ
                ViewBag.Message = "Chữ ký không hợp lệ!";
            }

            return View();
        }


    }
}