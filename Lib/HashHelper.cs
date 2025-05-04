using SHA3.Net;
using System;
using System.Security.Cryptography;
using System;
using System.Text;

namespace GiacintTrustEncrypt.Lib;
internal class HashHelper
{
    internal static string Hash(string input) => Convert.ToBase64String(HashBytes(input));

    internal static byte[] HashBytes(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] fullHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            byte[] hash24 = new byte[24];
            Array.Copy(fullHash, hash24, 24); // Обрезаем до 24 байт
            return hash24;
        }
    }
}
