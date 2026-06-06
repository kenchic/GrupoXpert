using GrupoXpert.Application.Calidad.Dtos;
using MediatR;

namespace GrupoXpert.Application.Calidad.Queries;

public sealed record ObtenerRevisionesPendientesQuery() : IRequest<IReadOnlyList<RevisionCalidadDto>>;
