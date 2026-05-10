using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

public sealed record RevocarAprobacionCommand(
    Guid ColaboradorId,
    Guid AdministradorId
) : IRequest;
