using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SuperbrainManagement.Helpers
{
    public partial class VnPayLibrary
    {
        private SortedList<string, string> requestData = new SortedList<string, string>(new VnPayCompare());
        private SortedList<string, string> responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                responseData.Add(key, value);
            }
        }
        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in requestData)
            {
                if (data.Length > 0)
                {
                    data.Append("&");
                }
                data.Append(HttpUtility.UrlEncode(kv.Key) + "=" + HttpUtility.UrlEncode(kv.Value));
            }

            string queryString = data.ToString();
Console.WriteLine("Query String: " + queryString);  // In ra query string
            // Tạo chữ ký từ các tham số trong queryString
            string signData = ComputeHash(vnp_HashSecret, queryString);
Console.WriteLine("Signature: " + signData);  // In ra chữ ký
            return baseUrl + "?" + queryString + "&vnp_SecureHash=" + signData;
        }


        private string ComputeHash(string secretKey, string inputData)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
            byte[] inputByte = Encoding.UTF8.GetBytes(inputData);

            using (var hmac = new HMACSHA512(keyByte))
            {
                byte[] hashByte = hmac.ComputeHash(inputByte);
                return BitConverter.ToString(hashByte).Replace("-", "").ToLower();
            }
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            string rspRaw = GetResponseData();
            string myChecksum = ComputeHash(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
        
        public string GetResponseData()
        {

            StringBuilder data = new StringBuilder();
            if (responseData.ContainsKey("vnp_SecureHashType"))
            {
                responseData.Remove("vnp_SecureHashType");
            }
            if (responseData.ContainsKey("vnp_SecureHash"))
            {
                responseData.Remove("vnp_SecureHash");
            }
            foreach (KeyValuePair<string, string> kv in responseData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            // Loại bỏ ký tự '&' cuối cùng
            if (data.Length > 0)
            {
                data.Length -= 1;
            }
            return data.ToString();
        }
        
    }
    public class Utils
    {


        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
        public static string GetIpAddress()
        {
            string ipAddress = "127.0.0.1"; // Default IP nếu không lấy được

            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
                    {
                        ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                }
            }
            catch (Exception ex)
            {
                ipAddress = "Invalid IP: " + ex.Message;
            }

            return ipAddress;
        }
    }
    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return string.CompareOrdinal(x, y);
        }
    }
}