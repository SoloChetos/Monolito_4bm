using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace Monolito_4bm
{
    [Serializable]
    public class ImagenTemporal  // Debe ser public para que Eval funcione
    {
        public int Indice { get; set; }
        public byte[] Bytes { get; set; }
        public string Nombre { get; set; }
        public string ContentType { get; set; }
        public string DataUri { get; set; }
    }

    public partial class NuevoProducto : System.Web.UI.Page
    {
        private CN_Producto productoNegocio = new CN_Producto();
        private CN_Proveedor proveedorNegocio = new CN_Proveedor();
        private const int MaxImagenes = 5;
        private const int MaxSizeMB = 5;
        private static readonly string[] ExtPermitidas = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "1")
            {
                Response.Redirect("Default.aspx");
                return;
            }

            // Inicializar lista de imágenes nuevas siempre (también en postbacks)
            if (ViewState["ImagenesNuevas"] == null)
                ViewState["ImagenesNuevas"] = new List<ImagenTemporal>();

            if (!IsPostBack)
            {
                CargarProveedores();
                ActualizarVista();
            }
        }

        private void CargarProveedores()
        {
            var lista = proveedorNegocio.ListarActivos();
            ddlProveedor.DataSource = lista;
            ddlProveedor.DataTextField = "prov_nombre";
            ddlProveedor.DataValueField = "prov_id";
            ddlProveedor.DataBind();
            ddlProveedor.Items.Insert(0, new ListItem("Sin proveedor", ""));
        }

        private List<ImagenTemporal> ObtenerImagenesNuevas()
        {
            if (ViewState["ImagenesNuevas"] == null)
                ViewState["ImagenesNuevas"] = new List<ImagenTemporal>();
            return (List<ImagenTemporal>)ViewState["ImagenesNuevas"];
        }

        private void ActualizarVista()
        {
            var nuevas = ObtenerImagenesNuevas();
            rptNuevasImagenes.DataSource = nuevas;
            rptNuevasImagenes.DataBind();
            lblSinNuevas.Visible = nuevas.Count == 0;

            // Deshabilitar el FileUpload si se alcanzó el límite
            bool limiteAlcanzado = nuevas.Count >= MaxImagenes;
            fuImagen.Enabled = !limiteAlcanzado;
            btnAgregarImagen.Enabled = !limiteAlcanzado;
            fuImagen.ToolTip = limiteAlcanzado ? "Límite de imágenes alcanzado" : "";
        }

        protected void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            if (!fuImagen.HasFiles)
            {
                MostrarMensaje("Seleccione al menos un archivo.", "warning");
                return;
            }

            var nuevas = ObtenerImagenesNuevas();
            int espacioDisponible = MaxImagenes - nuevas.Count;

            if (espacioDisponible <= 0)
            {
                MostrarMensaje($"Máximo {MaxImagenes} imágenes alcanzado.", "error");
                return;
            }
            //Validamos cada archivo seleccionado
            if (fuImagen.PostedFiles.Count > espacioDisponible)
            {
                MostrarMensaje($"Solo puedes subir {espacioDisponible} imagen(es) más. Has seleccionado {fuImagen.PostedFiles.Count}.", "warning");
                return;
            }

            int archivosAceptados = 0;

            foreach (HttpPostedFile file in fuImagen.PostedFiles)
            {
                string ext = Path.GetExtension(file.FileName).ToLower();
                if (!ExtPermitidas.Contains(ext)) continue;
                if (file.ContentLength > MaxSizeMB * 1024 * 1024) continue;

                byte[] bytes;
                using (BinaryReader br = new BinaryReader(file.InputStream))
                {
                    bytes = br.ReadBytes(file.ContentLength);
                }

                string base64 = Convert.ToBase64String(bytes);
                string dataUri = "data:" + file.ContentType + ";base64," + base64;
                int nuevoIndice = nuevas.Count > 0 ? nuevas.Max(i => i.Indice) + 1 : 1;

                nuevas.Add(new ImagenTemporal
                {
                    Indice = nuevoIndice,
                    Bytes = bytes,
                    Nombre = file.FileName,
                    ContentType = file.ContentType,
                    DataUri = dataUri
                });
                archivosAceptados++;
            }

            ViewState["ImagenesNuevas"] = nuevas;
            ActualizarVista();

            if (archivosAceptados > 0)
            {
                MostrarMensaje($"{archivosAceptados} imagen(es) previsualizada(s). Total: {nuevas.Count}/{MaxImagenes}.", "success");
            }
            else
            {
                MostrarMensaje("Ningún archivo válido (revise formato o tamaño máximo 5 MB).", "warning");
            }
        }

        protected void rptNuevasImagenes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "QuitarNueva")
            {
                int indice = Convert.ToInt32(e.CommandArgument);
                var nuevas = ObtenerImagenesNuevas();
                nuevas.RemoveAll(i => i.Indice == indice);
                ViewState["ImagenesNuevas"] = nuevas;
                ActualizarVista();
                MostrarMensaje("Imagen eliminada.", "success");
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MostrarMensaje("El nombre es obligatorio.", "warning");
                return;
            }

            try
            {
                string nombre = txtNombre.Text.Trim();
                int cantidad = string.IsNullOrEmpty(txtCantidad.Text) ? 0 : Convert.ToInt32(txtCantidad.Text);

                // Conversión robusta de precio (acepta punto o coma)
                decimal precio = 0;
                string precioStr = txtPrecio.Text.Trim().Replace(',', '.');
                if (decimal.TryParse(precioStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioTmp))
                {
                    precio = Math.Round(precioTmp, 2);
                }
                else
                {
                    MostrarMensaje("El precio no es válido. Use números y solo un punto o coma decimal.", "error");
                    return;
                }

                int? provId = string.IsNullOrEmpty(ddlProveedor.SelectedValue) ? (int?)null : Convert.ToInt32(ddlProveedor.SelectedValue);

                var nuevas = ObtenerImagenesNuevas();
                List<string> rutasNuevas = new List<string>();
                string carpeta = Server.MapPath("~/Images/Productos/");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                foreach (var imgTemp in nuevas)
                {
                    string nombreArchivo = Guid.NewGuid().ToString("N") + Path.GetExtension(imgTemp.Nombre);
                    string rutaFisica = Path.Combine(carpeta, nombreArchivo);
                    File.WriteAllBytes(rutaFisica, imgTemp.Bytes);
                    rutasNuevas.Add("Images/Productos/" + nombreArchivo);
                }

                productoNegocio.Insertar(nombre, cantidad, precio, provId, rutasNuevas);
                MostrarMensaje("Producto creado correctamente.", "success", "Productos.aspx");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al guardar: " + ex.Message, "error");
            }
        }

        private void MostrarMensaje(string mensaje, string tipo, string redirectUrl = null)
        {
            hfMsgType.Value = tipo;
            hfMsgText.Value = mensaje;
            hfRedirectUrl.Value = redirectUrl ?? "";
        }
    }
}