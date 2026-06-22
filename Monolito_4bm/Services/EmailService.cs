using System;
using System.Net;
using System.Net.Mail;

namespace Monolito_4bm.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _emailFrom = "soportemedikids@gmail.com";
        private readonly string _password = "jygc qgwj vkqx edzf";

        public bool EnviarCorreo(string destinatario, string asunto, string cuerpoHtml)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_emailFrom, "Monolito 4BM");
                    mail.To.Add(destinatario);
                    mail.Subject = asunto;
                    mail.Body = cuerpoHtml;
                    mail.IsBodyHtml = true;
                    mail.BodyEncoding = System.Text.Encoding.UTF8;

                    using (SmtpClient smtp = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(_emailFrom, _password);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 10000;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error Email: " + ex.Message);
                return false;
            }
        }

        public bool EnviarClaveTemporal(string email, string claveTemporal)
        {
            string asunto = "Recuperación de Contraseña - Monolito 4BM";
            string cuerpo = $@"
                <html><body>
                    <h2>Recuperación de Contraseña</h2>
                    <p>Tu clave temporal es: <strong>{claveTemporal}</strong></p>
                    <p>Válida por 15 minutos.</p>
                </body></html>";
            return EnviarCorreo(email, asunto, cuerpo);
        }
    }
}
