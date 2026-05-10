using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

public sealed record AprobarColaboradorCommand(
    Guid ColaboradorId,
    Guid AdministradorId
) : IRequest;
