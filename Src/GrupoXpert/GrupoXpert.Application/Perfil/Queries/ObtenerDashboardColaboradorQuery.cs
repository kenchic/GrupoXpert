using GrupoXpert.Application.Perfil.Dtos;
using MediatR;

namespace GrupoXpert.Application.Perfil.Queries;

public record ObtenerDashboardColaboradorQuery(Guid UsuarioId) : IRequest<DashboardColaboradorDto?>;