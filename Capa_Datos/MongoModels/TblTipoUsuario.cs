using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capa_Datos.MongoModels
{
    public class TblTipoUsuario
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int tusu_id { get; set; }
        public string tusu_nombre { get; set; }
        public string tusu_estado { get; set; }     // ahora string
    }
}