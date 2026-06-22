using Capa_Negocio;
using Capa_Datos;
using System;
using System.Web.UI;

namespace Monolito_4bm
{
    public partial class ProveedorEditar : System.Web.UI.Page
    {
        private CN_Proveedor proveedorNegocio = new CN_Proveedor();
        private int proveedorId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "1")
            {
                Response.Redirect("Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["id"] == null || !int.TryParse(Request.QueryString["id"], out proveedorId))
                {
                    Response.Redirect("Proveedores.aspx");
                    return;
                }

                hfProveedorId.Value = proveedorId.ToString();
                CargarDatos();
            }
            else
            {
                proveedorId = int.Parse(hfProveedorId.Value);
            }
        }

        private void CargarDatos()
        {
            var prov = proveedorNegocio.ObtenerPorId(proveedorId);
            if (prov == null)
            {
                MostrarMensaje("Proveedor no encontrado.", "error");
                return;
            }
            txtNombre.Text = prov.prov_nombre;
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
                proveedorNegocio.Actualizar(proveedorId, nombre);
                MostrarMensaje("Proveedor actualizado correctamente.", "success", "Proveedores.aspx");
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