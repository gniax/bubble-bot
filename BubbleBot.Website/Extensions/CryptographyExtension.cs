using System;
using System.Security.Cryptography;
using System.Text;

namespace BubbleBot.Website.Extensions
{
    public static class CryptographyExtension
    {

        public static string GetMD5(this string input)
        {
            string md5Result = null;

            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                md5Result = BitConverter.ToString(result).Replace("-", "").ToLower();
            }

            return md5Result;
        }

    }
}
