namespace GrupoXpert.Application.Common.Interfaces;

public interface IArchivoStorageService
{
    Task<string> GuardarAsync(
        string nombreArchivo,
        Stream contenido,
        string tipoContenido,
        CancellationToken cancellationToken = default);
}
