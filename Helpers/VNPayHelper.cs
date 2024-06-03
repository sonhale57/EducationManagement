using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SuperbrainManagement.Helpers
{
    public class VNPayHelper
    {
        private readonly string vnp_TmnCode = "TC9BD52B";
        private readonly string vnp_HashSecret = "3U9X5YKOLEBTOHVMWT4ZHH52E66NZO6U";
        private readonly string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private readonly string vnp_ReturnUrl = "https://localhost:44338/Payment/VnPayReturn";

        public string CreatePaymentUrl(decimal amount, string orderId, string orderInfo)
        {
            var vnPayData = new SortedList<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", ((int)(amount * 100)).ToString() },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", GetIpAddress() },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", vnp_ReturnUrl },
                { "vnp_TxnRef", orderId }
            };

            var queryString = vnPayData.Select(kv => HttpUtility.UrlEncode(kv.Key) + "=" + HttpUtility.UrlEncode(kv.Value)).ToArray();
            var signData = string.Join("&", queryString);

            var hash = HmacSHA512(vnp_HashSecret, signData);
            signData += "&vnp_SecureHash=" + hash;

            return vnp_Url + "?" + signData;
        }

        private static string GetIpAddress()
        {
            return HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "127.0.0.1";
        }

        public static string HmacSHA512(string key, string input)
        {
            using (var hmacsha512 = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(input));
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
        }
    }
}