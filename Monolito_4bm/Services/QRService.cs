using System;
using System.Drawing;
using System.IO;
using QRCoder;

namespace Monolito_4bm.Services
{
    public class QRService
    {
        public string GenerarQRBase64(string contenido)
        {
            try
            {
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    QRCodeData qrData = qrGenerator.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q);
                    using (QRCode qrCode = new QRCode(qrData))
                    {
                        using (Bitmap bitmap = qrCode.GetGraphic(20))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                byte[] bytes = ms.ToArray();
                                return Convert.ToBase64String(bytes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error QR: " + ex.Message);
                return null;
            }
        }
    }
}
