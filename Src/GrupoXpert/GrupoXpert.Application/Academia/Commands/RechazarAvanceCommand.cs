using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed record RechazarAvanceCommand(Guid AvanceId) : IRequest<Unit>;
