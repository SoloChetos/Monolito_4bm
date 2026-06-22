using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Capa_Datos.MongoModels
{
    public class TblProducto
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int pro_id { get; set; }
        public string pro_nombre { get; set; }
        public int pro_cantidad { get; set; }
        public decimal pro_precio { get; set; }
        public string pro_estado { get; set; }      // "A" o "I"
        public int? prov_id { get; set; }
        public List<ProductoImagen> imagenes { get; set; } = new List<ProductoImagen>();
    }

    public class ProductoImagen
    {
        public string img_ruta { get; set; }
        public int img_orden { get; set; }
    }
}