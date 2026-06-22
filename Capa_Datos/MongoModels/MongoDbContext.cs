using MongoDB.Driver;
using Capa_Datos.MongoModels;
using System.Configuration;

namespace Capa_Datos
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext()
        {
            var client = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString);
            _database = client.GetDatabase("Monolito4bm");
        }

        public IMongoCollection<TblTipoUsuario> TiposUsuario =>
            _database.GetCollection<TblTipoUsuario>("tipos_usuario");

        public IMongoCollection<TblUsuario> Usuarios =>
            _database.GetCollection<TblUsuario>("usuarios");

        public IMongoCollection<TblOtpToken> OtpTokens =>
            _database.GetCollection<TblOtpToken>("otp_tokens");

        public IMongoCollection<TblProducto> Productos =>
            _database.GetCollection<TblProducto>("productos");

        public IMongoCollection<TblProveedor> Proveedores =>
            _database.GetCollection<TblProveedor>("proveedores");

        // Más colecciones se irán agregando...
    }
}