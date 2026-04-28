$ErrorActionPreference = 'Stop'
$baseDir = "e:\Documentos\Proyectos\GradoXpert\Src\GrupoXpert"

# 1. Renombrar Archivos
$renames = @(
    @{ Path = "$baseDir\GrupoXpert.Application\Identidad\Comandos\IniciarSesionManejador.cs"; NewName = "IniciarSesionHandler.cs" },
    @{ Path = "$baseDir\GrupoXpert.Application\InyeccionDependencia.cs"; NewName = "DependencyInjection.cs" },
    @{ Path = "$baseDir\GrupoXpert.Domain\Identidad\IUsuarioRepositorio.cs"; NewName = "IUsuarioRepository.cs" },
    @{ Path = "$baseDir\GrupoXpert.Infrastructure\Persistencia\Configuraciones\UsuarioConfiguracion.cs"; NewName = "UsuarioConfiguration.cs" },
    @{ Path = "$baseDir\GrupoXpert.Infrastructure\Persistencia\Repositorios\UsuarioRepositorio.cs"; NewName = "UsuarioRepository.cs" },
    @{ Path = "$baseDir\GrupoXpert.Infrastructure\Servicios\ServicioHashClave.cs"; NewName = "HashClaveService.cs" },
    @{ Path = "$baseDir\GrupoXpert.Infrastructure\Servicios\ServicioToken.cs"; NewName = "TokenService.cs" },
    @{ Path = "$baseDir\GrupoXpert.Infrastructure\InyeccionDependencia.cs"; NewName = "DependencyInjection.cs" },
    @{ Path = "$baseDir\GrupoXpert.Application\Comun\Interfaces\IServicioHashClave.cs"; NewName = "IHashClaveService.cs" },
    @{ Path = "$baseDir\GrupoXpert.Application\Comun\Interfaces\IServicioToken.cs"; NewName = "ITokenService.cs" }
)

foreach ($r in $renames) {
    if (Test-Path $r.Path) {
        Rename-Item -Path $r.Path -NewName $r.NewName
        Write-Host "Renombrado archivo: $($r.NewName)"
    }
}

# 2. Mover AutenticacionController y eliminar Controladores
if (Test-Path "$baseDir\GrupoXpert.WebApi\Controladores\AutenticacionController.cs") {
    Move-Item -Path "$baseDir\GrupoXpert.WebApi\Controladores\AutenticacionController.cs" -Destination "$baseDir\GrupoXpert.WebApi\Controllers\AutenticacionController.cs" -Force
    Remove-Item -Path "$baseDir\GrupoXpert.WebApi\Controladores" -Recurse -Force
    Write-Host "Movido AutenticacionController.cs"
}

# 3. Renombrar Carpetas
$folderRenames = @(
    @{ Path = "$baseDir\GrupoXpert.Application\Comun"; NewName = "Common" },
    @{ Path = "$baseDir\GrupoXpert.Application\Identidad\Comandos"; NewName = "Commands" },
    @{ Path = "$baseDir\GrupoXpert.Domain\Identidad\Eventos"; NewName = "Events" },
    @{ Path = "$baseDir\GrupoXpert.Infrastructure\Persistencia\Configuraciones"; NewName = "Configurations" },
    @{ Path = "$baseDir\GrupoXpert.Infrastructure\Persistencia\Repositorios"; NewName = "Repositories" },
    @{ Path = "$baseDir\GrupoXpert.Infrastructure\Persistencia"; NewName = "Persistence" },
    @{ Path = "$baseDir\GrupoXpert.Infrastructure\Servicios"; NewName = "Services" }
)

foreach ($f in $folderRenames) {
    if (Test-Path $f.Path) {
        Rename-Item -Path $f.Path -NewName $f.NewName
        Write-Host "Renombrada carpeta: $($f.NewName)"
    }
}

# 4. Reemplazos de Texto (Namespaces y Nombres de Clases/Interfaces)
$replacements = @{
    "GrupoXpert\.Application\.Comun" = "GrupoXpert.Application.Common"
    "GrupoXpert\.Application\.Identidad\.Comandos" = "GrupoXpert.Application.Identidad.Commands"
    "GrupoXpert\.Domain\.Identidad\.Eventos" = "GrupoXpert.Domain.Identidad.Events"
    "GrupoXpert\.Infrastructure\.Persistencia\.Configuraciones" = "GrupoXpert.Infrastructure.Persistence.Configurations"
    "GrupoXpert\.Infrastructure\.Persistencia\.Repositorios" = "GrupoXpert.Infrastructure.Persistence.Repositories"
    "GrupoXpert\.Infrastructure\.Persistencia" = "GrupoXpert.Infrastructure.Persistence"
    "GrupoXpert\.Infrastructure\.Servicios" = "GrupoXpert.Infrastructure.Services"
    "GrupoXpert\.WebApi\.Controladores" = "GrupoXpert.WebApi.Controllers"
    
    "\bInyeccionDependencia\b" = "DependencyInjection"
    "\bAgregarAplicacion\b" = "AddApplication"
    "\bAgregarInfraestructura\b" = "AddInfrastructure"
    "\bIniciarSesionManejador\b" = "IniciarSesionHandler"
    "\bIUsuarioRepositorio\b" = "IUsuarioRepository"
    "\bUsuarioRepositorio\b" = "UsuarioRepository"
    "\bUsuarioConfiguracion\b" = "UsuarioConfiguration"
    "\bIServicioHashClave\b" = "IHashClaveService"
    "\bServicioHashClave\b" = "HashClaveService"
    "\bIServicioToken\b" = "ITokenService"
    "\bServicioToken\b" = "TokenService"
}

$files = Get-ChildItem -Path $baseDir -Recurse -Filter "*.cs" | Where-Object { $_.FullName -notmatch '\\bin\\|\\obj\\' }

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    foreach ($key in $replacements.Keys) {
        if ($content -match $key) {
            $content = $content -replace $key, $replacements[$key]
            $modified = $true
        }
    }
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content
        Write-Host "Actualizado contenido de: $($file.Name)"
    }
}

Write-Host "Refactorización completada."
