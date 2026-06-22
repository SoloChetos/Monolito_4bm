using Capa_Datos;
using Capa_Datos.MongoModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capa_Negocio
{
    public class CN_Usuario
    {
        private readonly MongoDbContext _db = new MongoDbContext();

        // ─────────────────────────── LOGIN ───────────────────────────
        public string IniciarSesion(string nick, string password)
        {
            var usuario = _db.Usuarios.Find(u => u.usu_nick == nick).FirstOrDefault();
            if (usuario == null)
                return "Usuario no existe.";

            if (usuario.usu_estado != "A")
                return "Usuario bloqueado.";

            try
            {
                string clave = EncryptionHelper.Decrypt(usuario.usu_contrasena);
                if (clave == password)
                {
                    var update = Builders<TblUsuario>.Update
                        .Set(u => u.usu_intentos, 0)
                        .Set(u => u.usu_intentos_dia, 0)
                        .Set(u => u.usu_fecha_ult_intento, null);
                    _db.Usuarios.UpdateOne(u => u.usu_id == usuario.usu_id, update);
                    return "Login exitoso.";
                }
                else
                {
                    // Calcular intentos del día actual
                    int intentosHoy = usuario.usu_fecha_ult_intento?.Date == DateTime.Today
                        ? usuario.usu_intentos_dia + 1
                        : 1;

                    var update = Builders<TblUsuario>.Update
                        .Set(u => u.usu_intentos, usuario.usu_intentos + 1)
                        .Set(u => u.usu_intentos_dia, intentosHoy)
                        .Set(u => u.usu_fecha_ult_intento, DateTime.Today);

                    _db.Usuarios.UpdateOne(u => u.usu_id == usuario.usu_id, update);
                    if (intentosHoy >= 3)
                    {
                        _db.Usuarios.UpdateOne(u => u.usu_id == usuario.usu_id,
                            Builders<TblUsuario>.Update.Set(u => u.usu_estado, "B"));
                        return "Usuario bloqueado por exceso de intentos.";
                    }
                    return $"Contraseña incorrecta. Intentos restantes: {3 - intentosHoy}";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // ─────────────────── OBTENER USUARIO ────────────────────────
        public TblUsuario ObtenerUsuarioPorNick(string nick)
        {
            return _db.Usuarios.Find(u => u.usu_nick == nick).FirstOrDefault();
        }

        public TblUsuario ObtenerUsuarioPorId(int usu_id)
        {
            return _db.Usuarios.Find(u => u.usu_id == usu_id).FirstOrDefault();
        }

        public TblUsuario ObtenerUsuarioPorCorreo(string correo)
        {
            return _db.Usuarios.Find(u => u.usu_correo == correo).FirstOrDefault();
        }

        // ─────────────────── VALIDACIONES ──────────────────────────
        public bool ExisteCedula(string cedula)
        {
            return _db.Usuarios.Find(u => u.usu_cedula == cedula).Any();
        }

        public bool ExisteCorreo(string correo)
        {
            return _db.Usuarios.Find(u => u.usu_correo == correo).Any();
        }

        public bool ExisteNickParaOtroUsuario(string nick, int usuIdActual)
        {
            return _db.Usuarios.Find(u => u.usu_nick == nick && u.usu_id != usuIdActual).Any();
        }

        // ─────────────────── REGISTRO ──────────────────────────────
        public string RegistrarUsuario(string cedula, string nombres, string apellidos,
            string direccion, string celular, string correo, DateTime fecha_cumple,
            string nick_personalizado, string password, byte[] foto, int? tusu_id)
        {
            try
            {
                string nick = string.IsNullOrWhiteSpace(nick_personalizado)
                    ? GenerarNickUnico(nombres, apellidos)
                    : nick_personalizado;

                if (ExisteNickParaOtroUsuario(nick, 0))
                    throw new Exception("El nick ya está en uso.");

                byte[] hash = EncryptionHelper.Encrypt(password);
                var nuevo = new TblUsuario
                {
                    usu_id = ObtenerSiguienteId(),
                    usu_cedula = cedula,
                    usu_nombres = nombres,
                    usu_apellidos = apellidos,
                    usu_dirreccion = direccion,
                    usu_celular = celular,
                    usu_correo = correo,
                    usu_fecha_creacion = DateTime.Now,
                    usu_fecha_cumple = fecha_cumple,
                    usu_nick = nick,
                    usu_contrasena = hash,
                    usu_intentos = 0,
                    usu_intentos_dia = 0,
                    usu_estado = "A",
                    tusu_id = tusu_id ?? 1,
                    usu_foto = foto
                };
                _db.Usuarios.InsertOne(nuevo);
                return nick;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private int ObtenerSiguienteId()
        {
            var ultimo = _db.Usuarios.Find(FilterDefinition<TblUsuario>.Empty)
                .SortByDescending(u => u.usu_id)
                .FirstOrDefault();
            return ultimo == null ? 1 : ultimo.usu_id + 1;
        }

        private string GenerarNickUnico(string nombres, string apellidos)
        {
            string baseNick = (nombres.Trim().Substring(0, 1) + apellidos.Trim()).ToLower();
            string nick;
            int intentos = 0;
            do
            {
                nick = baseNick + (intentos > 0 ? new Random().Next(10, 99).ToString() : "");
                intentos++;
            } while (_db.Usuarios.Find(u => u.usu_nick == nick).Any() && intentos < 20);
            return nick;
        }

        // ─────────────────── ACTUALIZACIONES ────────────────────────
        public string ActualizarDatosUsuario(int usu_id, string nombres, string apellidos,
            string direccion, string celular, string correo, DateTime? fecha_cumple)
        {
            var update = Builders<TblUsuario>.Update
                .Set(u => u.usu_nombres, nombres)
                .Set(u => u.usu_apellidos, apellidos)
                .Set(u => u.usu_dirreccion, direccion)
                .Set(u => u.usu_celular, celular)
                .Set(u => u.usu_correo, correo)
                .Set(u => u.usu_fecha_cumple, fecha_cumple);
            var result = _db.Usuarios.UpdateOne(u => u.usu_id == usu_id, update);
            return result.ModifiedCount > 0 ? "Datos actualizados correctamente." : "Usuario no encontrado.";
        }

        public string ActualizarNickUsuario(int usu_id, string nuevoNick)
        {
            var update = Builders<TblUsuario>.Update.Set(u => u.usu_nick, nuevoNick);
            var result = _db.Usuarios.UpdateOne(u => u.usu_id == usu_id, update);
            return result.ModifiedCount > 0 ? "Nick actualizado correctamente." : "Usuario no encontrado.";
        }

        public string ActualizarFotoPerfil(int usu_id, byte[] foto)
        {
            var update = Builders<TblUsuario>.Update.Set(u => u.usu_foto, foto);
            var result = _db.Usuarios.UpdateOne(u => u.usu_id == usu_id, update);
            return result.ModifiedCount > 0 ? "Foto de perfil actualizada correctamente." : "Usuario no encontrado.";
        }

        public string ActualizarEstadoUsuario(int usu_id, string nuevoEstado)
        {
            var update = Builders<TblUsuario>.Update.Set(u => u.usu_estado, nuevoEstado);
            var result = _db.Usuarios.UpdateOne(u => u.usu_id == usu_id, update);
            return result.ModifiedCount > 0 ? "Estado actualizado correctamente." : "Usuario no encontrado.";
        }

        public string DesbloquearUsuario(int usu_id)
        {
            var update = Builders<TblUsuario>.Update
                .Set(u => u.usu_estado, "A")
                .Set(u => u.usu_intentos, 0)
                .Set(u => u.usu_intentos_dia, 0)
                .Set(u => u.usu_fecha_ult_intento, null);
            var result = _db.Usuarios.UpdateOne(u => u.usu_id == usu_id, update);
            return result.ModifiedCount > 0 ? "Usuario desbloqueado exitosamente." : "Usuario no encontrado.";
        }

        // ─────────────────── LISTADOS ──────────────────────────────
        public List<TblUsuario> ListarTodosLosUsuarios()
        {
            return _db.Usuarios.Find(FilterDefinition<TblUsuario>.Empty).ToList();
        }

        public List<TblUsuario> ListarUsuariosBloqueados()
        {
            return _db.Usuarios.Find(u => u.usu_estado == "B").ToList();
        }

        // ─────────────────── OTP (MongoDB) ─────────────────────────
        public string GenerarOTP(int usu_id)
        {
            string codigo = new Random().Next(100000, 999999).ToString();
            var otp = new TblOtpToken
            {
                usu_id = usu_id,
                otp_codigo = codigo,
                otp_fecha_generacion = DateTime.Now,
                otp_fecha_expiracion = DateTime.Now.AddMinutes(10),
                otp_utilizado = false,
                otp_intentos = 0
            };
            _db.OtpTokens.InsertOne(otp);
            return codigo;
        }

        public int ValidarOTP(int usu_id, string codigo, out string mensaje)
        {
            var token = _db.OtpTokens.Find(t => t.usu_id == usu_id && t.otp_codigo == codigo)
                .SortByDescending(t => t.otp_fecha_generacion)
                .FirstOrDefault();
            if (token == null)
            {
                mensaje = "Código incorrecto.";
                return 0;
            }
            if (token.otp_utilizado)
            {
                mensaje = "Este código ya fue usado.";
                return -2;
            }
            if (DateTime.Now > token.otp_fecha_expiracion)
            {
                mensaje = "El código ha expirado.";
                return -1;
            }
            _db.OtpTokens.UpdateOne(t => t.Id == token.Id,
                Builders<TblOtpToken>.Update.Set(t => t.otp_utilizado, true));
            mensaje = "Código verificado exitosamente.";
            return 1;
        }

        public bool ExisteOTPValido(int usu_id)
        {
            return _db.OtpTokens.Find(t => t.usu_id == usu_id && !t.otp_utilizado && t.otp_fecha_expiracion > DateTime.Now).Any();
        }

        // ─────────────────── RECUPERACIÓN ──────────────────────────
        public string GenerarClaveTemporal(string nickOCorreo, out string correoEncontrado)
        {
            correoEncontrado = "";
            var usuario = _db.Usuarios.Find(u => u.usu_nick == nickOCorreo || u.usu_correo == nickOCorreo).FirstOrDefault();
            if (usuario == null) return "Usuario no encontrado";

            string clave = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var update = Builders<TblUsuario>.Update
                .Set(u => u.usu_clave_temporal, clave)
                .Set(u => u.usu_fecha_clave_temp, DateTime.Now);
            _db.Usuarios.UpdateOne(u => u.usu_id == usuario.usu_id, update);
            correoEncontrado = usuario.usu_correo;
            return clave;
        }

        public string CambiarContrasenaConClave(string nick, string claveTemporal, string nuevaContrasena)
        {
            var usuario = _db.Usuarios.Find(u => u.usu_nick == nick).FirstOrDefault();
            if (usuario == null) return "Usuario no encontrado.";
            if (usuario.usu_clave_temporal != claveTemporal) return "Clave temporal incorrecta.";
            if (!usuario.usu_fecha_clave_temp.HasValue || (DateTime.Now - usuario.usu_fecha_clave_temp.Value).TotalMinutes > 15)
                return "Clave temporal expirada (máx. 15 min).";

            byte[] nuevoHash = EncryptionHelper.Encrypt(nuevaContrasena);
            var update = Builders<TblUsuario>.Update
                .Set(u => u.usu_contrasena, nuevoHash)
                .Set(u => u.usu_clave_temporal, null)
                .Set(u => u.usu_fecha_clave_temp, null)
                .Set(u => u.usu_estado, "A")
                .Set(u => u.usu_intentos, 0)
                .Set(u => u.usu_intentos_dia, 0);
            _db.Usuarios.UpdateOne(u => u.usu_id == usuario.usu_id, update);
            return "Contraseña actualizada exitosamente.";
        }

        public string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}