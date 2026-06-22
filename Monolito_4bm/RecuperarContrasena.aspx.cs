using System;

namespace Monolito_4bm
{
    public partial class RecuperarContrasena : System.Web.UI.Page
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
                Capa_Negocio.CN_Usuario negocio = new Capa_Negocio.CN_Usuario();
                string celular, correo, nick;
                string clave = negocio.GenerarClaveTemporal(nickOCorreo, out celular, out correo, out nick);

                if (clave.StartsWith("Error") || clave == "Usuario no encontrado")
                {
                    hfMsgType.Value = "error";
                    hfMsgText.Value = clave;
                    return;
                }

                // Intentar enviar por WhatsApp
                string mensaje = string.Format(
                    "Monolito4bm - Tu clave temporal es: {0} (valida por 15 minutos). Tu nick es: {1}",
                    clave, nick);

                bool enviado = false;
                if (!string.IsNullOrEmpty(celular))
                {
                    enviado = Capa_Negocio.CN_Mensajeria.EnviarWhatsApp(celular, mensaje);
                }

                if (enviado)
                {
                    hfMsgType.Value = "success";
                    hfMsgText.Value = "Se envio la clave temporal a tu WhatsApp. Revisa tu celular " +
                        celular.Substring(0, 3) + "****" + celular.Substring(celular.Length - 2) + ".";
                    hfRedirectUrl.Value = "CambiarContrasena.aspx";
                }
                else
                {
                    // Si WhatsApp falla, mostrar la clave directamente (para entorno de pruebas)
                    hfMsgType.Value = "success";
                    hfMsgText.Value = "Tu clave temporal es: " + clave +
                        " (valida 15 min). Tu nick: " + nick +
                        ". Nota: WhatsApp no disponible, clave mostrada directamente.";
                    hfRedirectUrl.Value = "CambiarContrasena.aspx";
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
