using System;
using MediatR;
using GrupoXpert.Application.Perfil.Dtos;

namespace GrupoXpert.Application.Perfil.Queries;

public record ObtenerPerfilColaboradorQuery(Guid UsuarioId) : IRequest<PerfilColaboradorDto?>;
