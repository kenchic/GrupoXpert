$AppSettingsPath = "Src\GrupoXpert\GrupoXpert.Infrastructure\Persistence\appsettings.json"
$json = Get-Content $AppSettingsPath -Raw | ConvertFrom-Json
$connStr = $json.ConnectionStrings.master

$result = @{}
foreach ($part in $connStr -split ";") {
    $kv = $part.Trim() -split "=", 2
    if ($kv.Count -eq 2) { $result[$kv[0].Trim().ToLower()] = $kv[1].Trim() }
}

$server = $result["data source"]
if (-not $server) { $server = $result["server"] }

$database = $result["initial catalog"]
if (-not $database) { $database = $result["database"] }

$user = $result["user id"]
if (-not $user) { $user = $result["uid"] }

$pwd = $result["password"]
if (-not $pwd) { $pwd = $result["pwd"] }

$query = "USE [GrupoXpert]; SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'Perfil' AND TABLE_NAME LIKE 'PerfilesColaboradores%';"

Write-Host "Verificando con sqlcmd..."
if ($user -and $pwd) {
    sqlcmd -S $server -d $database -U $user -P $pwd -Q $query
} else {
    sqlcmd -S $server -d $database -E -Q $query
}
