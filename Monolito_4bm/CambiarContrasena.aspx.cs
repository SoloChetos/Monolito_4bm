using System;
using System.Web.UI;
using Capa_Negocio;

namespace Monolito_4bm
{
    public partial class CambiarContrasena : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No requiere sesión, pero el nick podría pasarse por query string o por sesión temporal
            if (Session["NickRecuperacion"] == null)
            {
                Response.Redirect("OlvidasteContrasena.aspx");
            }
        }

        protected void btnCambiar_Click(object sender, EventArgs e)
        {
            string claveTemporal = txtClaveTemporal.Text.Trim();
            string nuevaPass = txtNuevaPassword.Text.Trim();
            string confirmarPass = txtConfirmarPassword.Text.Trim();

            if (string.IsNullOrEmpty(claveTemporal) || string.IsNullOrEmpty(nuevaPass) || string.IsNullOrEmpty(confirmarPass))
            {
                MostrarMensaje("Todos los campos son obligatorios.", "warning");
                return;
            }

            if (nuevaPass != confirmarPass)
            {
                MostrarMensaje("Las contraseñas no coinciden.", "warning");
                return;
            }

            if (nuevaPass.Length < 6)
            {
                MostrarMensaje("La contraseña debe tener al menos 6 caracteres.", "warning");
                return;
            }

            try
            {
                CN_Usuario negocio = new CN_Usuario();
                string nick = Session["NickRecuperacion"].ToString();
                string resultado = negocio.CambiarContrasenaConClave(nick, claveTemporal, nuevaPass);

                if (resultado.ToLower().Contains("exitosa"))
                {
                    Session.Remove("NickRecuperacion");
                    MostrarMensaje("Contraseña actualizada exitosamente. Ya puedes iniciar sesión.", "success", "Login.aspx");
                }
                else
                {
                    MostrarMensaje(resultado, "error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, "error");
            }
        }

        private void MostrarMensaje(string mensaje, string tipo, string redirectUrl = null)
        {
            hfMsgType.Value = tipo;
            hfMsgText.Value = mensaje;
            hfRedirectUrl.Value = redirectUrl ?? "";
        }
    }
}