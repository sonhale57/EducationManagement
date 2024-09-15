using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
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
            string signData = ComputeHash(vnp_HashSecret, queryString);
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

        private string GetResponseData()
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in responseData)
            {
                if (data.Length > 0)
                {
                    data.Append("&");
                }
                data.Append(kv.Key + "=" + kv.Value);
            }
            return data.ToString();
        }
        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                responseData.Add(key, value);
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