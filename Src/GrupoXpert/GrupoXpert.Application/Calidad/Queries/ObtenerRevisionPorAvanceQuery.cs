using GrupoXpert.Application.Calidad.Dtos;
using MediatR;

namespace GrupoXpert.Application.Calidad.Queries;

public sealed record ObtenerRevisionPorAvanceQuery(Guid AvanceId) : IRequest<RevisionCalidadDto?>;
