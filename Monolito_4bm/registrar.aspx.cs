using System;
using System.IO;
using System.Web.UI;
using Capa_Negocio;

namespace Monolito_4bm
{
    public partial class registrar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack && ViewState["FotoBase64"] != null)
            {
                string base64 = ViewState["FotoBase64"].ToString();
                string tipo = ViewState["FotoTipo"] != null ? ViewState["FotoTipo"].ToString() : "image/jpeg";
                imgAvatar.ImageUrl = "data:" + tipo + ";base64," + base64;
                imgAvatar.Style["display"] = "block";
                lblAvatarIcon.Visible = false;
                imgPreview.ImageUrl = imgAvatar.ImageUrl;
                pnlPreview.Visible = true;
                btnQuitarFoto.Visible = true;
            }
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            lblFotoError.Visible = false;

            if (!fileFoto.HasFile)
            {
                lblFotoError.Text = "Seleccione un archivo primero.";
                lblFotoError.Visible = true;
                return;
            }

            string ext = Path.GetExtension(fileFoto.FileName).ToLower();
            string[] validExts = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (Array.IndexOf(validExts, ext) < 0)
            {
                lblFotoError.Text = "Formato no valido. Solo se permiten JPG, PNG, GIF o WebP.";
                lblFotoError.Visible = true;
                return;
            }

            if (fileFoto.PostedFile.ContentLength > 5 * 1024 * 1024)
            {
                lblFotoError.Text = "La imagen no debe superar los 5 MB.";
                lblFotoError.Visible = true;
                return;
            }

            fileFoto.PostedFile.InputStream.Position = 0;
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(fileFoto.PostedFile.InputStream))
            {
                bytes = br.ReadBytes(fileFoto.PostedFile.ContentLength);
            }

            string base64 = Convert.ToBase64String(bytes);
            string contentType = fileFoto.PostedFile.ContentType;

            ViewState["FotoBase64"] = base64;
            ViewState["FotoBytes"] = bytes;
            ViewState["FotoTipo"] = contentType;
            ViewState["FotoNombre"] = fileFoto.FileName;

            string dataUri = "data:" + contentType + ";base64," + base64;
            imgPreview.ImageUrl = dataUri;
            pnlPreview.Visible = true;
            btnQuitarFoto.Visible = true;

            imgAvatar.ImageUrl = dataUri;
            imgAvatar.Style["display"] = "block";
            lblAvatarIcon.Visible = false;
        }

        protected void btnQuitarFoto_Click(object sender, EventArgs e)
        {
            ViewState["FotoBase64"] = null;
            ViewState["FotoBytes"] = null;
            ViewState["FotoTipo"] = null;
            ViewState["FotoNombre"] = null;
            pnlPreview.Visible = false;
            btnQuitarFoto.Visible = false;
            imgAvatar.Style["display"] = "none";
            lblAvatarIcon.Visible = true;
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                string cedula = txtRegCedula.Text.Trim();
                string nombres = txtRegNombres.Text.Trim();
                string apellidos = txtRegApellidos.Text.Trim();
                string direccion = txtRegDireccion.Text.Trim();
                string celular = txtRegCelular.Text.Trim();
                string correo = txtRegCorreo.Text.Trim();
                DateTime fechaCumple;
                DateTime.TryParse(txtRegFechaCumple.Text, out fechaCumple);
                string nick = txtRegNick.Text.Trim();
                string password = txtRegPassword.Text.Trim();

                if (string.IsNullOrEmpty(cedula) || string.IsNullOrEmpty(nombres) ||
                    string.IsNullOrEmpty(apellidos) || string.IsNullOrEmpty(correo) ||
                    string.IsNullOrEmpty(password))
                {
                    hfMsgType.Value = "error";
                    hfMsgText.Value = "Complete todos los campos obligatorios.";
                    return;
                }

                if (fechaCumple == DateTime.MinValue || (DateTime.Today.Year - fechaCumple.Year - (DateTime.Today.DayOfYear < fechaCumple.DayOfYear ? 1 : 0)) < 18)
                {
                    hfMsgType.Value = "error";
                    hfMsgText.Value = "Debe ser mayor de edad (18 años) para registrarse.";
                    return;
                }

                CN_Usuario negocio = new CN_Usuario();

                if (negocio.ExisteCedula(cedula))
                {
                    hfMsgType.Value = "error";
                    hfMsgText.Value = "Ya existe un usuario registrado con esta cedula.";
                    return;
                }
                if (negocio.ExisteCorreo(correo))
                {
                    hfMsgType.Value = "error";
                    hfMsgText.Value = "Ya existe un usuario registrado con este correo electronico.";
                    return;
                }

                byte[] fotoBytes = ViewState["FotoBytes"] as byte[];
                int tusu_id = 2;

                string nickAsignado = negocio.RegistrarUsuario(
                    cedula, nombres, apellidos, direccion, celular,
                    correo, fechaCumple, nick, password, fotoBytes, tusu_id);

                if (!string.IsNullOrEmpty(nickAsignado))
                {
                    // La foto ya se guardó dentro del documento del usuario en MongoDB.
                    // No es necesario hacer nada más aquí.

                    hfMsgType.Value = "success";
                    hfMsgText.Value = "Usuario registrado correctamente. Tu nick es: " + nickAsignado;
                    hfRedirectUrl.Value = "Login.aspx";
                }
                else
                {
                    hfMsgType.Value = "error";
                    hfMsgText.Value = "No se pudo completar el registro.";
                }
            }
            catch (Exception ex)
            {
                hfMsgType.Value = "error";
                string msg = ex.Message;
                if (ex.InnerException != null) msg = ex.InnerException.Message;
                hfMsgText.Value = msg;
            }
        }
    }
}