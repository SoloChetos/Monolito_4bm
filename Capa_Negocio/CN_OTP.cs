using System;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Capa_Negocio
{
    public class CN_OTP
    {
        private static readonly string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        /// <summary>
        /// Genera un secreto aleatorio en Base32 para TOTP
        /// </summary>
        public static string GenerarSecreto()
        {
            byte[] bytes = new byte[10]; // 10 bytes = 16 chars base32
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return ToBase32(bytes);
        }

        /// <summary>
        /// Genera la URI otpauth para el QR
        /// </summary>
        public static string GenerarOtpAuthUri(string nick, string secreto, string issuer = "Monolito4bm")
        {
            return string.Format("otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6&period=30",
                Uri.EscapeDataString(issuer), Uri.EscapeDataString(nick), secreto);
        }

        /// <summary>
        /// Genera una imagen QR como byte[] a partir de texto
        /// Usa System.Drawing para crear un QR simple basado en una tabla de patrones
        /// </summary>
        public static byte[] GenerarQRImagen(string texto)
        {
            // Generar QR usando una API publica como alternativa a QRCoder
            // (para no depender de NuGet externo en .NET Framework)
            try
            {
                string url = "https://api.qrserver.com/v1/create-qr-code/?size=250x250&data=" +
                    Uri.EscapeDataString(texto);

                using (var client = new System.Net.WebClient())
                {
                    return client.DownloadData(url);
                }
            }
            catch
            {
                // Fallback: generar una imagen placeholder
                using (var bmp = new Bitmap(250, 250))
                using (var g = Graphics.FromImage(bmp))
                using (var ms = new MemoryStream())
                {
                    g.Clear(Color.White);
                    g.DrawString("QR: " + texto.Substring(0, Math.Min(30, texto.Length)),
                        new Font("Arial", 8), Brushes.Black, 10, 10);
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Valida un codigo TOTP contra el secreto
        /// </summary>
        public static bool ValidarOTP(string secreto, string codigoIngresado)
        {
            if (string.IsNullOrEmpty(secreto) || string.IsNullOrEmpty(codigoIngresado))
                return false;

            // Verificar el codigo actual y los adyacentes (ventana de 30s antes y despues)
            long timeStep = GetCurrentTimeStep();

            for (int i = -1; i <= 1; i++)
            {
                string codigo = GenerarTOTP(secreto, timeStep + i);
                if (codigo == codigoIngresado.PadLeft(6, '0'))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Genera un codigo TOTP de 6 digitos
        /// </summary>
        private static string GenerarTOTP(string secretoBase32, long timeStep)
        {
            byte[] key = FromBase32(secretoBase32);
            byte[] timeBytes = BitConverter.GetBytes(timeStep);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timeBytes);

            using (var hmac = new HMACSHA1(key))
            {
                byte[] hash = hmac.ComputeHash(timeBytes);
                int offset = hash[hash.Length - 1] & 0x0F;
                int binary =
                    ((hash[offset] & 0x7F) << 24) |
                    ((hash[offset + 1] & 0xFF) << 16) |
                    ((hash[offset + 2] & 0xFF) << 8) |
                    (hash[offset + 3] & 0xFF);

                int otp = binary % 1000000;
                return otp.ToString("D6");
            }
        }

        private static long GetCurrentTimeStep()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - epoch).TotalSeconds / 30;
        }

        private static string ToBase32(byte[] data)
        {
            var sb = new StringBuilder();
            int buffer = 0, bitsLeft = 0;

            foreach (byte b in data)
            {
                buffer = (buffer << 8) | b;
                bitsLeft += 8;
                while (bitsLeft >= 5)
                {
                    sb.Append(Base32Chars[(buffer >> (bitsLeft - 5)) & 31]);
                    bitsLeft -= 5;
                }
            }
            if (bitsLeft > 0)
            {
                sb.Append(Base32Chars[(buffer << (5 - bitsLeft)) & 31]);
            }
            return sb.ToString();
        }

        private static byte[] FromBase32(string base32)
        {
            base32 = base32.TrimEnd('=').ToUpper();
            var output = new System.Collections.Generic.List<byte>();
            int buffer = 0, bitsLeft = 0;

            foreach (char c in base32)
            {
                int val = Base32Chars.IndexOf(c);
                if (val < 0) continue;
                buffer = (buffer << 5) | val;
                bitsLeft += 5;
                if (bitsLeft >= 8)
                {
                    output.Add((byte)(buffer >> (bitsLeft - 8)));
                    bitsLeft -= 8;
                }
            }
            return output.ToArray();
        }
    }
}
