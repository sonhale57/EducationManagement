using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SuperbrainManagement.Helpers
{
    public class VnPayLibrary
    {
        private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var data = new StringBuilder();
            foreach (var kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.AppendFormat("{0}={1}&", kv.Key, Uri.EscapeDataString(kv.Value));
                }
            }
            string rawData = data.ToString().TrimEnd('&');
            string hash = HmacSHA512(vnpHashSecret, rawData);
            return baseUrl + "?" + rawData + "&vnp_SecureHash=" + hash;
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }
        public string GetResponseData(string key)
        {
            return _responseData.ContainsKey(key) ? _responseData[key] : null;
        }
        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var rawData = new StringBuilder();
            foreach (var kv in _responseData.Where(k => k.Key != "vnp_SecureHash" && k.Key != "vnp_SecureHashType"))
            {
                rawData.AppendFormat("{0}={1}&", kv.Key, kv.Value);
            }
            string hash = HmacSHA512(secretKey, rawData.ToString().TrimEnd('&'));
            return hash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }


        private string HmacSHA512(string key, string inputData)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
                return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }
        }
    }
    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}