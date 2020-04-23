using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BubbleBot.Utility.Security
{
    public static class AESEncryption
    {
        public static string Encrypt(string value, string password)
        {
            var vectorBytes = Encoding.ASCII.GetBytes(_vector);
            var saltBytes = Encoding.ASCII.GetBytes(_salt);
            var valueBytes = Encoding.ASCII.GetBytes(value);

            byte[] encrypted;
            using (var cipher = Aes.Create())
            {
                var _passwordBytes = new Rfc2898DeriveBytes(password, saltBytes, _iterations);
                var keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                using (var encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
                {
                    using (var to = new MemoryStream())
                    {
                        using (var writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(valueBytes, 0, valueBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }

                //cipher.Clear();
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string value, string password)
        {
            var vectorBytes = Encoding.ASCII.GetBytes(_vector);
            var saltBytes = Encoding.ASCII.GetBytes(_salt);
            var valueBytes = Convert.FromBase64String(value);

            byte[] decrypted;
            var decryptedByteCount = 0;

            using (var cipher = Aes.Create())
            {
                var _passwordBytes = new Rfc2898DeriveBytes(password, saltBytes, _iterations);
                var keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                try
                {
                    using (var decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
                    {
                        using (var from = new MemoryStream(valueBytes))
                        {
                            using (var reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                            {
                                decrypted = new byte[valueBytes.Length];
                                decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                            }
                        }
                    }
                }
                catch
                {
                    return string.Empty;
                }

                //cipher.Clear();
            }

            return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        }

        #region Settings

        private static readonly int _iterations = 10;
        private static readonly int _keySize = 256;

        private static readonly string _salt = "aselrias38490a32";
        private static readonly string _vector = "8947az34awl34kjq";

        #endregion
    }
}