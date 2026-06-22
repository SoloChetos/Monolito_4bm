using System;
using System.Web.UI;

namespace Monolito_4bm
{
    public partial class SiteAdmin : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null || Session["TipoUsuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (Session["TipoUsuario"].ToString() != "1")
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                lblUsuario.Text = Session["NombreUsuario"] != null ? Session["NombreUsuario"].ToString() : "Admin";
                if (Session["FotoPerfil"] != null)
                {
                    imgPerfil.ImageUrl = "data:image/jpeg;base64," + Session["FotoPerfil"].ToString();
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
