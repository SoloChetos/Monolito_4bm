using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Monolito_4bm
{
    public partial class Productos : System.Web.UI.Page
    {
        private CN_Producto productoNegocio = new CN_Producto();
        private CN_Proveedor proveedorNegocio = new CN_Proveedor();

        private int PaginaActual
        {
            get { return (int)(ViewState["PaginaActual"] ?? 1); }
            set { ViewState["PaginaActual"] = value; }
        }

        private const int tamanoPagina = 12;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "1")
            {
                Response.Redirect("Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                PaginaActual = 1;
                CargarProveedores();
                CargarProductos();
            }
        }

        private void CargarProveedores()
        {
            var lista = proveedorNegocio.ListarActivos();
            ddlProveedorFiltro.DataSource = lista;
            ddlProveedorFiltro.DataTextField = "prov_nombre";
            ddlProveedorFiltro.DataValueField = "prov_id";
            ddlProveedorFiltro.DataBind();
        }

        private void CargarProductos()
        {
            string nombre = txtBuscar.Text;
            string provFiltro = ddlProveedorFiltro.SelectedValue;
            int? provId = null;
            if (provFiltro == "-1") provId = -1;
            else if (!string.IsNullOrEmpty(provFiltro)) provId = Convert.ToInt32(provFiltro);

            decimal? precioMin = string.IsNullOrEmpty(txtPrecioMin.Text) ? (decimal?)null : Convert.ToDecimal(txtPrecioMin.Text);
            decimal? precioMax = string.IsNullOrEmpty(txtPrecioMax.Text) ? (decimal?)null : Convert.ToDecimal(txtPrecioMax.Text);
            string orden = ddlOrden.SelectedValue;
            string estadoFiltro = ddlEstado.SelectedValue;

            DataTable dt = productoNegocio.BuscarProductosTodos(nombre, provId, precioMin, precioMax, orden);

            if (estadoFiltro == "activos")
            {
                DataRow[] filas = dt.Select("pro_estado = 'A'");
                dt = filas.Any() ? filas.CopyToDataTable() : dt.Clone();
            }
            else if (estadoFiltro == "inactivos")
            {
                DataRow[] filas = dt.Select("pro_estado = 'I'");
                dt = filas.Any() ? filas.CopyToDataTable() : dt.Clone();
            }

            int totalRegistros = dt.Rows.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanoPagina);
            if (PaginaActual > totalPaginas) PaginaActual = totalPaginas;
            if (PaginaActual < 1) PaginaActual = 1;

            int inicio = (PaginaActual - 1) * tamanoPagina;
            DataTable dtPagina = dt.Clone();
            for (int i = inicio; i < inicio + tamanoPagina && i < totalRegistros; i++)
                dtPagina.ImportRow(dt.Rows[i]);

            rptProductos.DataSource = dtPagina;
            rptProductos.DataBind();

            lblSinResultados.Visible = dtPagina.Rows.Count == 0;
            litPaginaActual.Text = PaginaActual.ToString();
            litTotalPaginas.Text = totalPaginas.ToString();
            btnPagAnterior.Enabled = PaginaActual > 1;
            btnPagSiguiente.Enabled = PaginaActual < totalPaginas;
        }

        protected string ObtenerUrlImagen(object ruta)
        {
            string r = ruta as string;
            if (string.IsNullOrEmpty(r))
                return ResolveUrl("~/Images/Productos/placeholder.png");
            return ResolveUrl("~/" + r);
        }

        public string ObtenerProveedor(object provNombre)
        {
            string nombre = provNombre as string;
            return string.IsNullOrEmpty(nombre) ? "Sin proveedor" : nombre;
        }

        protected void rptProductos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                LinkButton lnkToggle = (LinkButton)e.Item.FindControl("lnkToggleEstado");
                if (lnkToggle != null)
                {
                    string[] args = lnkToggle.CommandArgument.Split('|');
                    string estado = args[1];
                    lnkToggle.Attributes["onclick"] = $"return confirmarToggle('{lnkToggle.UniqueID}', '{estado}');";

                    HtmlGenericControl icon = (HtmlGenericControl)lnkToggle.FindControl("iconoToggle");
                    if (icon != null)
                        icon.Attributes["class"] = estado == "A" ? "fas fa-toggle-on text-success" : "fas fa-toggle-off text-muted";
                }

                LinkButton lnkEliminar = (LinkButton)e.Item.FindControl("lnkEliminarFisico");
                if (lnkEliminar != null)
                    lnkEliminar.Attributes["onclick"] = $"return confirmarEliminacionFisica('{lnkEliminar.UniqueID}');";
            }
        }

        protected void rptProductos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ToggleEstado")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int id = Convert.ToInt32(args[0]);
                string nuevoEstado = args[1] == "A" ? "I" : "A";

                // Usar la capa de negocio (MongoDB)
                productoNegocio.CambiarEstadoProducto(id, nuevoEstado);
                MostrarMensaje($"Producto {(nuevoEstado == "A" ? "activado" : "desactivado")}.", "success");
                CargarProductos();
            }
            else if (e.CommandName == "EliminarFisico")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                productoNegocio.EliminarFisico(id);
                MostrarMensaje("Producto eliminado físicamente.", "success");
                CargarProductos();
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e) { PaginaActual = 1; CargarProductos(); }

        protected void btnPagAnterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                CargarProductos();
            }
        }

        protected void btnPagSiguiente_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            CargarProductos();
        }

        protected void btnNuevo_Click(object sender, EventArgs e) { Response.Redirect("NuevoProducto.aspx"); }
        protected void btnImportar_Click(object sender, EventArgs e) { Response.Redirect("ImportarExcel.aspx"); }

        protected void btnEliminarSeleccion_Click(object sender, EventArgs e)
        {
            List<int> ids = new List<int>();
            foreach (RepeaterItem item in rptProductos.Items)
            {
                CheckBox chk = (CheckBox)item.FindControl("chkSeleccion");
                if (chk != null && chk.Checked)
                    ids.Add(Convert.ToInt32(chk.Attributes["data-proid"]));
            }
            if (ids.Count > 0)
            {
                productoNegocio.EliminarSeleccionados(ids);
                MostrarMensaje($"{ids.Count} producto(s) eliminado(s).", "success");
                CargarProductos();
            }
        }

        protected void btnEliminarTodos_Click(object sender, EventArgs e)
        {
            productoNegocio.EliminarTodos();
            MostrarMensaje("Todos los productos han sido eliminados y el contador de IDs reiniciado.", "success");
            CargarProductos();
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            hfMsgType.Value = tipo;
            hfMsgText.Value = mensaje;
            hfRedirectUrl.Value = "";
        }
    }
}