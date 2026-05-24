namespace GrupoXpert.Application.Academia.Dtos;

public sealed record ArchivoAdjuntoDto(
    Guid Id,
    Guid AvanceId,
    string NombreArchivo,
    string Url,
    long TamanioBytes,
    string TipoContenido,
    DateTime FechaSubida
);
