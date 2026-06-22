using System;

namespace Monolito_4bm
{
    public partial class UserPanel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                lblNombre.Text = Session["NombreUsuario"] != null ? Session["NombreUsuario"].ToString() : Session["Usuario"].ToString();
                if (Session["FotoPerfil"] != null)
                {
                    imgPerfilGrande.ImageUrl = "data:image/jpeg;base64," + Session["FotoPerfil"].ToString();
                }
                else
                {
                    imgPerfilGrande.Visible = false;
                }
            }
        }
    }
}
