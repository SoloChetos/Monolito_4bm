using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Negocio;

namespace Monolito_4bm
{
    public partial class AdminPanel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarUsuariosBloqueados();
        }

        private void CargarUsuariosBloqueados()
        {
            try
            {
                CN_Usuario negocio = new CN_Usuario();
                var lista = negocio.ListarUsuariosBloqueados();
                gvUsuarios.DataSource = lista;
                gvUsuarios.DataBind();
            }
            catch (Exception ex)
            {
                hfMsg.Value = "Error al cargar: " + ex.Message;
            }
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Desbloquear")
            {
                int usu_id = int.Parse(e.CommandArgument.ToString());
                CN_Usuario negocio = new CN_Usuario();
                string resultado = negocio.DesbloquearUsuario(usu_id);
                hfMsg.Value = resultado;
                CargarUsuariosBloqueados();
            }
        }

        protected void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarUsuariosBloqueados();
        }
    }
}
