using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Monolito_4bm
{
    public partial class PrincipalMaster : MasterPage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx?expired=1");
                return;
            }

            if (!IsPostBack)
            {
                // --- Cargar foto de perfil o iniciales ---
                string nombreCompleto = Session["NombreUsuario"] != null ? Session["NombreUsuario"].ToString() : "Usuario";
                if (Session["FotoPerfil"] != null && !string.IsNullOrEmpty(Session["FotoPerfil"].ToString()))
                {
                    imgPerfilMaster.ImageUrl = "data:image/jpeg;base64," + Session["FotoPerfil"].ToString();
                    imgPerfilMaster.Visible = true;
                    avatarInitials.Visible = false;
                }
                else
                {
                    imgPerfilMaster.Visible = false;
                    avatarInitials.Visible = true;
                    avatarInitials.InnerText = ObtenerIniciales(nombreCompleto);
                }

                // --- Generar menú superior (navbar) según rol ---
                string rol = Session["Rol"]?.ToString() ?? "2";
                if (rol == "1") // Administrador
                {
                    // Se elimina el ítem duplicado de "Gestión de Usuarios".
                    // El menú queda limpio ya que el Dashboard maneja la navegación principal.
                    masterBody.Attributes["class"] = "rol-admin";
                }
                else
                {
                    ulMenuDinamico.Controls.Add(CrearMenuItem("Juego", "Juego.aspx", "fas fa-gamepad"));
                    masterBody.Attributes["class"] = "rol-user";
                }
            }
        }

        private string ObtenerIniciales(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto)) return "U";
            var partes = nombreCompleto.Trim().Split(' ');
            if (partes.Length == 1) return partes[0].Substring(0, 1).ToUpper();
            return (partes[0].Substring(0, 1) + partes[partes.Length - 1].Substring(0, 1)).ToUpper();
        }

        private HtmlGenericControl CrearMenuItem(string texto, string url, string icono)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            li.Attributes.Add("class", "nav-item");

            HtmlGenericControl a = new HtmlGenericControl("a");
            a.Attributes.Add("class", "nav-link");
            a.Attributes.Add("href", url);
            a.InnerHtml = $"<i class='{icono} me-1'></i> {texto}";

            // CORRECCIÓN UX: Se elimina el color blanco forzado para que herede el #555555 del CSS
            // y sea visible en el navbar minimalista.

            li.Controls.Add(a);
            return li;
        }

        protected void btnCerrarSesionMaster_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            if (Request.Cookies["UserInfo"] != null)
            {
                var cookie = new System.Web.HttpCookie("UserInfo");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            Response.Redirect("Login.aspx?logout=1");
        }
    }
}