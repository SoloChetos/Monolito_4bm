$xml = '<testsuite name="Monolito.ConexionBD" tests="2" failures="0" time="0">'

# Prueba SQL Server
try {
    $conn = New-Object System.Data.SqlClient.SqlConnection("Server=localhost\SQLEXPRESS;Database=Monolito4bm;Integrated Security=True;Connect Timeout=5")
    $conn.Open()
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT 1"
    $cmd.ExecuteScalar() | Out-Null
    $conn.Close()
    $xml += '<testcase name="Conexion_SQLServer" classname="ConexionBD" time="0"/>'
} catch {
    $xml += '<testcase name="Conexion_SQLServer" classname="ConexionBD" time="0"><failure message="Error: ' + $_.Exception.Message + '"/></testcase>'
}

# Prueba MongoDB
try {
    # Cargar ensamblados de MongoDB
    $driverPath = ".\packages\MongoDB.Driver.3.9.0\lib\net472\MongoDB.Driver.dll"
    $bsonPath = ".\packages\MongoDB.Bson.3.9.0\lib\net472\MongoDB.Bson.dll"
    if (Test-Path $driverPath) {
        Add-Type -Path $driverPath
        Add-Type -Path $bsonPath
        $client = New-Object MongoDB.Driver.MongoClient("mongodb://localhost:27017")
        $db = $client.GetDatabase("Monolito4bm")
        $db.RunCommand("{ping:1}") | Out-Null
        $xml += '<testcase name="Conexion_MongoDB" classname="ConexionBD" time="0"/>'
    } else {
        throw "No se encontraron las DLLs de MongoDB en $driverPath"
    }
} catch {
    $xml += '<testcase name="Conexion_MongoDB" classname="ConexionBD" time="0"><failure message="Error: ' + $_.Exception.Message + '"/></testcase>'
}

$xml += '</testsuite>'
$xml | Out-File -FilePath test-results.xml -Encoding UTF8
Write-Host "[PRUEBA] Reporte test-results.xml generado."