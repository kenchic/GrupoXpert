using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed record AgregarArchivoCommand(
    Guid AvanceId,
    string NombreArchivo,
    string Url,
    long TamanioBytes,
    string TipoContenido
) : IRequest<Unit>;
