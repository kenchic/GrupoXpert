using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Exceptions;

namespace GrupoXpert.Domain.Academia;

public sealed class ArchivoAdjunto : Entity
{
    public Guid AvanceId { get; private set; }
    public string NombreArchivo { get; private set; }
    public string Url { get; private set; }
    public long TamanioBytes { get; private set; }
    public string TipoContenido { get; private set; }
    public DateTime FechaSubida { get; private set; }

    private ArchivoAdjunto()
    {
        NombreArchivo = string.Empty;
        Url = string.Empty;
        TipoContenido = string.Empty;
    }

    public ArchivoAdjunto(
        Guid avanceId,
        string nombreArchivo,
        string url,
        long tamanioBytes,
        string tipoContenido)
    {
        if (avanceId == Guid.Empty)
            throw new ArgumentException("El ID del avance es requerido.", nameof(avanceId));

        if (string.IsNullOrWhiteSpace(nombreArchivo))
            throw new ExcepcionDominio("El nombre del archivo es requerido.");

        if (string.IsNullOrWhiteSpace(url))
            throw new ExcepcionDominio("La URL del archivo es requerida.");

        if (tamanioBytes <= 0)
            throw new ExcepcionDominio("El tamaño del archivo debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(tipoContenido))
            throw new ExcepcionDominio("El tipo de contenido es requerido.");

        AvanceId = avanceId;
        NombreArchivo = nombreArchivo;
        Url = url;
        TamanioBytes = tamanioBytes;
        TipoContenido = tipoContenido;
        FechaSubida = DateTime.UtcNow;
    }
}
