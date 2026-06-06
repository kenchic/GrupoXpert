using GrupoXpert.Application.Calidad.Dtos;
using MediatR;

namespace GrupoXpert.Application.Calidad.Commands;

public sealed record RechazarRevisionCommand(
    Guid RevisionId,
    string Observaciones
) : IRequest<RevisionCalidadDto>;
