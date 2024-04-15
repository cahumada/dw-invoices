using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace iaas.app.dw.invoices.Application.Base
{
    internal class AesEncryption
    {
      
        static byte[] key = new byte[32];
        static byte[] iv = [246, 58, 0, 213, 98, 219, 85, 11, 127, 135, 34, 94, 91, 146, 177, 204];

        internal AesEncryption(IConfiguration configuration)
        {
            string cipherKey = configuration.GetSection("Security:CipherKey").Value;
            key = Encoding.Unicode.GetBytes(cipherKey);
        }

        internal static byte[] Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        internal static string EncryptToString(string plainText)
        {
            return Convert.ToBase64String(Encrypt(plainText));
        }

        internal static string Decrypt(byte[] cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        internal static string DecryptFromString(string cipherText)
        {
            var cipher = Convert.FromBase64String(cipherText);

            return Decrypt(cipher);
        }
    }
}
