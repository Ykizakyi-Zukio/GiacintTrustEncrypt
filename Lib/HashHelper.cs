using SHA3.Net;
using System;
using System.Security.Cryptography;
using System.Text;

namespace GiacintTrustEncrypt.Lib
{
    internal static class Shake256Helper
    {
        /// <summary>
        /// Хеширует пароль с использованием SHAKE256 без соли.
        /// </summary>
        public static string Hash(string password, int hashLength = 32)
        {
            // Преобразуем пароль в байты
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Используем SHAKE256 для хеширования
            var shake256 = new SHA3.Net.SHAKE256();
            byte[] hash = shake256.Hash(passwordBytes, hashLength);

            return Convert.ToBase64String(hash); // возвращаем хеш в формате Base64
        }

        /// <summary>
        /// Проверка пароля с хешом.
        /// </summary>
        public static bool Verify(string password, string storedHash, int hashLength = 32)
        {
            string computedHash = Hash(password, hashLength);
            return computedHash == storedHash;
        }
    }
}
