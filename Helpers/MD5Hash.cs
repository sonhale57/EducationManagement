using System.Security.Cryptography;
using System.Text;
using System;

namespace SuperbrainManagement.Controllers
{
    public class MD5Hash
    {
        public MD5Hash()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public string GetMD5Working(string input)
        {
            MD5 md5 = MD5.Create();

            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < hash.Length; j++)
            {
                sb.Append(hash[j].ToString("X2"));
            }

            return sb.ToString().ToLower();
        }
        public string mahoamd5(string txt)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(Encoding.ASCII.GetBytes(txt));
            String strpass = BitConverter.ToString(result);
            return strpass;
        }
        string key = "sieulienket.vn";
        public string Encrypt(string toEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            TripleDESCryptoServiceProvider tdes =
            new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
            toEncryptArray, 0, toEncryptArray.Length);
            string ressult = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            ressult = ressult.Replace("&", "xxxand");
            ressult = ressult.Replace("?", "xxxask");
            ressult = ressult.Replace("+", "xxxadd");
            ressult = ressult.Replace("=", "xxxlike");
            ressult = ressult.Replace("/", "xxxet");
            return ressult;
        }

        public string Decrypt(string toDecrypt)
        {
            toDecrypt = toDecrypt.Replace("xxxet", "/");
            toDecrypt = toDecrypt.Replace("xxxand", "&");
            toDecrypt = toDecrypt.Replace("xxxask", "?");
            toDecrypt = toDecrypt.Replace("xxxadd", "+");
            toDecrypt = toDecrypt.Replace("xxxlike", "=");

            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
            toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public string javascripthash(string textpass)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(textpass);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public string GeneratePassword(string password)
        {
            // Băm mật khẩu ban đầu thành chuỗi MD5
            string hashedPassword = GetMD5Working(password);

            // Loại bỏ các ký tự không mong muốn từ chuỗi MD5
            string processedPassword = hashedPassword.Replace("&^%$", "");

            return processedPassword;
        }
    }
}