using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Capa_Negocio;
using Capa_Datos.MongoModels;

namespace Monolito_4bm
{
    public partial class Proveedores : System.Web.UI.Page
    {
        private CN_Proveedor proveedorNegocio = new CN_Proveedor();

        private int PaginaActual
        {
            get { return (int)(ViewState["PaginaActual"] ?? 1); }
            set { ViewState["PaginaActual"] = value; }
        }
        private const int tamanoPagina = 10;

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
            }
        }

        protected void btnImportar_Click(object sender, EventArgs e) => Response.Redirect("ImportarProveedores.aspx");

        private void CargarProveedores()
        {
            string filtroNombre = txtBuscar.Text.Trim();
            string estadoFiltro = ddlEstado.SelectedValue;
            string orden = ddlOrden.SelectedValue;

            var todos = proveedorNegocio.ListarTodos();

            if (!string.IsNullOrEmpty(filtroNombre))
                todos = todos.Where(p => p.prov_nombre.IndexOf(filtroNombre, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            if (estadoFiltro == "activos")
                todos = todos.Where(p => p.prov_estado == "A").ToList();
            else if (estadoFiltro == "inactivos")
                todos = todos.Where(p => p.prov_estado == "I").ToList();

            if (orden == "recientes")
                todos = todos.OrderByDescending(p => p.prov_id).ToList();
            else
                todos = todos.OrderBy(p => p.prov_nombre).ToList();

            // Paginación manual
            int totalRegistros = todos.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanoPagina);
            if (PaginaActual > totalPaginas) PaginaActual = totalPaginas;
            if (PaginaActual < 1) PaginaActual = 1;

            int inicio = (PaginaActual - 1) * tamanoPagina;
            var pagina = todos.Skip(inicio).Take(tamanoPagina).ToList();

            gvProveedores.DataSource = pagina;
            gvProveedores.DataBind();

            litPaginaActual.Text = PaginaActual.ToString();
            litTotalPaginas.Text = totalPaginas.ToString();
            btnPagAnterior.Enabled = PaginaActual > 1;
            btnPagSiguiente.Enabled = PaginaActual < totalPaginas;
        }

        protected void gvProveedores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lnkToggle = (LinkButton)e.Row.FindControl("lnkToggleEstado");
                if (lnkToggle != null)
                {
                    string[] args = lnkToggle.CommandArgument.Split('|');
                    string estado = args[1];
                    lnkToggle.Attributes["onclick"] = $"return confirmarToggle('{lnkToggle.UniqueID}', '{estado}');";
                }

                LinkButton lnkEliminar = (LinkButton)e.Row.FindControl("lnkEliminarFisico");
                if (lnkEliminar != null)
                {
                    lnkEliminar.Attributes["onclick"] = $"return confirmarEliminacionFisica('{lnkEliminar.UniqueID}');";
                }
            }
        }

        protected void gvProveedores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleEstado")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int id = Convert.ToInt32(args[0]);
                string nuevoEstado = args[1] == "A" ? "I" : "A";

                proveedorNegocio.ActualizarEstado(id, nuevoEstado);
                MostrarMensaje($"Proveedor {(nuevoEstado == "A" ? "activado" : "desactivado")}.", "success");
                CargarProveedores();
            }
            else if (e.CommandName == "EliminarFisico")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                proveedorNegocio.EliminarFisico(id);
                MostrarMensaje("Proveedor eliminado físicamente. Sus productos quedaron sin proveedor.", "success");
                CargarProveedores();
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            PaginaActual = 1;
            CargarProveedores();
        }

        protected void btnNuevo_Click(object sender, EventArgs e) => Response.Redirect("NuevoProveedor.aspx");

        protected void btnPagAnterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                CargarProveedores();
            }
        }

        protected void btnPagSiguiente_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            CargarProveedores();
        }

        protected void btnEliminarSeleccion_Click(object sender, EventArgs e)
        {
            List<int> ids = new List<int>();
            foreach (GridViewRow row in gvProveedores.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSeleccion");
                if (chk != null && chk.Checked)
                    ids.Add(Convert.ToInt32(gvProveedores.DataKeys[row.RowIndex].Value));
            }
            if (ids.Count > 0)
            {
                foreach (int id in ids) proveedorNegocio.EliminarFisico(id);
                MostrarMensaje($"{ids.Count} proveedor(es) eliminado(s).", "success");
                CargarProveedores();
            }
        }

        protected void btnEliminarTodos_Click(object sender, EventArgs e)
        {
            proveedorNegocio.EliminarTodos();
            MostrarMensaje("Todos los proveedores han sido eliminados.", "success");
            CargarProveedores();
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            hfMsgType.Value = tipo;
            hfMsgText.Value = mensaje;
            hfRedirectUrl.Value = "";
        }
    }
}