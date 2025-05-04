using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GiacintTrustEncrypt.Lib
{
    internal class Encryptor
    {
        private readonly byte[] key; // 32 байта

        public Encryptor(string keyString)
        {
            key = Encoding.ASCII.GetBytes(keyString);

            if (key.Length != 32)
            {
                Console.WriteLine(key.Length);
                //throw new ArgumentException("Ключ должен быть длиной 32 байта (256 бит).");
            }
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.GenerateIV();
            var iv = aes.IV;

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Объединяем IV + encrypted
            byte[] result = new byte[iv.Length + encrypted.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string base64CipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(base64CipherText);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] iv = new byte[16]; // AES block size = 16 байт
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
