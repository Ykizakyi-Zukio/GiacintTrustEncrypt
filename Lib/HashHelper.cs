using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using System;
using System.Security.Cryptography;
using System.Text;

namespace GiacintTrustEncrypt.Lib
{
    internal static class HashHelper
    {
        public static string Hash(string password, int hashLength = 32)
        {
            if (hashLength < 4 || hashLength > 512)
                throw new ArgumentOutOfRangeException(nameof(hashLength));

            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            var config = new Argon2Config
            {
                Password = Encoding.UTF8.GetBytes(password),
                Salt = salt,
                HashLength = hashLength,
                TimeCost = 4,
                MemoryCost = 65536,
                Lanes = 2,
                Threads = 2,
                Type = Argon2Type.DataIndependentAddressing,
                Version = Argon2Version.Nineteen
            };

            using var argon2 = new Argon2(config);
            using var hash = argon2.Hash();

            return Convert.ToBase64String(hash.Buffer); // возвращаем только хеш (base64)
        }

        // Верификация невозможна без параметров и соли — добавь отдельно, если потребуется.
    }
}
