using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Capa_Datos.MongoModels
{
    public class TblUsuario
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int usu_id { get; set; }
        public string usu_cedula { get; set; }
        public string usu_nombres { get; set; }
        public string usu_apellidos { get; set; }
        public string usu_dirreccion { get; set; }
        public string usu_celular { get; set; }
        public string usu_correo { get; set; }
        public DateTime usu_fecha_creacion { get; set; }
        public DateTime? usu_fecha_cumple { get; set; }
        public string usu_nick { get; set; }
        public byte[] usu_contrasena { get; set; }
        public int usu_intentos { get; set; }
        public string usu_codigo_OTP { get; set; }
        public string usu_estado { get; set; }        // ahora string
        public int? tusu_id { get; set; }
        public DateTime? usu_fecha_ult_intento { get; set; }
        public int usu_intentos_dia { get; set; }
        public byte[] usu_foto { get; set; }
        public string usu_clave_temporal { get; set; }
        public DateTime? usu_fecha_clave_temp { get; set; }
        public string usu_otp_secret { get; set; }
        public string usu_recordarme_token { get; set; }
        public DateTime? usu_recordarme_expiracion { get; set; }
    }
}