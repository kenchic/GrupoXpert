using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public record AsignarAsesorCommand(
    Guid SolicitudId,
    Guid AsesorId
) : IRequest<Unit>;
