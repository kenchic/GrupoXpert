using GrupoXpert.Application.Calidad.Dtos;
using MediatR;

namespace GrupoXpert.Application.Calidad.Commands;

public sealed record CrearRevisionCalidadCommand(
    Guid AvanceId,
    Guid RevisorId
) : IRequest<RevisionCalidadDto>;
