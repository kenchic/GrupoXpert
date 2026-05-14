using GrupoXpert.Application.Perfil.Dtos;
using MediatR;

namespace GrupoXpert.Application.Perfil.Queries;

public record ObtenerPerfilPorUsuarioQuery(Guid UsuarioId) : IRequest<PerfilClienteDto?>;
