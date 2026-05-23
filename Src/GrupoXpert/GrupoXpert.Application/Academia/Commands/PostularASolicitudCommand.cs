using System;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

/// <summary>
/// Comando para que un colaborador se postule a una solicitud académica.
/// </summary>
public sealed record PostularASolicitudCommand(Guid SolicitudId, Guid ColaboradorId) : IRequest<Unit>;
