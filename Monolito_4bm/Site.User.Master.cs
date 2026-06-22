using System;
using System.Web.UI;

namespace Monolito_4bm
{
    public partial class SiteUser : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null || Session["TipoUsuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                lblUsuario.Text = Session["NombreUsuario"] != null ? Session["NombreUsuario"].ToString() : "Usuario";
                if (Session["FotoPerfil"] != null)
                {
                    imgPerfil.ImageUrl = "data:image/jpeg;base64," + Session["FotoPerfil"].ToString();
                }
                else
                {
                    imgPerfil.Visible = false;
                }
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}
