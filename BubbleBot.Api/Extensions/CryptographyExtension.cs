using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BubbleBot.Api.Extensions
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

        private static Random random = new Random();
        public static string GenerateString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
