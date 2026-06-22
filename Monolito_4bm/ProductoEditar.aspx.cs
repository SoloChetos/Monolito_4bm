using Capa_Negocio;
using Capa_Datos.MongoModels;
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
    public class ImagenTemporalEditar
    {
        public int Indice { get; set; }
        public byte[] Bytes { get; set; }
        public string Nombre { get; set; }
        public string ContentType { get; set; }
        public string DataUri { get; set; }
    }

    public partial class ProductoEditar : System.Web.UI.Page
    {
        protected global::System.Web.UI.WebControls.Label lblSinImagenes;
        private CN_Producto productoNegocio = new CN_Producto();
        private CN_Proveedor proveedorNegocio = new CN_Proveedor();
        private int productoId = 0;
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

            if (ViewState["ImagenesNuevas"] == null)
                ViewState["ImagenesNuevas"] = new List<ImagenTemporalEditar>();
            if (ViewState["ImagenesAEliminar"] == null)
                ViewState["ImagenesAEliminar"] = new List<int>();

            if (!IsPostBack)
            {
                if (Request.QueryString["id"] == null || !int.TryParse(Request.QueryString["id"], out productoId))
                {
                    Response.Redirect("Productos.aspx");
                    return;
                }

                hfProducId.Value = productoId.ToString();
                CargarProveedores();
                CargarDatosProducto();
                ActualizarVista();
            }
            else
            {
                productoId = int.Parse(hfProducId.Value);
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

        private void CargarDatosProducto()
        {
            var prod = productoNegocio.ObtenerProductoPorId(productoId);
            if (prod == null)
            {
                MostrarMensaje("Producto no encontrado.", "error");
                return;
            }
            txtNombre.Text = prod.pro_nombre;
            txtCantidad.Text = prod.pro_cantidad.ToString();

            // pro_precio ahora es decimal, no decimal?
            txtPrecio.Text = prod.pro_precio.ToString("F2", CultureInfo.InvariantCulture);

            if (prod.prov_id.HasValue)
                ddlProveedor.SelectedValue = prod.prov_id.Value.ToString();
        }

        private List<ImagenTemporalEditar> ObtenerImagenesNuevas()
        {
            if (ViewState["ImagenesNuevas"] == null)
                ViewState["ImagenesNuevas"] = new List<ImagenTemporalEditar>();
            return (List<ImagenTemporalEditar>)ViewState["ImagenesNuevas"];
        }

        private void ActualizarVista()
        {
            var aEliminar = (List<int>)ViewState["ImagenesAEliminar"];
            var nuevas = (List<ImagenTemporalEditar>)ViewState["ImagenesNuevas"];

            var imagenesBD = productoNegocio.ObtenerImagenes(productoId);
            // Las imágenes ya no tienen img_id, usamos el índice como identificador temporal
            var existentes = imagenesBD.Where(img => !aEliminar.Contains(imagenesBD.IndexOf(img))).ToList();

            rptImagenesExistentes.DataSource = existentes;
            rptImagenesExistentes.DataBind();
            lblSinImagenes.Visible = existentes.Count == 0;

            rptNuevasImagenes.DataSource = nuevas;
            rptNuevasImagenes.DataBind();
            lblSinNuevas.Visible = nuevas.Count == 0;

            int totalActuales = existentes.Count + nuevas.Count;
            bool limiteAlcanzado = totalActuales >= MaxImagenes;

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
            var aEliminar = (List<int>)ViewState["ImagenesAEliminar"];
            int cantidadRealExistentes = productoNegocio.ObtenerImagenes(productoId).Count - aEliminar.Count;
            int espacioDisponible = MaxImagenes - (nuevas.Count + cantidadRealExistentes);

            if (espacioDisponible <= 0)
            {
                MostrarMensaje($"Ya has alcanzado el límite de {MaxImagenes} imágenes.", "error");
                return;
            }

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

                nuevas.Add(new ImagenTemporalEditar
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
                int totalDespuesDeSubir = cantidadRealExistentes + nuevas.Count;
                string msjExito = $"{archivosAceptados} imagen(es) previsualizada(s).\n\nProgreso: {totalDespuesDeSubir} de {MaxImagenes} imágenes subidas.";
                MostrarMensaje(msjExito, "success");
            }
            else
            {
                MostrarMensaje("Ningún archivo válido para agregar (revise formato o peso).", "warning");
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
            }
        }

        protected void rptImagenesExistentes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EliminarExistente")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                var aEliminar = (List<int>)ViewState["ImagenesAEliminar"];

                if (!aEliminar.Contains(index))
                {
                    aEliminar.Add(index);
                }

                ViewState["ImagenesAEliminar"] = aEliminar;
                ActualizarVista();
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
                var aEliminar = (List<int>)ViewState["ImagenesAEliminar"];
                var nuevas = ObtenerImagenesNuevas();
                var prod = productoNegocio.ObtenerProductoPorId(productoId);
                if (prod == null)
                {
                    MostrarMensaje("Producto no encontrado.", "error");
                    return;
                }

                // Preparar la lista final de imágenes: conservamos las que no se marcaron para eliminar + las nuevas
                List<ProductoImagen> imagenesFinales = new List<ProductoImagen>();
                var existentes = prod.imagenes ?? new List<ProductoImagen>();
                for (int i = 0; i < existentes.Count; i++)
                {
                    if (!aEliminar.Contains(i))
                        imagenesFinales.Add(existentes[i]);
                    else
                    {
                        // Eliminar físicamente la imagen marcada
                        productoNegocio.EliminarImagenFisica(existentes[i].img_ruta);
                    }
                }

                // Agregar las nuevas rutas
                string carpeta = Server.MapPath("~/Images/Productos/");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                foreach (var imgTemp in nuevas)
                {
                    string nombreArchivo = Guid.NewGuid().ToString("N") + Path.GetExtension(imgTemp.Nombre);
                    string rutaFisica = Path.Combine(carpeta, nombreArchivo);
                    File.WriteAllBytes(rutaFisica, imgTemp.Bytes);
                    string rutaRelativa = "Images/Productos/" + nombreArchivo;
                    imagenesFinales.Add(new ProductoImagen { img_ruta = rutaRelativa, img_orden = imagenesFinales.Count });
                }

                // Datos del producto
                string nombre = txtNombre.Text.Trim();
                int cantidad = string.IsNullOrEmpty(txtCantidad.Text) ? 0 : Convert.ToInt32(txtCantidad.Text);
                decimal precio = 0;
                string precioStr = txtPrecio.Text.Trim().Replace(',', '.');
                if (!decimal.TryParse(precioStr, NumberStyles.Any, CultureInfo.InvariantCulture, out precio))
                {
                    MostrarMensaje("El precio no es válido.", "error");
                    return;
                }
                int? provId = string.IsNullOrEmpty(ddlProveedor.SelectedValue) ? (int?)null : Convert.ToInt32(ddlProveedor.SelectedValue);

                // Actualizar en MongoDB
                productoNegocio.ActualizarCompleto(productoId, nombre, cantidad, precio, provId, imagenesFinales);

                ViewState["ImagenesNuevas"] = new List<ImagenTemporalEditar>();
                ViewState["ImagenesAEliminar"] = new List<int>();

                MostrarMensaje("Producto actualizado correctamente.", "success", "Productos.aspx");
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