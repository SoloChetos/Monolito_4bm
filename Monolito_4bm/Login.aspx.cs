using System;
using System.Web.UI;
using Capa_Negocio;
using Monolito_4bm.Services;

namespace Monolito_4bm
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["logout"] == "1")
                {
                    hfMsgType.Value = "success";
                    hfMsgText.Value = "Has cerrado sesión exitosamente.";
                    hfRedirectUrl.Value = string.Empty;
                }
                else if (Request.QueryString["expired"] == "1")
                {
                    hfMsgType.Value = "warning";
                    hfMsgText.Value = "Por tu seguridad, la sesión expiró automáticamente.";
                    hfRedirectUrl.Value = string.Empty;
                }
            }
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            string nick = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            try
            {
                CN_Usuario negocio = new CN_Usuario();

                // 1. Obtener usuario para validar estado ANTES de autenticar
                var usuario = negocio.ObtenerUsuarioPorNick(nick);
                if (usuario == null)
                {
                    MostrarError("Usuario o contraseña incorrectos.");
                    return;
                }

                // usu_estado ahora es string, no char?
                string estado = (usuario.usu_estado ?? "A").ToUpper();
                if (estado != "A")
                {
                    string mensaje = estado == "B"
                        ? "Usuario bloqueado. Contacte al administrador."
                        : "Usuario inactivo. No puede iniciar sesión.";
                    MostrarError(mensaje);
                    return;
                }

                // 2. Autenticar (solo si está activo)
                string mensajeLogin = negocio.IniciarSesion(nick, password);
                if (!mensajeLogin.ToLower().Contains("exitoso"))
                {
                    MostrarError(mensajeLogin);
                    return;
                }

                // 3. Login exitoso → guardar temporales
                Session["IdUsuarioTemp"] = usuario.usu_id;
                Session["NickTemp"] = nick;
                Session["CorreoUsuario"] = usuario.usu_correo;
                Session["Recordarme"] = chkRemember.Checked;
                Session["TipoUsuarioTemp"] = usuario.tusu_id.HasValue ? usuario.tusu_id.Value : 2;
                Session["NombreUsuarioTemp"] = (usuario.usu_nombres ?? "") + " " + (usuario.usu_apellidos ?? "");

                // usu_foto ahora es byte[], no Binary
                if (usuario.usu_foto != null && usuario.usu_foto.Length > 0)
                {
                    Session["FotoPerfilTemp"] = Convert.ToBase64String(usuario.usu_foto);
                }

                // 4. Generar OTP, QR y enviar correo
                string codigoOTP = negocio.GenerarOTP(usuario.usu_id);

                QRService qrService = new QRService();
                string qrBase64 = qrService.GenerarQRBase64(codigoOTP);

                string asunto = "Código de Acceso - Óptica 4BM";

                string cuerpo = $@"
                    <div style='font-family: Arial, sans-serif; color: #333; max-width: 400px; margin: 0 auto; border: 1px solid #eaeaea; padding: 20px; border-radius: 8px;'>
                        <h3 style='text-align: center; margin-top: 0;'>Validación de Acceso</h3>
                        <p style='font-size: 14px;'>Tu código OTP de seguridad es:</p>
                        <h2 style='background: #f8f9fa; padding: 10px; text-align: center; letter-spacing: 2px; border-radius: 4px; font-size: 24px; color: #212529;'>{codigoOTP}</h2>
                        <p style='font-size: 14px; text-align: center; margin-top: 20px;'>O escanea el siguiente código QR:</p>
                        <div style='text-align: center;'>
                            <img src='data:image/png;base64,{qrBase64}' alt='QR OTP' style='width: 150px; height: 150px; border: 1px solid #eee; border-radius: 8px; padding: 5px;' />
                        </div>
                        <p style='font-size: 12px; color: #888; text-align: center; margin-top: 20px;'>Este código será válido por 10 minutos.</p>
                    </div>";

                EmailService emailService = new EmailService();
                bool enviado = emailService.EnviarCorreo(usuario.usu_correo, asunto, cuerpo);

                if (enviado)
                {
                    hfMsgType.Value = "success";
                    hfMsgText.Value = "Se ha enviado un código de acceso a su correo.";
                    hfRedirectUrl.Value = "ValidarOTP.aspx";
                }
                else
                {
                    MostrarError("Error al enviar el correo OTP. Revise su conexión o credenciales.");
                }
            }
            catch (Exception)
            {
                MostrarError("Error de conexión con el sistema.");
            }
        }

        protected void btnShowRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("registrar.aspx");
        }

        private void MostrarError(string mensaje)
        {
            hfMsgType.Value = "error";
            hfMsgText.Value = mensaje;
            hfRedirectUrl.Value = "";
        }
    }
}