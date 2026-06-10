using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed record LiberarSolicitudCommand(Guid SolicitudId) : IRequest<Unit>;