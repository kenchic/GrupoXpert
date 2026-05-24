using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed record AprobarAvanceCommand(Guid AvanceId) : IRequest<Unit>;
