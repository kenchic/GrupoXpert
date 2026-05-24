namespace GrupoXpert.Application.Academia.Dtos;

public sealed record ComentarioDto(
    Guid Id,
    Guid AvanceId,
    Guid AutorId,
    string NombreAutor,
    string Contenido,
    DateTime FechaCreacion
);
