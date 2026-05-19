using GrupoXpert.Application.Academia.Dtos;
using MediatR;

namespace GrupoXpert.Application.Academia.Queries;

public record ObtenerSolicitudPorIdQuery(Guid Id) : IRequest<SolicitudAcademicaDto?>;
