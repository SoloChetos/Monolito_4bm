using System;
using Capa_Negocio;

namespace Monolito_4bm
{
    public partial class CambiarClave : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnCambiar_Click(object sender, EventArgs e)
        {
            string nick = txtNick.Text.Trim();
            string clave = txtClaveTemporal.Text.Trim();
            string nueva = txtNuevaPass.Text.Trim();
            string confirmar = txtConfirmarPass.Text.Trim();

            if (string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(clave) ||
                string.IsNullOrEmpty(nueva) || string.IsNullOrEmpty(confirmar))
            {
                hfMsgType.Value = "error";
                hfMsgText.Value = "Complete todos los campos.";
                return;
            }

            if (nueva != confirmar)
            {
                hfMsgType.Value = "error";
                hfMsgText.Value = "Las contraseñas no coinciden.";
                return;
            }

            if (nueva.Length < 4)
            {
                hfMsgType.Value = "error";
                hfMsgText.Value = "La contraseña debe tener al menos 4 caracteres.";
                return;
            }

            try
            {
                CN_Usuario negocio = new CN_Usuario();
                string resultado = negocio.CambiarContrasenaConClave(nick, clave, nueva);

                if (resultado.ToLower().Contains("exitosamente"))
                {
                    hfMsgType.Value = "success";
                    hfMsgText.Value = resultado;
                    hfRedirectUrl.Value = "Login.aspx";
                }
                else
                {
                    hfMsgType.Value = "error";
                    hfMsgText.Value = resultado;
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
