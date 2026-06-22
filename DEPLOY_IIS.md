# Despliegue en IIS - Monolito4bm

## Requisitos Previos
1. Windows con IIS habilitado
2. .NET Framework 4.8.1 instalado
3. SQL Server con la BD Monolito4bm configurada

## Pasos para Desplegar

### 1. Habilitar IIS en Windows
1. Ir a **Panel de Control** > **Programas** > **Activar o desactivar características de Windows**
2. Marcar **Internet Information Services**
3. Expandir y marcar **Herramientas de administración web** > **Consola de administración de IIS**
4. Expandir **Servicios World Wide Web** > **Características de desarrollo de aplicaciones** > marcar **ASP.NET 4.8**
5. Aceptar y esperar la instalación

### 2. Publicar desde Visual Studio
1. Clic derecho en el proyecto **Monolito_4bm** > **Publicar**
2. Seleccionar **Carpeta** como destino
3. Elegir una ruta, por ejemplo: `C:\inetpub\wwwroot\Monolito4bm`
4. Clic en **Publicar**

### 3. Configurar el Sitio en IIS Manager
1. Abrir **Administrador de IIS** (inetmgr)
2. Expandir **Sitios** > clic derecho > **Agregar sitio web**
3. Configurar:
   - **Nombre del sitio**: Monolito4bm
   - **Grupo de aplicaciones**: DefaultAppPool (o crear uno nuevo con .NET CLR v4.0)
   - **Ruta de acceso física**: `C:\inetpub\wwwroot\Monolito4bm`
   - **Puerto**: 8080 (o el que desees)
4. Aceptar

### 4. Configurar el Pool de Aplicaciones
1. Ir a **Grupos de aplicaciones** en IIS
2. Seleccionar el pool usado > **Configuración avanzada**
3. Verificar:
   - **Versión de .NET CLR**: v4.0
   - **Modo de canalización**: Integrado
   - **Identidad**: ApplicationPoolIdentity

### 5. Permisos de Carpeta
```
icacls "C:\inetpub\wwwroot\Monolito4bm" /grant "IIS_IUSRS:(OI)(CI)M"
icacls "C:\inetpub\wwwroot\Monolito4bm\Perfil" /grant "IIS_IUSRS:(OI)(CI)M"
```

### 6. Connection String de Producción
Editar el `Web.config` publicado si el servidor SQL es diferente:
```xml
<connectionStrings>
  <add name="Capa_Datos.Properties.Settings.Monolito4bmConnectionString"
       connectionString="Data Source=SERVIDOR\SQLEXPRESS;Initial Catalog=Monolito4bm;Integrated Security=True;Encrypt=False;Connect Timeout=10"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 7. Probar
Abrir el navegador y visitar: `http://localhost:8080/Login.aspx`
