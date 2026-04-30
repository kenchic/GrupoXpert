$baseDir = "e:\Documentos\Proyectos\GrupoXpert\Src\GrupoXpert"

function Move-OrRenameDir($source, $dest) {
    if (Test-Path $source) {
        if (Test-Path $dest) {
            # Move contents
            Get-ChildItem -Path $source | Move-Item -Destination $dest -Force
            Remove-Item -Path $source -Recurse -Force
            Write-Host "Movido contenido de $source a $dest y borrado original."
        } else {
            Rename-Item -Path $source -NewName (Split-Path $dest -Leaf)
            Write-Host "Renombrada carpeta: $source a $dest"
        }
    }
}

# 3. Renombrar Carpetas
Move-OrRenameDir "$baseDir\GrupoXpert.Application\Comun" "$baseDir\GrupoXpert.Application\Common"
Move-OrRenameDir "$baseDir\GrupoXpert.Application\Identidad\Comandos" "$baseDir\GrupoXpert.Application\Identidad\Commands"
Move-OrRenameDir "$baseDir\GrupoXpert.Domain\Identidad\Eventos" "$baseDir\GrupoXpert.Domain\Identidad\Events"
Move-OrRenameDir "$baseDir\GrupoXpert.Infrastructure\Persistencia\Configuraciones" "$baseDir\GrupoXpert.Infrastructure\Persistencia\Configurations"
Move-OrRenameDir "$baseDir\GrupoXpert.Infrastructure\Persistencia\Repositorios" "$baseDir\GrupoXpert.Infrastructure\Persistencia\Repositories"
Move-OrRenameDir "$baseDir\GrupoXpert.Infrastructure\Persistencia" "$baseDir\GrupoXpert.Infrastructure\Persistence"
Move-OrRenameDir "$baseDir\GrupoXpert.Infrastructure\Servicios" "$baseDir\GrupoXpert.Infrastructure\Services"

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
