-- =====================================================
-- MONOLITO 4BM - MODULO DE SEGURIDAD - SCRIPTS SQL
-- Ejecutar en SSMS contra la BD Monolito4bm
-- =====================================================

USE Monolito4bm;
GO

-- =====================================================
-- 1. TABLA tbl_usuario_imagen (multiples imagenes)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_usuario_imagen')
BEGIN
    CREATE TABLE tbl_usuario_imagen (
        img_id INT IDENTITY(1,1) PRIMARY KEY,
        usu_id INT NOT NULL,
        img_datos VARBINARY(MAX) NOT NULL,
        img_nombre VARCHAR(100),
        img_tipo VARCHAR(20),
        img_tamano INT,
        img_es_perfil BIT DEFAULT 0,
        img_fecha_subida DATETIME DEFAULT GETDATE(),
        img_estado CHAR(1) DEFAULT 'A',
        CONSTRAINT FK_imagen_usuario FOREIGN KEY (usu_id) REFERENCES tbl_usuario(usu_id)
    );
END
GO

-- =====================================================
-- 2. COLUMNAS NUEVAS en tbl_usuario
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_usuario') AND name = 'usu_clave_temporal')
    ALTER TABLE tbl_usuario ADD usu_clave_temporal VARCHAR(100) NULL;
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_usuario') AND name = 'usu_fecha_clave_temp')
    ALTER TABLE tbl_usuario ADD usu_fecha_clave_temp DATETIME NULL;
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_usuario') AND name = 'usu_otp_secret')
    ALTER TABLE tbl_usuario ADD usu_otp_secret VARCHAR(32) NULL;
GO

-- =====================================================
-- 3. SP: Desbloquear Usuario
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_DesbloquearUsuario')
    DROP PROCEDURE sp_DesbloquearUsuario;
GO

CREATE PROCEDURE sp_DesbloquearUsuario
    @usu_id INT
AS
BEGIN
    UPDATE tbl_usuario
    SET usu_estado = 'A', usu_intentos = 0, usu_intentos_dia = 0
    WHERE usu_id = @usu_id;

    SELECT 'Usuario desbloqueado exitosamente' AS Mensaje;
END
GO

-- =====================================================
-- 4. SP: Listar Usuarios Bloqueados
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ListarUsuariosBloqueados')
    DROP PROCEDURE sp_ListarUsuariosBloqueados;
GO

CREATE PROCEDURE sp_ListarUsuariosBloqueados
AS
BEGIN
    SELECT u.usu_id, u.usu_cedula, u.usu_nombres, u.usu_apellidos,
           u.usu_nick, u.usu_correo, u.usu_celular, u.usu_intentos,
           u.usu_intentos_dia, u.usu_fecha_ult_intento,
           t.tusu_nombre AS TipoUsuario
    FROM tbl_usuario u
    LEFT JOIN tbl_tipo_usuario t ON u.tusu_id = t.tusu_id
    WHERE u.usu_estado = 'B';
END
GO

-- =====================================================
-- 5. SP: Generar Clave Temporal
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GenerarClaveTemporal')
    DROP PROCEDURE sp_GenerarClaveTemporal;
GO

CREATE PROCEDURE sp_GenerarClaveTemporal
    @nick_o_correo VARCHAR(100),
    @clave_temporal VARCHAR(100)
AS
BEGIN
    DECLARE @usu_id INT, @celular VARCHAR(10), @correo VARCHAR(100), @nick VARCHAR(50);

    SELECT @usu_id = usu_id, @celular = usu_celular, @correo = usu_correo, @nick = usu_nick
    FROM tbl_usuario
    WHERE (usu_nick = @nick_o_correo OR usu_correo = @nick_o_correo) AND usu_estado IN ('A', 'B');

    IF @usu_id IS NULL
    BEGIN
        SELECT 'Usuario no encontrado' AS Mensaje, NULL AS Celular, NULL AS Correo;
        RETURN;
    END

    UPDATE tbl_usuario
    SET usu_clave_temporal = @clave_temporal,
        usu_fecha_clave_temp = GETDATE()
    WHERE usu_id = @usu_id;

    SELECT 'Clave temporal generada' AS Mensaje, @celular AS Celular, @correo AS Correo, @nick AS Nick;
END
GO

-- =====================================================
-- 6. SP: Validar Clave Temporal y Cambiar Contrasena
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CambiarContrasenaConClave')
    DROP PROCEDURE sp_CambiarContrasenaConClave;
