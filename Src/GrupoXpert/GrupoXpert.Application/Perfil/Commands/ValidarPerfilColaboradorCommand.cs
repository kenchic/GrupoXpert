using System;
using MediatR;

namespace GrupoXpert.Application.Perfil.Commands;

public record ValidarPerfilProfesionalCommand(
    Guid UsuarioId,
    decimal EvaluacionCalidad
) : IRequest<Unit>;
