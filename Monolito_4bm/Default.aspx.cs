using System;
using System.Web;
using System.Web.UI;

namespace Monolito_4bm
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            // Prevención de almacenamiento en caché por seguridad (evita "Back" tras logout)
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Validar sesión activa
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDashboard();
            }
        }

        private void CargarDashboard()
        {
            // Asignar nombre del usuario al encabezado
            lblNombre.Text = Session["NombreUsuario"]?.ToString() ?? "Usuario";

            // Validar el nivel de acceso (Asume 2 como fallback por seguridad)
            int rol = Convert.ToInt32(Session["Rol"] ?? 2);

            // Control de visibilidad de paneles basado en el rol
            bool esAdministrador = (rol == 1);

            pnlAdmin.Visible = esAdministrador;
            pnlUsuario.Visible = !esAdministrador;
        }
    }
}