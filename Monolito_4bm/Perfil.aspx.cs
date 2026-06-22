using System;
using System.IO;
using System.Web.UI;
using Capa_Negocio;

namespace Monolito_4bm
{
    public partial class Perfil : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDatosPerfil();
            }
        }

        private void CargarDatosPerfil()
        {
            int usuId = Convert.ToInt32(Session["UsuarioId"]);
            CN_Usuario negocio = new CN_Usuario();
            var usuario = negocio.ObtenerUsuarioPorId(usuId);
            if (usuario == null)
            {
                MostrarMensaje("Error al cargar datos del usuario.", "error");
                return;
            }

            txtCedula.Text = usuario.usu_cedula ?? "";
            txtNick.Text = usuario.usu_nick ?? "";
            txtNombres.Text = usuario.usu_nombres ?? "";
            txtApellidos.Text = usuario.usu_apellidos ?? "";
            txtDireccion.Text = usuario.usu_dirreccion ?? "";
            txtCelular.Text = usuario.usu_celular ?? "";
            txtCorreo.Text = usuario.usu_correo ?? "";
            txtFechaCumple.Text = usuario.usu_fecha_cumple.HasValue ? usuario.usu_fecha_cumple.Value.ToString("yyyy-MM-dd") : "";

            // Mensaje de cumpleaños
            if (usuario.usu_fecha_cumple.HasValue)
            {
                DateTime nac = usuario.usu_fecha_cumple.Value;
                int edad = CalcularEdad(nac);
                string mensajeCumple = $"Tienes {edad} años. ";
                if (nac.Month == DateTime.Now.Month && nac.Day == DateTime.Now.Day)
                    mensajeCumple += "¡Feliz cumpleaños! 🎂";
                else
                {
                    DateTime proximoCumple = new DateTime(DateTime.Now.Year, nac.Month, nac.Day);
                    if (proximoCumple < DateTime.Now)
                        proximoCumple = proximoCumple.AddYears(1);
                    int diasFaltantes = (int)(proximoCumple - DateTime.Now).TotalDays;
                    mensajeCumple += $"Tu próximo cumpleaños es en {diasFaltantes} día(s).";
                }
                infoCumple.InnerText = mensajeCumple;
            }

            // Foto actual
            if (Session["FotoPerfil"] != null)
            {
                string base64 = Session["FotoPerfil"].ToString();
                imgPreview.ImageUrl = "data:image/jpeg;base64," + base64;
                hfBase64Preview.Value = base64;
            }
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            int edad = DateTime.Now.Year - fechaNacimiento.Year;
            if (DateTime.Now.DayOfYear < fechaNacimiento.DayOfYear)
                edad--;
            return edad;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int usuId = Convert.ToInt32(Session["UsuarioId"]);
                string nombres = txtNombres.Text.Trim();
                string apellidos = txtApellidos.Text.Trim();

                if (string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos))
                {
                    MostrarMensaje("Nombres y apellidos son obligatorios.", "warning");
                    return;
                }

                CN_Usuario negocio = new CN_Usuario();

                // Actualizar datos básicos (usando el método con 7 parámetros)
                DateTime? fechaCumple = string.IsNullOrEmpty(txtFechaCumple.Text) ? (DateTime?)null : DateTime.Parse(txtFechaCumple.Text);
                string direccion = txtDireccion.Text.Trim();
                string celular = txtCelular.Text.Trim();
                string correo = txtCorreo.Text.Trim();

                string msj = negocio.ActualizarDatosUsuario(usuId, nombres, apellidos, direccion, celular, correo, fechaCumple);

                // Si se subió una nueva foto
                if (fuFotoPerfil.HasFile)
                {
                    if (fuFotoPerfil.PostedFile.ContentLength > 5 * 1024 * 1024)
                    {
                        MostrarMensaje("La imagen no debe superar los 5 MB.", "error");
                        return;
                    }
                    string ext = Path.GetExtension(fuFotoPerfil.FileName).ToLower();
                    if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif" && ext != ".webp")
                    {
                        MostrarMensaje("Formato de imagen no permitido.", "error");
                        return;
                    }

                    using (BinaryReader br = new BinaryReader(fuFotoPerfil.PostedFile.InputStream))
                    {
                        byte[] bytes = br.ReadBytes(fuFotoPerfil.PostedFile.ContentLength);
                        negocio.ActualizarFotoPerfil(usuId, bytes);

                        string base64 = Convert.ToBase64String(bytes);
                        Session["FotoPerfil"] = base64;
                        imgPreview.ImageUrl = "data:image/jpeg;base64," + base64;
                        hfBase64Preview.Value = base64;
                    }
                }

                // Actualizar sesión con nuevos nombres
                Session["NombreUsuario"] = nombres + " " + apellidos;

                MostrarMensaje("Perfil actualizado correctamente.", "success");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, "error");
            }
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            hfMsgType.Value = tipo;
            hfMsgText.Value = mensaje;
            hfRedirectUrl.Value = "";
        }
    }
}