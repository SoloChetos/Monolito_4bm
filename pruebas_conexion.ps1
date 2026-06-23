$xml = '<testsuite name="Monolito.ConexionBD" tests="2" failures="0" time="0">'

# Prueba SQL Server (usando SqlClient nativo de .NET)
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

# Prueba MongoDB (verificando que el puerto 27017 está abierto)
try {
    $result = Test-NetConnection -ComputerName localhost -Port 27017 -WarningAction SilentlyContinue
    if ($result.TcpTestSucceeded) {
        $xml += '<testcase name="Conexion_MongoDB" classname="ConexionBD" time="0"/>'
    } else {
        throw "No se pudo conectar al puerto 27017 (MongoDB)."
    }
} catch {
    $xml += '<testcase name="Conexion_MongoDB" classname="ConexionBD" time="0"><failure message="Error: ' + $_.Exception.Message + '"/></testcase>'
}

$xml += '</testsuite>'
$xml | Out-File -FilePath test-results.xml -Encoding UTF8
Write-Host "[PRUEBA] Reporte test-results.xml generado."