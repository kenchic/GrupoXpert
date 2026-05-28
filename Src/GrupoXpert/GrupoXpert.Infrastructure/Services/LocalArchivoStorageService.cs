using GrupoXpert.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GrupoXpert.Infrastructure.Services;

public sealed class LocalArchivoStorageService(
    IConfiguration configuracion) : IArchivoStorageService
{
    private readonly string _rutaBase = configuracion["Storage:UploadPath"]
        ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");

    public async Task<string> GuardarAsync(
        string nombreArchivo,
        Stream contenido,
        string tipoContenido,
        CancellationToken cancellationToken = default)
    {
        var carpeta = Path.Combine(_rutaBase, "avances");
        Directory.CreateDirectory(carpeta);

        var extension = Path.GetExtension(nombreArchivo);
        var nombreSeguro = $"{Guid.NewGuid()}{extension}";
        var rutaCompleta = Path.Combine(carpeta, nombreSeguro);

        await using var fs = new FileStream(rutaCompleta, FileMode.Create);
        await contenido.CopyToAsync(fs, cancellationToken);

        return $"/uploads/avances/{nombreSeguro}";
    }
}
