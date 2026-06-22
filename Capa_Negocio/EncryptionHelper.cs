using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Capa_Negocio
{
    public static class EncryptionHelper
    {
        private static readonly byte[] Salt = new byte[0]; // No se usa salt
        private static readonly byte[] IV = new byte[8];   // 8 bytes en 0, igual que SQL Server

        // Método para encriptar (usado al registrar o migrar datos)
        public static byte[] Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            using (var tdes = new TripleDESCryptoServiceProvider())
            {
                tdes.Key = DeriveKey("cl@ve", 16); // 16 bytes = 128 bits
                tdes.IV = IV;
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, tdes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.Unicode.GetBytes(plainText); // SQL Server usa Unicode
                        cs.Write(plainBytes, 0, plainBytes.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }

        // Método para desencriptar (usado en el login)
        public static string Decrypt(byte[] cipherBytes)
        {
            if (cipherBytes == null || cipherBytes.Length == 0)
                throw new ArgumentNullException(nameof(cipherBytes));

            using (var tdes = new TripleDESCryptoServiceProvider())
            {
                tdes.Key = DeriveKey("cl@ve", 16);
                tdes.IV = IV;
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream(cipherBytes))
                {
                    using (var cs = new CryptoStream(ms, tdes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs, Encoding.Unicode))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        // Derivar clave usando SHA1, igual que ENCRYPTBYPASSPHRASE
        private static byte[] DeriveKey(string passphrase, int keyLength)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(passphrase));
                byte[] key = new byte[keyLength];
                Array.Copy(hash, 0, key, 0, Math.Min(keyLength, hash.Length));
                return key;
            }
        }
    }
}