using System;
using System.Web.UI.WebControls;
using Capa_Negocio;

namespace Monolito_4bm
{
    public partial class AdminUsuarios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "1")
            {
                Response.Redirect("Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarUsuarios();
            }
        }

        private void CargarUsuarios()
        {
            try
            {
                CN_Usuario negocio = new CN_Usuario();
                var usuarios = negocio.ListarTodosLosUsuarios();

                // 🚩 Depuración: escribe en la ventana de Resultados el valor de intentos_dia del primer usuario
                foreach (var u in usuarios)
                    System.Diagnostics.Debug.WriteLine($"{u.usu_nick} -> intentos_dia = {u.usu_intentos_dia}");

                gvUsuarios.DataSource = usuarios;
                gvUsuarios.DataBind();
            }
            catch (Exception ex)
            {
                hfMsgType.Value = "error";
                hfMsgText.Value = "Error al cargar usuarios: " + ex.Message;
            }
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                CN_Usuario negocio = new CN_Usuario();

                if (e.CommandName == "ToggleEstado")
                {
                    string[] args = e.CommandArgument.ToString().Split('|');
                    int usuId = Convert.ToInt32(args[0]);
                    string estadoActual = args[1];   // ahora es string, no char

                    string nuevoEstado = estadoActual == "A" ? "I" : "A";
                    string msj = negocio.ActualizarEstadoUsuario(usuId, nuevoEstado);

                    if (msj.ToLower().Contains("correctamente"))
                    {
                        hfMsgType.Value = "success";
                        hfMsgText.Value = "Estado del usuario actualizado a " + (nuevoEstado == "A" ? "Activo" : "Inactivo") + ".";
                    }
                    else
                    {
                        hfMsgType.Value = "error";
                        hfMsgText.Value = msj;
                    }
                }
                else if (e.CommandName == "Desbloquear")
                {
                    int usuId = Convert.ToInt32(e.CommandArgument);
                    string msj = negocio.DesbloquearUsuario(usuId);

                    if (msj.ToLower().Contains("exitosamente"))
                    {
                        hfMsgType.Value = "success";
                        hfMsgText.Value = "Usuario desbloqueado (intentos reseteados a 0).";
                    }
                    else
                    {
                        hfMsgType.Value = "error";
                        hfMsgText.Value = msj;
                    }
                }

                CargarUsuarios();
            }
            catch (Exception ex)
            {
                hfMsgType.Value = "error";
                hfMsgText.Value = "Error en la acción: " + ex.Message;
            }
        }
    }
}