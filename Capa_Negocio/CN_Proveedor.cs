using Capa_Datos;
using Capa_Datos.MongoModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Capa_Negocio
{
    public class CN_Proveedor
    {
        private readonly MongoDbContext _db = new MongoDbContext();

        public List<TblProveedor> ListarActivos()
        {
            return _db.Proveedores.Find(p => p.prov_estado == "A").ToList();
        }

        public List<TblProveedor> ListarTodos()
        {
            return _db.Proveedores.Find(FilterDefinition<TblProveedor>.Empty).ToList();
        }

        public TblProveedor ObtenerPorId(int id)
        {
            return _db.Proveedores.Find(p => p.prov_id == id).FirstOrDefault();
        }

        public void Insertar(string nombre)
        {
            int nuevoId = _db.Proveedores.Find(FilterDefinition<TblProveedor>.Empty)
                .SortByDescending(p => p.prov_id).FirstOrDefault()?.prov_id + 1 ?? 1;
            _db.Proveedores.InsertOne(new TblProveedor { prov_id = nuevoId, prov_nombre = nombre, prov_estado = "A" });
        }

        public void Actualizar(int id, string nombre)
        {
            _db.Proveedores.UpdateOne(p => p.prov_id == id,
                Builders<TblProveedor>.Update.Set(p => p.prov_nombre, nombre));
        }

        public void Eliminar(int id)  // lógico
        {
            _db.Proveedores.UpdateOne(p => p.prov_id == id,
                Builders<TblProveedor>.Update.Set(p => p.prov_estado, "I"));
        }

        public bool TieneProductos(int prov_id)
        {
            return _db.Productos.Find(p => p.prov_id == prov_id && p.pro_estado == "A").Any();
        }

        public void ActualizarEstado(int id, string nuevoEstado)
        {
            _db.Proveedores.UpdateOne(p => p.prov_id == id,
                Builders<TblProveedor>.Update.Set(p => p.prov_estado, nuevoEstado));
        }

        public void EliminarFisico(int id)
        {
            _db.Proveedores.DeleteOne(p => p.prov_id == id);
        }

        public void EliminarTodos()
        {
            _db.Proveedores.DeleteMany(FilterDefinition<TblProveedor>.Empty);
            // Reinicio del contador: el siguiente Insertar usará ObtenerSiguienteId() que ahora devolverá 1.
        }
        public void ImportarProveedoresMasivo(DataTable dt, bool eliminarNoIncluidos, bool actualizarExistentes, bool soloNuevos = false)
        {
            List<int> idsProcesados = new List<int>();

            // ── CASO: SOLO NUEVOS ──────────────────────────────────────────
            if (soloNuevos)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string nombre = row["prov_nombre"]?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(nombre)) continue;
                    Insertar(nombre);   // Insertar ya genera un nuevo ID
                                        // No necesitamos el ID para este caso
                }
                return;
            }

            // ── BORRADO TOTAL (ambas casillas desmarcadas) ────────────────
            if (!eliminarNoIncluidos && !actualizarExistentes)
            {
                EliminarTodos();   // este método ahora sí reinicia el contador
            }

            // ── PROCESAR CADA FILA ────────────────────────────────────────
            foreach (DataRow row in dt.Rows)
            {
                int provId = 0;
                int.TryParse(row["prov_id"]?.ToString(), out provId);
                string nombre = row["prov_nombre"]?.ToString() ?? "";

                if (string.IsNullOrWhiteSpace(nombre)) continue;

                if (provId > 0 && actualizarExistentes)
                {
                    Actualizar(provId, nombre);
                    idsProcesados.Add(provId);
                }
                else
                {
                    Insertar(nombre);
                    // Insertar no devuelve el nuevo ID, pero podemos obtenerlo buscando el máximo
                    int nuevoId = _db.Proveedores.Find(FilterDefinition<TblProveedor>.Empty)
                        .SortByDescending(p => p.prov_id)
                        .FirstOrDefault()?.prov_id ?? 0;
                    if (nuevoId > 0) idsProcesados.Add(nuevoId);
                }
            }

            // ── ELIMINAR NO INCLUIDOS ──────────────────────────────────────
            if (eliminarNoIncluidos && idsProcesados.Count > 0)
            {
                var filtro = Builders<TblProveedor>.Filter.Nin(p => p.prov_id, idsProcesados);
                _db.Proveedores.DeleteMany(filtro);
            }
        }
    }
}