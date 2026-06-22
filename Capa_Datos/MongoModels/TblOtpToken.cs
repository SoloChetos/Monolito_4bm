using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Capa_Datos.MongoModels
{
    public class TblOtpToken
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int usu_id { get; set; }
        public string otp_codigo { get; set; }
        public DateTime? otp_fecha_generacion { get; set; }
        public DateTime otp_fecha_expiracion { get; set; }
        public bool otp_utilizado { get; set; }
        public int otp_intentos { get; set; }
    }
}