GO

CREATE PROCEDURE sp_CambiarContrasenaConClave
    @nick VARCHAR(50),
    @clave_temporal VARCHAR(100),
    @nueva_contrasena VARCHAR(50)
AS
BEGIN
    DECLARE @usu_id INT, @clave_guardada VARCHAR(100), @fecha_clave DATETIME;

    SELECT @usu_id = usu_id, @clave_guardada = usu_clave_temporal, @fecha_clave = usu_fecha_clave_temp
    FROM tbl_usuario
    WHERE usu_nick = @nick;

    IF @usu_id IS NULL
    BEGIN
        SELECT 'Usuario no encontrado' AS Mensaje; RETURN;
    END

    IF @clave_guardada IS NULL OR @clave_guardada != @clave_temporal
    BEGIN
        SELECT 'Clave temporal incorrecta' AS Mensaje; RETURN;
    END

    IF DATEDIFF(MINUTE, @fecha_clave, GETDATE()) > 15
    BEGIN
        SELECT 'Clave temporal expirada (max 15 min)' AS Mensaje; RETURN;
    END

    UPDATE tbl_usuario
    SET usu_contrasena = ENCRYPTBYPASSPHRASE('monolitoKey', @nueva_contrasena),
        usu_clave_temporal = NULL,
        usu_fecha_clave_temp = NULL,
        usu_estado = 'A',
        usu_intentos = 0,
        usu_intentos_dia = 0
    WHERE usu_id = @usu_id;

    SELECT 'Contrasena actualizada exitosamente' AS Mensaje;
END
GO

-- =====================================================
-- 7. SP: Guardar Imagen
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GuardarImagen')
    DROP PROCEDURE sp_GuardarImagen;
GO

CREATE PROCEDURE sp_GuardarImagen
    @usu_id INT,
    @img_datos VARBINARY(MAX),
    @img_nombre VARCHAR(100),
    @img_tipo VARCHAR(20),
    @img_tamano INT,
    @img_es_perfil BIT = 0
AS
BEGIN
    -- Si es foto de perfil, desactivar la anterior
    IF @img_es_perfil = 1
    BEGIN
        UPDATE tbl_usuario_imagen
        SET img_es_perfil = 0
        WHERE usu_id = @usu_id AND img_es_perfil = 1;
    END

    INSERT INTO tbl_usuario_imagen (usu_id, img_datos, img_nombre, img_tipo, img_tamano, img_es_perfil)
    VALUES (@usu_id, @img_datos, @img_nombre, @img_tipo, @img_tamano, @img_es_perfil);

    SELECT SCOPE_IDENTITY() AS ImagenId;
END
GO

-- =====================================================
-- 8. SP: Obtener Foto de Perfil
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ObtenerFotoPerfil')
    DROP PROCEDURE sp_ObtenerFotoPerfil;
GO

CREATE PROCEDURE sp_ObtenerFotoPerfil
    @usu_id INT
AS
BEGIN
    SELECT TOP 1 img_datos, img_tipo
    FROM tbl_usuario_imagen
    WHERE usu_id = @usu_id AND img_es_perfil = 1 AND img_estado = 'A'
    ORDER BY img_fecha_subida DESC;
END
GO

-- =====================================================
-- 9. SP: Obtener datos usuario para sesion
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ObtenerUsuarioPorNick')
    DROP PROCEDURE sp_ObtenerUsuarioPorNick;
GO

CREATE PROCEDURE sp_ObtenerUsuarioPorNick
    @nick VARCHAR(50)
AS
BEGIN
    SELECT u.usu_id, u.usu_nick, u.usu_nombres, u.usu_apellidos,
           u.usu_correo, u.usu_celular, u.tusu_id, u.usu_otp_secret,
           u.usu_foto, u.usu_estado
    FROM tbl_usuario u
    WHERE u.usu_nick = @nick;
END
GO

-- =====================================================
-- 10. SP: Guardar OTP Secret
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GuardarOtpSecret')
    DROP PROCEDURE sp_GuardarOtpSecret;
GO

CREATE PROCEDURE sp_GuardarOtpSecret
    @usu_id INT,
    @otp_secret VARCHAR(32)
AS
BEGIN
    UPDATE tbl_usuario
    SET usu_otp_secret = @otp_secret
    WHERE usu_id = @usu_id;

    SELECT 'OTP configurado exitosamente' AS Mensaje;
END
GO

PRINT '=== Todos los scripts ejecutados exitosamente ==='
GO
