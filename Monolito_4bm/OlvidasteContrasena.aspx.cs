using System;
using System.Web.UI;
using Capa_Negocio;

namespace Monolito_4bm
{
    public partial class OlvidasteContrasena : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No requiere sesión
        }

        protected void btnEnviarClave_Click(object sender, EventArgs e)
        {
            string identificador = txtIdentificador.Text.Trim();
            if (string.IsNullOrEmpty(identificador))
            {
                MostrarMensaje("Ingrese su nick o correo.", "warning");
                return;
            }

            try
            {
                CN_Usuario negocio = new CN_Usuario();

                // Buscar al usuario por nick o correo
                var usuario = negocio.ObtenerUsuarioPorNick(identificador)
                              ?? negocio.ObtenerUsuarioPorCorreo(identificador);

                if (usuario == null)
                {
                    MostrarMensaje("Usuario no encontrado.", "error");
                    return;
                }

                // Generar clave temporal (el método también guarda en BD)
                string claveTemporal = negocio.GenerarClaveTemporal(identificador, out string correoEncontrado);
                if (claveTemporal.StartsWith("Error") || claveTemporal.StartsWith("Usuario no encontrado"))
                {
                    MostrarMensaje(claveTemporal, "error");
                    return;
                }

                // Formatear número de celular (Ecuador: 593 + últimos 9 dígitos)
                string celular = usuario.usu_celular ?? "";
                if (celular.Length >= 10)
                {
                    celular = "593" + celular.Substring(celular.Length - 9);
                }
                else
                {
                    MostrarMensaje("El usuario no tiene un número de celular válido.", "error");
                    return;
                }

                // Guardar el nick en sesión temporal para usarlo al cambiar contraseña
                Session["NickRecuperacion"] = usuario.usu_nick;

                string mensaje = $"Tu clave temporal para recuperar la contraseña es: {claveTemporal}";
                string urlWa = $"https://wa.me/{celular}?text={Uri.EscapeDataString(mensaje)}";

                // Abrir WhatsApp en el navegador del cliente
                string script = $"window.open('{urlWa}', '_blank');";
                ClientScript.RegisterStartupScript(this.GetType(), "whatsapp", script, true);

                MostrarMensaje("Clave temporal enviada a WhatsApp. Redirigiendo para cambiar la contraseña...", "success", "CambiarContrasena.aspx");
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