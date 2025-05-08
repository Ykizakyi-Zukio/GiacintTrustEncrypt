using System.Security.Cryptography;
using System.Text;

namespace GiacintTrustEncrypt.Lib
{
    internal class Key
    {
        private readonly byte[] key; // 256-битный ключ
        private byte[] encryptedData; // nonce + ciphertext + tag

        public Key(string plaintext)
        {
            key = RandomNumberGenerator.GetBytes(32); // AES-256
            Encrypt(plaintext);
        }

        private void Encrypt(string plaintext)
        {
            byte[] source = Encoding.UTF8.GetBytes(plaintext);
            byte[] nonce = RandomNumberGenerator.GetBytes(12); // 96-битный nonce (рекомендуется для AES-GCM)
            byte[] ciphertext = new byte[source.Length];
            byte[] tag = new byte[16]; // 128-битный тег

            using var aes = new AesGcm(key);
            aes.Encrypt(nonce, source, ciphertext, tag);

            // Объединяем nonce + ciphertext + tag
            encryptedData = new byte[nonce.Length + ciphertext.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, encryptedData, 0, nonce.Length);
            Buffer.BlockCopy(ciphertext, 0, encryptedData, nonce.Length, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, encryptedData, nonce.Length + ciphertext.Length, tag.Length);
        }

        internal string Get()
        {
            var decrypted = GetBytes();
            return Encoding.UTF8.GetString(decrypted);
        }


        internal byte[] GetBytes()
        {
            int nonceLen = 12;
            int tagLen = 16;

            if (encryptedData.Length < nonceLen + tagLen)
                throw new Exception("Encrypted data is too short");

            byte[] nonce = new byte[nonceLen];
            Buffer.BlockCopy(encryptedData, 0, nonce, 0, nonceLen);

            int ciphertextLen = encryptedData.Length - nonceLen - tagLen;
            byte[] ciphertext = new byte[ciphertextLen];
            Buffer.BlockCopy(encryptedData, nonceLen, ciphertext, 0, ciphertextLen);

            byte[] tag = new byte[tagLen];
            Buffer.BlockCopy(encryptedData, nonceLen + ciphertextLen, tag, 0, tagLen);

            byte[] decrypted = new byte[ciphertextLen];
            using var aes = new AesGcm(key);
            aes.Decrypt(nonce, ciphertext, tag, decrypted);

            // Перешифровываем заново с новым nonce
            Encrypt(Encoding.UTF8.GetString(decrypted));

            return decrypted;
        }
    }
}
