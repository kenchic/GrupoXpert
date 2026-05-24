using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed record AgregarComentarioCommand(
    Guid AvanceId,
    Guid AutorId,
    string Contenido
) : IRequest<Unit>;
