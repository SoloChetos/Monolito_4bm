using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capa_Datos.MongoModels
{
    public class TblProveedor
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int prov_id { get; set; }
        public string prov_nombre { get; set; }
        public string prov_estado { get; set; }     // "A" o "I"
    }
}