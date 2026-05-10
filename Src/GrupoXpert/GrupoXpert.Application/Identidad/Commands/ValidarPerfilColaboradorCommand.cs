using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

public sealed record ValidarPerfilColaboradorCommand(
    Guid ColaboradorId,
    Guid AdministradorId
) : IRequest;
