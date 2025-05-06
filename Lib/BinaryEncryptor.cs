using System.Security.Cryptography;
using System.Text;

namespace GiacintTrustEncrypt.Lib
{
    internal class BinaryEncryptor
    {
        private readonly byte[] key;

        public BinaryEncryptor(string _key)
        {
            key = Encoding.UTF8.GetBytes(_key);
        }

        internal byte[] Encrypt(byte[] data)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV(); // случайный IV
            aes.Padding = PaddingMode.PKCS7;

            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length); // сохраняем IV в начало

            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return ms.ToArray(); // [IV][ciphertext]
        }


        internal byte[] Decrypt(byte[] encryptedData)
        {
            byte[] iv = encryptedData[..16];             // первые 16 байт — IV
            byte[] ciphertext = encryptedData[16..];     // остальное — шифртекст

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.PKCS7;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(ciphertext, 0, ciphertext.Length);
            cs.FlushFinalBlock();

            return ms.ToArray(); // оригинальные данные
        }
    }
}
