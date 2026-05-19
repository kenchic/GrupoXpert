$AppSettingsPath = "Src\GrupoXpert\GrupoXpert.Infrastructure\Persistence\appsettings.json"
$json = Get-Content $AppSettingsPath -Raw | ConvertFrom-Json
$connStr = $json.ConnectionStrings.master

$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'Perfil' AND TABLE_NAME LIKE 'PerfilesColaboradores%'"
$reader = $cmd.ExecuteReader()

$dt = New-Object System.Data.DataTable
$dt.Load($reader)

$dt | Format-Table -AutoSize

$conn.Close()
