using System;
using Capa_Negocio;

namespace Monolito_4bm
{
    public partial class ConfigurarOTP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null) { Response.Redirect("Login.aspx"); return; }
            if (!IsPostBack) CargarEstado();
        }

        private void CargarEstado()
        {
            CN_Usuario negocio = new CN_Usuario();
            var usuario = negocio.ObtenerUsuarioPorNick(Session["Usuario"].ToString());
            if (usuario != null && !string.IsNullOrEmpty(usuario.usu_otp_secret))
            {
                lblEstado.Text = "ACTIVADO";
                lblEstado.CssClass = "status-active";
                pnlConfigurar.Visible = true;
                // Permitir reconfigurar
            }
            else
            {
                lblEstado.Text = "NO CONFIGURADO";
                lblEstado.CssClass = "status-inactive";
            }
        }

        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                string secreto = CN_OTP.GenerarSecreto();
                ViewState["OtpSecret"] = secreto;

                string nick = Session["Usuario"].ToString();
                string uri = CN_OTP.GenerarOtpAuthUri(nick, secreto);

                // Generar QR desde servidor
                byte[] qrBytes = CN_OTP.GenerarQRImagen(uri);
                imgQR.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(qrBytes);
                lblSecret.Text = secreto;
                pnlQR.Visible = true;
            }
            catch (Exception ex)
            {
                hfMsgType.Value = "error";
                hfMsg.Value = "Error al generar QR: " + ex.Message;
            }
        }

        protected void btnVerificar_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigoOTP.Text.Trim();
            string secreto = ViewState["OtpSecret"] as string;

            if (string.IsNullOrEmpty(codigo) || codigo.Length != 6)
            {
                hfMsgType.Value = "error";
                hfMsg.Value = "Ingrese un codigo OTP de 6 digitos.";
                pnlQR.Visible = true;
                return;
            }

            if (string.IsNullOrEmpty(secreto))
            {
                hfMsgType.Value = "error";
                hfMsg.Value = "Genere un nuevo QR primero.";
                return;
            }

            if (CN_OTP.ValidarOTP(secreto, codigo))
            {
                // Guardar secreto en BD
                int usu_id = (int)Session["UsuarioId"];
                CN_Usuario negocio = new CN_Usuario();
                negocio.GuardarOtpSecret(usu_id, secreto);

                hfMsgType.Value = "success";
                hfMsg.Value = "OTP configurado exitosamente. Se requerira en cada login.";
                pnlQR.Visible = false;
                CargarEstado();
            }
            else
            {
                hfMsgType.Value = "error";
                hfMsg.Value = "Codigo OTP incorrecto. Intente de nuevo.";
                pnlQR.Visible = true;
            }
        }
    }
}
