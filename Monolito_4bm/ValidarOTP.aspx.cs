using System;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using Capa_Negocio;
using Monolito_4bm.Services;
using ZXing;

namespace Monolito_4bm
{
    public partial class ValidarOTP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            if (Session["IdUsuarioTemp"] == null)
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnVerificar_Click(object sender, EventArgs e)
        {
            string codigo = null;

            // 0. Código escaneado automáticamente (prioridad)
            if (!string.IsNullOrEmpty(hfScannedCode.Value))
            {
                codigo = hfScannedCode.Value.Trim();
                hfScannedCode.Value = "";
            }
            // 1. Ingreso manual
            else if (!string.IsNullOrEmpty(txtOTP.Text.Trim()))
            {
                codigo = txtOTP.Text.Trim();
            }
            // 2. Imagen QR subida
            else if (fileQR.HasFile)
            {
                string ext = Path.GetExtension(fileQR.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif")
                {
                    byte[] bytes = fileQR.FileBytes;
                    codigo = LeerCodigoQR(bytes);
                    if (string.IsNullOrEmpty(codigo))
                    {
                        MostrarSweetAlert("No se pudo leer un código QR de la imagen.", "warning");
                        return;
                    }
                }
                else
                {
                    MostrarSweetAlert("Formato de imagen no válido. Use JPG, PNG, BMP o GIF.", "warning");
                    return;
                }
            }
            else
            {
                MostrarSweetAlert("Ingrese el código manualmente, escanee o suba una imagen del QR.", "warning");
                return;
            }

            // Validar OTP contra la base de datos
            int usuId = Convert.ToInt32(Session["IdUsuarioTemp"]);
            CN_Usuario negocio = new CN_Usuario();
            string mensaje;
            int resultado = negocio.ValidarOTP(usuId, codigo, out mensaje);

            switch (resultado)
            {
                case 1: // Éxito
                    Session["UsuarioId"] = usuId;
                    Session["Usuario"] = Session["NickTemp"];
                    Session["Rol"] = Session["TipoUsuarioTemp"];
                    Session["NombreUsuario"] = Session["NombreUsuarioTemp"];
                    Session["FotoPerfil"] = Session["FotoPerfilTemp"];

                    bool recordarme = Session["Recordarme"] != null && (bool)Session["Recordarme"];
                    if (recordarme)
                    {
                        HttpCookie cookie = new HttpCookie("UserInfo");
                        cookie.Values["Username"] = Session["NickTemp"].ToString();
                        cookie.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Add(cookie);
                    }

                    // Limpiar temporales
                    Session.Remove("IdUsuarioTemp");
                    Session.Remove("NickTemp");
                    Session.Remove("CorreoUsuario");
                    Session.Remove("Recordarme");
                    Session.Remove("TipoUsuarioTemp");
                    Session.Remove("NombreUsuarioTemp");
                    Session.Remove("FotoPerfilTemp");

                    MostrarSweetAlert("Código verificado con éxito. Redirigiendo...", "success", "Default.aspx");
                    break;

                case 0:
                    MostrarSweetAlert("Código incorrecto.", "error");
                    break;
                case -1:
                    MostrarSweetAlert("El código ha expirado. Solicite uno nuevo.", "warning");
                    break;
                case -2:
                    MostrarSweetAlert("Este código ya fue utilizado. Solicite uno nuevo.", "warning");
                    break;
                default:
                    MostrarSweetAlert(mensaje, "error");
                    break;
            }
        }

        protected void btnReenviar_Click(object sender, EventArgs e)
        {
            try
            {
                int usuId = Convert.ToInt32(Session["IdUsuarioTemp"]);
                CN_Usuario negocio = new CN_Usuario();

                if (negocio.ExisteOTPValido(usuId))
                {
                    MostrarSweetAlert("El código ya fue enviado. Espere 10 minutos o use el código recibido.", "warning");
                    return;
                }

                string correo = Session["CorreoUsuario"].ToString();
                string codigoOTP = negocio.GenerarOTP(usuId);

                QRService qrService = new QRService();
                string qrBase64 = qrService.GenerarQRBase64(codigoOTP);

                string asunto = "Nuevo Código de Acceso - Óptica 4BM";

                // Actualizado: Plantilla HTML idéntica a la del Login para mantener coherencia
                string cuerpo = $@"
                    <div style='font-family: Arial, sans-serif; color: #333; max-width: 400px; margin: 0 auto; border: 1px solid #eaeaea; padding: 20px; border-radius: 8px;'>
                        <h3 style='text-align: center; margin-top: 0;'>Validación de Acceso</h3>
                        <p style='font-size: 14px;'>Tu nuevo código OTP de seguridad es:</p>
                        <h2 style='background: #f8f9fa; padding: 10px; text-align: center; letter-spacing: 2px; border-radius: 4px; font-size: 24px; color: #212529;'>{codigoOTP}</h2>
                        <p style='font-size: 14px; text-align: center; margin-top: 20px;'>O escanea el siguiente código QR:</p>
                        <div style='text-align: center;'>
                            <img src='data:image/png;base64,{qrBase64}' alt='QR OTP' style='width: 150px; height: 150px; border: 1px solid #eee; border-radius: 8px; padding: 5px;' />
                        </div>
                        <p style='font-size: 12px; color: #888; text-align: center; margin-top: 20px;'>Este código será válido por 10 minutos.</p>
                    </div>";

                EmailService emailService = new EmailService();
                bool enviado = emailService.EnviarCorreo(correo, asunto, cuerpo);

                if (enviado)
                    MostrarSweetAlert("Nuevo código enviado con éxito al correo.", "success");
                else
                    MostrarSweetAlert("Error al enviar el correo. Revise su conexión.", "error");
            }
            catch (Exception ex)
            {
                MostrarSweetAlert("Error: " + ex.Message, "error");
            }
        }

        private string LeerCodigoQR(byte[] imagenBytes)
        {
            try
            {
                using (var ms = new MemoryStream(imagenBytes))
                {
                    var reader = new BarcodeReader();
                    using (var bitmap = (Bitmap)Image.FromStream(ms))
                    {
                        var result = reader.Decode(bitmap);
                        return result?.Text;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private void MostrarSweetAlert(string mensaje, string tipo, string redirigir = null)
        {
            hfMsgType.Value = tipo;
            hfMsgText.Value = mensaje;
            hfRedirectUrl.Value = redirigir ?? "";
        }
    }
}