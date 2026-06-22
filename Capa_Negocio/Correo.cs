using System;
using System.Net;
using System.Net.Mail;

namespace Capa_Negocio
{
    public class Correo
    {
        string from = "soportemedikids@gmail.com";
        string pass = "jygc qgwj vkqx edzf";

        public bool enviar_correo(string to, string msj)
        {
            try
            {
                MailMessage m = new MailMessage();

                m.From = new MailAddress(from);
                m.To.Add(to);
                m.Subject = "Recuperación de contraseña";
                m.Body = msj;
                m.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();

                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential(from, pass);
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;

                smtp.Send(m);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar correo: " + ex.Message);
                return false;
            }
        }
    }
}