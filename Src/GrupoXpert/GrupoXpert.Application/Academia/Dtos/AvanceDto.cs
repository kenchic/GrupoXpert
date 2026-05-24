namespace GrupoXpert.Application.Academia.Dtos;

public sealed record AvanceDto(
    Guid Id,
    Guid SolicitudId,
    Guid AsesorId,
    string? NombreAsesor,
    string Descripcion,
    int NumeroFase,
    int Tipo,
    int Estado,
    DateTime FechaSubida,
    IReadOnlyList<ComentarioDto> Comentarios,
    IReadOnlyList<ArchivoAdjuntoDto> ArchivosAdjuntos
);
