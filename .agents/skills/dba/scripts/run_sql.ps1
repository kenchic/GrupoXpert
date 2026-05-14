<#
.SYNOPSIS
    Ejecuta un script SQL (.sql) contra SQL Server, leyendo la conexión
    automáticamente desde el appsettings.json del proyecto Spherical.
.PARAMETER ScriptFile
    Ruta absoluta (o relativa al workspace) al archivo .sql a ejecutar.
.PARAMETER AppSettingsPath
    Ruta al appsettings.json. Por defecto busca en src/Spherical.Api/appsettings.json
.PARAMETER ConnectionName
    Nombre de la ConnectionString dentro del JSON. Por defecto: "CadenaConexion"
.EXAMPLE
    .\run_sql.ps1 -ScriptFile "docs/sql/CreateUsers.sql"
.EXAMPLE
    .\run_sql.ps1 -ScriptFile "docs/sql/CreateUsers.sql" -ConnectionName "CadenaConexion"
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$ScriptFile,

    [string]$AppSettingsPath = "",
    [string]$ConnectionName = "CadenaConexion"
)

# ─── Funciones auxiliares ──────────────────────────────────────────────────────

function Parse-ConnectionString {
    param([string]$connStr)

    $result = @{
        Server        = ""
        Database      = ""
        User          = ""
        Password      = ""
        IsWindowsAuth = $false
    }

    foreach ($part in $connStr -split ";") {
        $kv = $part.Trim() -split "=", 2
        if ($kv.Count -ne 2) { continue }
        $key = $kv[0].Trim().ToLower()
        $value = $kv[1].Trim()

        switch -Wildcard ($key) {
            "data source" { $result.Server = $value }
            "server" { $result.Server = $value }
            "initial catalog" { $result.Database = $value }
            "database" { $result.Database = $value }
            "user id" { $result.User = $value }
            "uid" { $result.User = $value }
            "password" { $result.Password = $value }
            "pwd" { $result.Password = $value }
            "integrated security" {
                if ($value -match "^(true|sspi|yes)$") {
                    $result.IsWindowsAuth = $true
                }
            }
        }
    }
    return $result
}

function Find-AppSettings {
    # Busca el appsettings.json subiendo desde el directorio actual
    $candidates = @(
        "src\GrupoXpert\GrupoXpert.Infrastructure\Persistence\appsettings.json",
        "GrupoXpert.Infrastructure\appsettings.json",
        "appsettings.json"
    )

    # Buscar relativo al script
    $scriptDir = Split-Path -Parent $MyInvocation.ScriptName
    $workspaceRoot = Resolve-Path (Join-Path $scriptDir "..\..\..") -ErrorAction SilentlyContinue

    foreach ($candidate in $candidates) {
        $fullPath = Join-Path $workspaceRoot $candidate
        if (Test-Path $fullPath) { return $fullPath }
    }

    # Fallback: buscar desde el directorio actual
    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) { return (Resolve-Path $candidate).Path }
    }

    return $null
}

# ─── Inicio del script ─────────────────────────────────────────────────────────

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "  🗄️  DBA Skill :: Ejecutor de Scripts SQL Server" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan

# 1. Validar archivo SQL
if (-not (Test-Path $ScriptFile)) {
    Write-Error "❌ Archivo SQL no encontrado: $ScriptFile"
    exit 1
}

# 2. Localizar appsettings.json
if ([string]::IsNullOrEmpty($AppSettingsPath)) {
    $AppSettingsPath = Find-AppSettings
}

if ([string]::IsNullOrEmpty($AppSettingsPath) -or -not (Test-Path $AppSettingsPath)) {
    Write-Error "❌ No se encontró appsettings.json. Especifica la ruta con -AppSettingsPath."
    exit 1
}

Write-Host "  📄 Config   : $AppSettingsPath" -ForegroundColor White

# 3. Leer y parsear el JSON
try {
    $json = Get-Content $AppSettingsPath -Raw | ConvertFrom-Json
    $connStr = $json.ConnectionStrings.$ConnectionName

    if ([string]::IsNullOrEmpty($connStr)) {
        Write-Error "❌ No se encontró la ConnectionString '$ConnectionName' en el appsettings.json."
        exit 1
    }
}
catch {
    Write-Error "❌ Error al leer appsettings.json: $_"
    exit 1
}

# 4. Parsear los componentes de la cadena de conexión
$conn = Parse-ConnectionString -connStr $connStr

if ([string]::IsNullOrEmpty($conn.Server) -or [string]::IsNullOrEmpty($conn.Database)) {
    Write-Error "❌ No se pudo extraer Server o Database de la ConnectionString."
    exit 1
}

Write-Host "  🖥️  Servidor : $($conn.Server)" -ForegroundColor White
Write-Host "  🗃️  Base     : $($conn.Database)" -ForegroundColor White
Write-Host "  🔐 Auth     : $(if ($conn.IsWindowsAuth) { 'Windows (Integrada)' } else { "SQL ($($conn.User))" })" -ForegroundColor White
Write-Host "  📜 Script   : $ScriptFile" -ForegroundColor White
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""

# 5. Verificar herramienta disponible
$invokeAvailable = Get-Command Invoke-Sqlcmd -ErrorAction SilentlyContinue
$sqlcmdAvailable = Get-Command sqlcmd       -ErrorAction SilentlyContinue

if (-not $invokeAvailable -and -not $sqlcmdAvailable) {
    Write-Error "❌ No se encontró 'sqlcmd' ni 'Invoke-Sqlcmd'."
    Write-Host "   → Instala el módulo: Install-Module -Name SqlServer -Scope CurrentUser" -ForegroundColor Yellow
    exit 1
}

# 6. Ejecutar el script
try {
    if ($invokeAvailable) {
        Write-Host "📦 Ejecutando con Invoke-Sqlcmd..." -ForegroundColor Yellow

        $params = @{
            ServerInstance         = $conn.Server
            Database               = $conn.Database
            InputFile              = $ScriptFile
            TrustServerCertificate = $true
            ErrorAction            = "Stop"
            Verbose                = $true
        }

        if (-not $conn.IsWindowsAuth) {
            $params["Username"] = $conn.User
            $params["Password"] = $conn.Password
        }

        Invoke-Sqlcmd @params

    }
    else {
        Write-Host "📦 Ejecutando con sqlcmd..." -ForegroundColor Yellow

        if ($conn.IsWindowsAuth) {
            sqlcmd -S $conn.Server -d $conn.Database -i $ScriptFile -E -b
        }
        else {
            sqlcmd -S $conn.Server -d $conn.Database -U $conn.User -P $conn.Password -i $ScriptFile -b
        }

        if ($LASTEXITCODE -ne 0) { throw "sqlcmd terminó con código: $LASTEXITCODE" }
    }

    Write-Host ""
    Write-Host "✅ Script ejecutado correctamente en '$($conn.Database)'." -ForegroundColor Green

}
catch {
    Write-Host ""
    Write-Error "❌ Error durante la ejecución: $_"
    exit 1
}
