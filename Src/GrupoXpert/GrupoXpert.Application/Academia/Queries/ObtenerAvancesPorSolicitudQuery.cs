using GrupoXpert.Application.Academia.Dtos;
using MediatR;

namespace GrupoXpert.Application.Academia.Queries;

public sealed record ObtenerAvancesPorSolicitudQuery(Guid SolicitudId) : IRequest<IReadOnlyList<AvanceDto>>;
