using System;
using Capa_Negocio;
using Monolito_4bm.Services;

namespace Monolito_4bm
{
    public partial class RecuperarClave : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnEnviarClave_Click(object sender, EventArgs e)
        {
            string nickOCorreo = txtNickCorreo.Text.Trim();
            if (string.IsNullOrEmpty(nickOCorreo))
            {
                hfMsgType.Value = "error";
                hfMsgText.Value = "Ingrese su nick o correo electronico.";
                return;
            }

            try
            {
                CN_Usuario negocio = new CN_Usuario();
                string correo;
                string clave = negocio.GenerarClaveTemporal(nickOCorreo, out correo);

                if (clave.StartsWith("Error") || clave == "Usuario no encontrado")
                {
                    hfMsgType.Value = "error";
                    hfMsgText.Value = clave;
                    return;
                }

                // Enviar por Correo Electrónico
                EmailService emailService = new EmailService();
                bool enviado = emailService.EnviarClaveTemporal(correo, clave);

                if (enviado)
                {
                    hfMsgType.Value = "success";
                    hfMsgText.Value = "Clave enviada a su correo.";
                    hfRedirectUrl.Value = "CambiarClave.aspx";
                }
                else
                {
                    hfMsgType.Value = "error";
                    hfMsgText.Value = "Error al enviar el correo con la clave temporal.";
                }
            }
            catch (Exception ex)
            {
                hfMsgType.Value = "error";
                hfMsgText.Value = "Error: " + ex.Message;
            }
        }
    }
}
