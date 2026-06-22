using Capa_Negocio;
using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;


namespace Monolito_4bm
{
    public partial class ImportarProveedores : System.Web.UI.Page
    {
        private CN_Proveedor proveedorNegocio = new CN_Proveedor();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "1")
            {
                Response.Redirect("Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                Session.Remove("DatosExcel");
            }
        }

        protected void btnDescargarPlantilla_Click(object sender, EventArgs e)
        {
            string plantilla = "prov_id;prov_nombre\n;\n";
            byte[] bytes = new UTF8Encoding(true).GetBytes(plantilla);
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("content-disposition", "attachment; filename=Plantilla_Proveedores.csv");
            Response.BinaryWrite(bytes);
            Response.End();
        }

        protected void btnCargar_Click(object sender, EventArgs e)
        {
            if (!fuArchivo.HasFile)
            {
                MostrarSweetAlert("Debe seleccionar un archivo válido.", "warning");
                return;
            }

            try
            {
                DataTable dt = ExtraerDatosDeExcel(fuArchivo);

                if (dt == null || dt.Rows.Count == 0)
                {
                    MostrarSweetAlert("El archivo está vacío o no tiene el formato correcto.", "error");
                    return;
                }

                if (!dt.Columns.Contains("prov_nombre"))
                {
                    MostrarSweetAlert("El archivo debe contener la columna: prov_nombre.", "error");
                    return;
                }

                // Agregar columna de estado
                dt.Columns.Add("Estado", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    int provId = 0;
                    if (row["prov_id"] != DBNull.Value && int.TryParse(row["prov_id"].ToString(), out provId) && provId > 0)
                    {
                        var prov = proveedorNegocio.ObtenerPorId(provId);
                        row["Estado"] = prov != null ? "Actualizar" : "Nuevo";
                    }
                    else
                    {
                        row["Estado"] = "Nuevo";
                    }
                }

                Session["DatosExcel"] = dt;
                gvPreview.DataSource = dt;
                gvPreview.DataBind();
                pnlCarga.Visible = false;
                pnlPreview.Visible = true;
            }
            catch (Exception ex)
            {
                MostrarSweetAlert("Error al leer el archivo: " + ex.Message, "error");
            }
        }

        protected void btnCancelarPreview_Click(object sender, EventArgs e)
        {
            Session.Remove("DatosExcel");
            pnlPreview.Visible = false;
            pnlCarga.Visible = true;
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)Session["DatosExcel"];
            if (dt == null)
            {
                MostrarSweetAlert("La sesión expiró. Intente de nuevo.", "warning");
                pnlPreview.Visible = false;
                pnlCarga.Visible = true;
                return;
            }

            bool soloNuevos = chkSoloNuevos.Checked;
            bool actualizar = chkActualizarExistentes.Checked;
            bool eliminar = chkEliminarNoIncluidos.Checked;

            try
            {
                proveedorNegocio.ImportarProveedoresMasivo(dt, eliminar, actualizar, soloNuevos);

                Session.Remove("DatosExcel");
                pnlPreview.Visible = false;
                pnlCarga.Visible = true;
                MostrarSweetAlert("Proveedores importados correctamente.", "success", "Proveedores.aspx");
            }
            catch (Exception ex)
            {
                MostrarSweetAlert("Error durante la importación: " + ex.Message, "error");
            }
        }

        private DataTable ExtraerDatosDeExcel(System.Web.UI.WebControls.FileUpload archivo)
        {
            string extension = Path.GetExtension(archivo.FileName).ToLower();
            if (extension == ".csv")
                return LeerCSV(archivo.PostedFile.InputStream);
            else
                return LeerExcel(archivo.PostedFile.InputStream);
        }

        private DataTable LeerCSV(Stream stream)
        {
            DataTable dt = new DataTable();
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                fileBytes = ms.ToArray();
            }
            string content = Encoding.UTF8.GetString(fileBytes);
            if (content.Contains('\ufffd')) // carácter de reemplazo: codificación incorrecta
            {
                content = Encoding.GetEncoding(1252).GetString(fileBytes);
            }
            using (StringReader reader = new StringReader(content))
            {
                string headerLine = reader.ReadLine();
                if (string.IsNullOrEmpty(headerLine))
                    throw new Exception("El archivo CSV está vacío.");

                string[] headers = headerLine.Split(';');
                foreach (string header in headers)
                    dt.Columns.Add(header.Trim());

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] values = line.Split(';');
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < headers.Length && i < values.Length; i++)
                        row[i] = values[i].Trim();
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        private DataTable LeerExcel(Stream stream)
        {
            ExcelPackage.License.SetNonCommercialOrganization("Monolito4bm");
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                DataTable dt = new DataTable();

                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    string header = worksheet.Cells[1, col].Text.Trim();
                    if (!string.IsNullOrEmpty(header))
                        dt.Columns.Add(header);
                }

                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    DataRow dr = dt.NewRow();
                    for (int col = 1; col <= dt.Columns.Count; col++)
                    {
                        dr[col - 1] = worksheet.Cells[row, col].Text;
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
        }

        private void MostrarSweetAlert(string mensaje, string tipo, string url = "")
        {
            hfMsgType.Value = tipo;
            hfMsgText.Value = mensaje;
            hfRedirectUrl.Value = url;
        }
    }
}