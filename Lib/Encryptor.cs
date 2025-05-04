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

        public byte[] Encrypt(byte[] plainData)
        {
            using var aes = Aes.Create();
            aes.Key = key; // Ключ 32 байта
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV(); // Генерируем случайный IV

            using var encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);

            // Объединяем IV + зашифрованные данные
            byte[] fullData = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, fullData, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, fullData, aes.IV.Length, encrypted.Length);

            return fullData;
        }


        public byte[] Decrypt(byte[] encryptedData)
        {
            if (encryptedData.Length < 16) throw new ArgumentException("Файл повреждён!");

            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Извлекаем IV (первые 16 байт)
            byte[] iv = new byte[16];
            Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
            aes.IV = iv;

            // Извлекаем само зашифрованное содержимое
            int cipherLen = encryptedData.Length - iv.Length;
            byte[] cipher = new byte[cipherLen];
            Buffer.BlockCopy(encryptedData, iv.Length, cipher, 0, cipherLen);

            using var decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return decrypted;
        }

    }
}
