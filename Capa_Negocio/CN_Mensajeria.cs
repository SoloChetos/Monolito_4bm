using System;
using System.Net;
using System.Text;

namespace Capa_Negocio
{
    public class CN_Mensajeria
    {
        /// <summary>
        /// Envia un mensaje de WhatsApp usando CallMeBot API
        /// El numero debe estar registrado previamente en CallMeBot:
        /// Enviar "I allow callmebot to send me messages" al +34 644 51 95 23
        /// </summary>
        public static bool EnviarWhatsApp(string celular, string mensaje, string apiKey = "")
        {
            try
            {
                // Formatear numero (Ecuador +593)
                string numero = celular;
                if (!numero.StartsWith("+"))
                {
                    if (numero.StartsWith("0"))
                        numero = "+593" + numero.Substring(1);
                    else
                        numero = "+593" + numero;
                }

                string mensajeCodificado = Uri.EscapeDataString(mensaje);

                // Si no hay apiKey, intentar enviar igual (modo demo)
                if (string.IsNullOrEmpty(apiKey))
                    apiKey = "DEMO_KEY";

                string url = string.Format(
                    "https://api.callmebot.com/whatsapp.php?phone={0}&text={1}&apikey={2}",
                    numero, mensajeCodificado, apiKey);

                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    string response = client.DownloadString(url);
                    return true;
                }
            }
            catch (Exception)
            {
                // Si falla el envio por WhatsApp, no bloquear el flujo
                return false;
            }
        }

        /// <summary>
        /// Envia un correo electronico con la clave temporal
        /// Metodo alternativo si WhatsApp falla
        /// </summary>
        public static bool EnviarCorreo(string destinatario, string asunto, string cuerpo,
            string smtpServer = "smtp.gmail.com", int puerto = 587,
            string usuario = "", string password = "")
        {
            try
            {
                if (string.IsNullOrEmpty(usuario)) return false;

                var mail = new System.Net.Mail.MailMessage();
                mail.From = new System.Net.Mail.MailAddress(usuario);
                mail.To.Add(destinatario);
                mail.Subject = asunto;
                mail.Body = cuerpo;
                mail.IsBodyHtml = true;

                var smtp = new System.Net.Mail.SmtpClient(smtpServer, puerto);
                smtp.Credentials = new NetworkCredential(usuario, password);
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
