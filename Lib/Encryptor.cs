using System;
using System.Security.Cryptography;
using System.Text;

namespace GiacintTrustEncrypt.Lib
{
    internal class Encryptor
    {
        private readonly byte[] key; // 32 байта = 256 бит

        public Encryptor(string keyString)
        {
            if (keyString.Length != 32)
                throw new ArgumentException("Ключ должен быть длиной 32 символа.");

            key = Encoding.UTF8.GetBytes(keyString);
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV(); // Случайный IV

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Конкатенация: IV (16 байт) + зашифрованные данные
            byte[] result = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string encryptedText)
        {
            byte[] fullData = Convert.FromBase64String(encryptedText);

            if (fullData.Length < 16)
                throw new ArgumentException("Данные повреждены или не соответствуют ожидаемому формату.");

            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Извлекаем IV (первые 16 байт)
            byte[] iv = new byte[16];
            Buffer.BlockCopy(fullData, 0, iv, 0, iv.Length);
            aes.IV = iv;

            // Остальное — зашифрованный текст
            int cipherLength = fullData.Length - iv.Length;
            byte[] cipherBytes = new byte[cipherLength];
            Buffer.BlockCopy(fullData, iv.Length, cipherBytes, 0, cipherLength);

            using var decryptor = aes.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
