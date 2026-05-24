using System;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

/// <summary>
/// Comando para que el administrador seleccione un postulado específico para una solicitud.
/// </summary>
public sealed record SeleccionarPostuladoCommand(Guid SolicitudId, Guid ColaboradorId) : IRequest<Unit>;
