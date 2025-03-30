using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Modules
{
    public static class EncryptionHelper
    {
        // Key và IV cho AES-256 (32 bytes key, 16 bytes IV)
        private static readonly byte[] aesKey = Encoding.UTF8.GetBytes("Kv3F!yEJ_z&d-MVy;JKVOz.sHow!qSE-"); // 32 ký tự
        private static readonly byte[] aesIV = Encoding.UTF8.GetBytes("abcdef9876543210"); // 16 ký tự        

        /// <summary>
        /// Mã hóa chuỗi sử dụng AES-256 và trả về chuỗi đã mã hóa dạng Base64.
        /// </summary>
        public static string Encrypt(string plainText)
        {
#if UNITY_EDITOR
            return plainText;
#endif
            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = aesKey;
                aesAlg.IV = aesIV;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            // Chuyển đổi mảng byte thành chuỗi Base64 để lưu trữ dễ dàng.
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Giải mã chuỗi Base64 đã mã hóa bởi AES-256.
        /// </summary>
        public static string Decrypt(string cipherText)
        {
#if UNITY_EDITOR
            return cipherText;
#endif
            string plaintext = null;
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = aesKey;
                aesAlg.IV = aesIV;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}