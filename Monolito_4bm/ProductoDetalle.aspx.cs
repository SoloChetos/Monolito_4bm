using Capa_Negocio;
using Capa_Datos.MongoModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4bm
{
    public partial class ProductoDetalle : System.Web.UI.Page
    {
        private CN_Producto productoNegocio = new CN_Producto();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "1")
            {
                Response.Redirect("Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int proId))
                {
                    hfProId.Value = proId.ToString();
                    CargarDetalle(proId);
                }
                else
                {
                    Response.Redirect("Productos.aspx");
                }
            }
        }

        private void CargarDetalle(int proId)
        {
            var producto = productoNegocio.ObtenerProductoPorId(proId);
            if (producto == null)
            {
                Response.Redirect("Productos.aspx");
                return;
            }

            // Datos básicos (pro_precio y pro_cantidad ya no son nullables)
            ltlNombre.Text = producto.pro_nombre;
            ltlStock.Text = producto.pro_cantidad.ToString();
            decimal precio = producto.pro_precio;
            ltlPrecio.Text = string.Format("{0:C2}", precio);
            decimal precioAntiguo = precio * 1.3m;
            ltlPrecioAntiguo.Text = string.Format("{0:C2}", precioAntiguo);
            decimal cuota = precio / 6;
            ltlCuota.Text = string.Format("{0:C2}", cuota);

            hfProductName.Value = producto.pro_nombre;
            hfProductPrice.Value = precio.ToString(CultureInfo.InvariantCulture);

            // Imágenes: carrusel y miniaturas (lista de ProductoImagen)
            var imagenes = producto.imagenes ?? new List<ProductoImagen>();
            if (imagenes.Count > 0)
            {
                rptCarouselItems.DataSource = imagenes;
                rptCarouselItems.DataBind();
                rptThumbnails.DataSource = imagenes;
                rptThumbnails.DataBind();
            }
            else
            {
                var placeholderList = new List<ProductoImagen>
                {
                    new ProductoImagen { img_ruta = "Images/Productos/placeholder.png" }
                };
                rptCarouselItems.DataSource = placeholderList;
                rptCarouselItems.DataBind();
                rptThumbnails.DataSource = new List<ProductoImagen>();
                rptThumbnails.DataBind();
            }

            // Gráfico (simulado)
            int stockActual = producto.pro_cantidad;
            var labels = new[] { "Hace 4d", "Hace 3d", "Hace 2d", "Ayer", "Hoy" };
            var data = new int[]
            {
                Math.Max(0, stockActual - 12),
                Math.Max(0, stockActual - 8),
                Math.Max(0, stockActual - 3),
                Math.Max(0, stockActual - 1),
                stockActual
            };
            string labelsJson = string.Join(",", labels.Select(l => $"'{l}'"));
            string dataJson = string.Join(",", data);

            string chartScript = $@"
                <script>
                    window.addEventListener('load', function() {{
                        const ctx = document.getElementById('stockChart').getContext('2d');
                        new Chart(ctx, {{
                            type: 'line',
                            data: {{
                                labels: [{labelsJson}],
                                datasets: [{{
                                    label: 'Cantidad en stock',
                                    data: [{dataJson}],
                                    backgroundColor: 'rgba(15, 52, 96, 0.1)',
                                    borderColor: '#0f3460',
                                    borderWidth: 3,
                                    fill: true,
                                    tension: 0.3
                                }}]
                            }},
                            options: {{
                                responsive: true,
                                maintainAspectRatio: true,
                                plugins: {{
                                    legend: {{ position: 'top' }}
                                }}
                            }}
                        }});
                    }});
                </script>";
            ltlChartScript.Text = chartScript;
        }

        protected void btnWhatsApp_Click(object sender, EventArgs e)
        {
            string nombre = hfProductName.Value;
            decimal precio = Convert.ToDecimal(hfProductPrice.Value, CultureInfo.InvariantCulture);
            string mensaje = $"Hola, me interesa el producto: {nombre} - Precio: {precio:C2}";
            string url = $"https://wa.me/?text={Uri.EscapeDataString(mensaje)}";
            Response.Redirect(url);
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("Productos.aspx");
        }
    }
}