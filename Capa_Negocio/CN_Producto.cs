using Capa_Datos;
using Capa_Datos.MongoModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace Capa_Negocio
{
    public class CN_Producto
    {
        private readonly MongoDbContext _db = new MongoDbContext();

        // ── Buscar todos los productos (sin paginación) ──────────────────
        public DataTable BuscarProductosTodos(string nombre, int? provId, decimal? precioMin, decimal? precioMax, string orden)
        {
            var filtro = Builders<TblProducto>.Filter.Empty;

            if (!string.IsNullOrEmpty(nombre))
                filtro = Builders<TblProducto>.Filter.Where(p => p.pro_nombre.ToLower().Contains(nombre.ToLower()));
            if (provId.HasValue && provId.Value == -1)
                filtro &= Builders<TblProducto>.Filter.Where(p => p.prov_id == null);
            else if (provId.HasValue)
                filtro &= Builders<TblProducto>.Filter.Where(p => p.prov_id == provId.Value);
            if (precioMin.HasValue)
                filtro &= Builders<TblProducto>.Filter.Where(p => p.pro_precio >= precioMin.Value);
            if (precioMax.HasValue)
                filtro &= Builders<TblProducto>.Filter.Where(p => p.pro_precio <= precioMax.Value);

            var productos = _db.Productos.Find(filtro).ToList();
            var proveedores = _db.Proveedores.Find(FilterDefinition<TblProveedor>.Empty).ToList();

            var query = from p in productos
                        join pr in proveedores on p.prov_id equals pr.prov_id into gj
                        from subpr in gj.DefaultIfEmpty()
                        select new
                        {
                            p.pro_id,
                            p.pro_nombre,
                            p.pro_cantidad,
                            p.pro_precio,
                            p.pro_estado,
                            p.prov_id,
                            prov_nombre = subpr?.prov_nombre ?? "Sin proveedor",
                            pro_imagen_principal = p.imagenes != null && p.imagenes.Any()
                                ? p.imagenes.OrderBy(i => i.img_orden).First().img_ruta
                                : null
                        };

            switch (orden)
            {
                case "precio_asc": query = query.OrderBy(x => x.pro_precio); break;
                case "precio_desc": query = query.OrderByDescending(x => x.pro_precio); break;
                case "antiguos": query = query.OrderBy(x => x.pro_id); break;
                default: query = query.OrderByDescending(x => x.pro_id); break;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("pro_id", typeof(int));
            dt.Columns.Add("pro_nombre", typeof(string));
            dt.Columns.Add("pro_cantidad", typeof(int));
            dt.Columns.Add("pro_precio", typeof(decimal));
            dt.Columns.Add("pro_estado", typeof(string));
            dt.Columns.Add("prov_id", typeof(int));
            dt.Columns.Add("prov_nombre", typeof(string));
            dt.Columns.Add("pro_imagen_principal", typeof(string));

            foreach (var item in query)
            {
                DataRow row = dt.NewRow();
                row["pro_id"] = item.pro_id;
                row["pro_nombre"] = item.pro_nombre;
                row["pro_cantidad"] = item.pro_cantidad;
                row["pro_precio"] = item.pro_precio;
                row["pro_estado"] = item.pro_estado;
                row["prov_id"] = (object)item.prov_id ?? DBNull.Value;
                row["prov_nombre"] = item.prov_nombre;
                row["pro_imagen_principal"] = (object)item.pro_imagen_principal ?? DBNull.Value;
                dt.Rows.Add(row);
            }

            return dt;
        }

        // ── Obtener producto por ID ──────────────────────────────────────
        public TblProducto ObtenerProductoPorId(int pro_id)
        {
            return _db.Productos.Find(p => p.pro_id == pro_id).FirstOrDefault();
        }

        // ── Obtener imágenes (para Repeater de edición) ──────────────────
        public List<ProductoImagen> ObtenerImagenes(int pro_id)
        {
            var prod = ObtenerProductoPorId(pro_id);
            return prod?.imagenes ?? new List<ProductoImagen>();
        }

        // ── Guardar imagen en disco (devuelve ruta relativa) ─────────────
        public string GuardarImagen(HttpPostedFile file, string carpetaFisica)
        {
            string nombreUnico = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
            string rutaFisica = Path.Combine(carpetaFisica, nombreUnico);
            file.SaveAs(rutaFisica);
            return "Images/Productos/" + nombreUnico;
        }

        public string GuardarImagenDesdeStream(Stream stream, string nombreArchivo, string carpetaFisica)
        {
            string nombreUnico = Guid.NewGuid().ToString("N") + Path.GetExtension(nombreArchivo);
            string rutaFisica = Path.Combine(carpetaFisica, nombreUnico);
            using (var fileStream = new FileStream(rutaFisica, FileMode.Create))
                stream.CopyTo(fileStream);
            return "Images/Productos/" + nombreUnico;
        }

        // ── Eliminar imagen física del disco ─────────────────────────────
        public void EliminarImagenFisica(string rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa)) return;
            string rutaAbsoluta = HttpContext.Current.Server.MapPath("~/" + rutaRelativa);
            if (File.Exists(rutaAbsoluta)) File.Delete(rutaAbsoluta);
        }

        // ── Insertar producto con imágenes ───────────────────────────────
        public int Insertar(string nombre, int cantidad, decimal precio, int? provId, List<string> rutasImagenes)
        {
            int nuevoId = ObtenerSiguienteId();
            var producto = new TblProducto
            {
                pro_id = nuevoId,
                pro_nombre = nombre,
                pro_cantidad = cantidad,
                pro_precio = precio,
                pro_estado = "A",
                prov_id = provId,
                imagenes = rutasImagenes.Select((ruta, idx) => new ProductoImagen { img_ruta = ruta, img_orden = idx }).ToList()
            };
            _db.Productos.InsertOne(producto);
            return nuevoId;
        }

        private int ObtenerSiguienteId()
        {
            var ultimo = _db.Productos.Find(FilterDefinition<TblProducto>.Empty)
                .SortByDescending(p => p.pro_id)
                .FirstOrDefault();
            return ultimo == null ? 1 : ultimo.pro_id + 1;
        }

        // ── Actualizar producto (y opcionalmente reemplazar imágenes) ────
        public void Actualizar(int proId, string nombre, int cantidad, decimal precio, int? provId, List<string> nuevasRutas = null)
        {
            var update = Builders<TblProducto>.Update
                .Set(p => p.pro_nombre, nombre)
                .Set(p => p.pro_cantidad, cantidad)
                .Set(p => p.pro_precio, precio)
                .Set(p => p.prov_id, provId);

            if (nuevasRutas != null && nuevasRutas.Count > 0)
            {
                var imagenes = nuevasRutas.Select((ruta, idx) => new ProductoImagen { img_ruta = ruta, img_orden = idx }).ToList();
                update = update.Set(p => p.imagenes, imagenes);
            }

            _db.Productos.UpdateOne(p => p.pro_id == proId, update);
        }

        // ── Eliminar lógico ──────────────────────────────────────────────
        public void EliminarLogico(int proId)
        {
            _db.Productos.UpdateOne(p => p.pro_id == proId,
                Builders<TblProducto>.Update.Set(p => p.pro_estado, "I"));
        }

        // ── Eliminar físico (producto + imágenes en disco) ────────────────
        public void EliminarFisico(int proId)
        {
            var prod = ObtenerProductoPorId(proId);
            if (prod?.imagenes != null)
                foreach (var img in prod.imagenes)
                    EliminarImagenFisica(img.img_ruta);

            _db.Productos.DeleteOne(p => p.pro_id == proId);
        }

        // ── Eliminar varios ──────────────────────────────────────────────
        public void EliminarSeleccionados(List<int> ids)
        {
            foreach (int id in ids) EliminarFisico(id);
        }

        // ── Eliminar todos ───────────────────────────────────────────────
        public void EliminarTodos()
        {
            var todos = _db.Productos.Find(FilterDefinition<TblProducto>.Empty).ToList();
            foreach (var prod in todos)
                if (prod.imagenes != null)
                    foreach (var img in prod.imagenes)
                        EliminarImagenFisica(img.img_ruta);

            _db.Productos.DeleteMany(FilterDefinition<TblProducto>.Empty);
        }

        public void ActualizarCompleto(int proId, string nombre, int cantidad, decimal precio, int? provId, List<ProductoImagen> imagenes)
        {
            var update = Builders<TblProducto>.Update
                .Set(p => p.pro_nombre, nombre)
                .Set(p => p.pro_cantidad, cantidad)
                .Set(p => p.pro_precio, precio)
                .Set(p => p.prov_id, provId)
                .Set(p => p.imagenes, imagenes);
            _db.Productos.UpdateOne(p => p.pro_id == proId, update);
        }

        // ── Importación masiva desde Excel ───────────────────────────────
        public void ImportarProductosMasivo(DataTable dt, bool eliminarNoIncluidos, bool actualizarExistentes, bool soloNuevos = false)
        {
            List<int> idsProcesados = new List<int>();

            // ── CASO: SOLO NUEVOS ──────────────────────────────────────────
            if (soloNuevos)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string nombre = row["pro_nombre"]?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(nombre)) continue;

                    int cantidad = 0;
                    int.TryParse(row["pro_cantidad"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out cantidad);
                    decimal precio = 0;
                    decimal.TryParse(row["pro_precio"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out precio);
                    int? provId = null;
                    if (int.TryParse(row["prov_id"]?.ToString(), out int pId) && pId > 0) provId = pId;
                    string imgRutas = row["img_ruta"]?.ToString() ?? "";

                    List<string> rutas = imgRutas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => r.Trim()).ToList();
                    int nuevoId = Insertar(nombre, cantidad, precio, provId, rutas);
                    idsProcesados.Add(nuevoId);
                }
                // No se elimina nada porque es "solo nuevos"
                return;
            }

            // ── BORRADO TOTAL (ambas casillas desmarcadas) ────────────────
            if (!eliminarNoIncluidos && !actualizarExistentes)
            {
                // Borrar todas las imágenes físicas
                var todos = _db.Productos.Find(FilterDefinition<TblProducto>.Empty).ToList();
                foreach (var prod in todos)
                    if (prod.imagenes != null)
                        foreach (var img in prod.imagenes)
                            EliminarImagenFisica(img.img_ruta);

                // Borrar todos los documentos de la colección
                _db.Productos.DeleteMany(FilterDefinition<TblProducto>.Empty);
                // El contador de IDs se reinicia automáticamente porque ObtenerSiguienteId() buscará el máximo ID y devolverá 1 si no hay documentos.
            }

            // ── PROCESAR CADA FILA ────────────────────────────────────────
            foreach (DataRow row in dt.Rows)
            {
                int proId = 0;
                int.TryParse(row["pro_id"]?.ToString(), out proId);
                string nombre = row["pro_nombre"]?.ToString() ?? "";
                int cantidad = 0;
                int.TryParse(row["pro_cantidad"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out cantidad);
                decimal precio = 0;
                decimal.TryParse(row["pro_precio"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out precio);
                int? provId = null;
                if (int.TryParse(row["prov_id"]?.ToString(), out int pid) && pid > 0) provId = pid;
                string imgRutas = row["img_ruta"]?.ToString() ?? "";

                List<string> rutas = imgRutas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim()).ToList();

                if (proId > 0 && actualizarExistentes)
                {
                    Actualizar(proId, nombre, cantidad, precio, provId, rutas.Count > 0 ? rutas : null);
                    idsProcesados.Add(proId);
                }
                else
                {
                    int nuevoId = Insertar(nombre, cantidad, precio, provId, rutas);
                    idsProcesados.Add(nuevoId);
                }
            }

            // ── ELIMINAR NO INCLUIDOS ──────────────────────────────────────
            if (eliminarNoIncluidos && idsProcesados.Count > 0)
            {
                var filtro = Builders<TblProducto>.Filter.Nin(p => p.pro_id, idsProcesados);
                _db.Productos.DeleteMany(filtro);
            }
        }
        public void CambiarEstadoProducto(int proId, string nuevoEstado)
        {
            _db.Productos.UpdateOne(p => p.pro_id == proId,
                Builders<TblProducto>.Update.Set(p => p.pro_estado, nuevoEstado));
        }

        // ── Listar activos (para dropdowns) ──────────────────────────────
        public List<TblProducto> ListarActivos()
        {
            return _db.Productos.Find(p => p.pro_estado == "A").ToList();
        }
    }
}