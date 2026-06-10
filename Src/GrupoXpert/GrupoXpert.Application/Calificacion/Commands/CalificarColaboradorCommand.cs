using GrupoXpert.Application.Calificacion.Dtos;
using MediatR;

namespace GrupoXpert.Application.Calificacion.Commands;

public sealed record CalificarColaboradorCommand(
    Guid SolicitudId,
    Guid ClienteId,
    Guid ColaboradorId,
    int Puntaje,
    string? Observacion) : IRequest<CalificacionColaboradorDto>;