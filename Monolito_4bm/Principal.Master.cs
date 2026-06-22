using System;

namespace Monolito_4bm
{
    public partial class Principal : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ADMIN
                if (Session["admin"] != null)
                {
                    lbl_nse.Text = Session["admin"].ToString();

                    pnl_admin.Visible = true;
                    pnl_usu.Visible = false;
                }

                // USUARIO
                else if (Session["usu"] != null)
                {
                    lbl_nse.Text = Session["usu"].ToString();

                    pnl_admin.Visible = false;
                    pnl_usu.Visible = true;
                }

                // NO LOGIN
                else
                {
                    Response.Redirect("/Seguridad/Login.aspx");
                }
            }
        }

        protected void btn_cerrar_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("/Seguridad/Login.aspx");
        }
    }
}