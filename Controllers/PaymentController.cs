using SuperbrainManagement.Helpers;
using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperbrainManagement.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ModelDbContext db = new ModelDbContext();
        private readonly VNPayHelper _vnPayHelper = new VNPayHelper();

        // Hiển thị trang thanh toán
        public ActionResult Payment(int orderId)
        {
            var o = db.Orders.Find(orderId);
            var od = db.OrderDetails.Where(x => x.IdOrder == orderId);
            decimal tongtien = od.Sum(x => x.TotalAmount).Value;
            ViewBag.orderId = orderId;
            ViewBag.tongtien = tongtien;
            ViewBag.mota = "Đơn hàng " + o.Code;
            return View();
        }

        // Tạo yêu cầu thanh toán
        [HttpPost]
        public ActionResult CreatePayment(decimal amount, int orderId, string orderInfo)
        {
            var paymentUrl = _vnPayHelper.CreatePaymentUrl(amount, orderId.ToString(), orderInfo);
            return Redirect(paymentUrl);
        }

        // Nhận phản hồi từ VNPay
        public ActionResult VnPayReturn()
        {
            var vnpayData = Request.QueryString;
            string hashSecret = "3U9X5YKOLEBTOHVMWT4ZHH52E66NZO6U"; // HashSecret của bạn
            var vnp_SecureHash = vnpayData["vnp_SecureHash"];
            var data = vnpayData.AllKeys
                                .Where(k => k != "vnp_SecureHash" && k.StartsWith("vnp_"))
                                .OrderBy(k => k)
                                .Select(k => k + "=" + vnpayData[k])
                                .ToArray();
            var signData = string.Join("&", data);
            var checkSignature = VNPayHelper.HmacSHA512(hashSecret, signData);

            if (checkSignature == vnp_SecureHash)
            {
                if (vnpayData["vnp_ResponseCode"] == "00")
                {
                    // Thanh toán thành công
                    ViewBag.Message = "Thanh toán thành công!";
                    // Cập nhật trạng thái đơn hàng
                    // ... (Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu)
                }
                else
                {
                    // Thanh toán thất bại
                    ViewBag.Message = "Thanh toán thất bại!";
                }
            }
            else
            {
                // Sai chữ ký
                ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý!";
            }

            return View();
        }
    }
}