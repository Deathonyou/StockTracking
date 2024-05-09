using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace StockTracking
{
    public static class Helper
    {
        public static string Encoder(string Text)
        {
            MD5CryptoServiceProvider mD5Crypto = new MD5CryptoServiceProvider();
            byte[] byteType = Encoding.UTF8.GetBytes(Text);
            byteType = mD5Crypto.ComputeHash(byteType);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte byt in byteType)
                stringBuilder.Append(byt.ToString("x2").ToLower());
            return stringBuilder.ToString();
        }
    }
}