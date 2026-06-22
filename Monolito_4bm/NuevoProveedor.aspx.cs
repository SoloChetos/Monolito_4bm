using Capa_Negocio;
using System;
using System.Web.UI;

namespace Monolito_4bm
{
    public partial class NuevoProveedor : System.Web.UI.Page
    {
        private CN_Proveedor proveedorNegocio = new CN_Proveedor();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "1")
            {
                Response.Redirect("Default.aspx");
                return;
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                MostrarMensaje("El nombre del proveedor es obligatorio.", "warning");
                return;
            }

            try
            {
                proveedorNegocio.Insertar(nombre);
                MostrarMensaje("Proveedor creado correctamente.", "success", "Proveedores.aspx");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al guardar: " + ex.Message, "error");
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