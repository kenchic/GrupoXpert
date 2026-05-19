using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GrupoXpert.Domain.Perfil;
using GrupoXpert.Application.Common.Interfaces;

namespace GrupoXpert.Application.Perfil.Commands;

public class ValidarPerfilProfesionalHandler(
    IPerfilColaboradorRepository perfilColaboradorRepository,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<ValidarPerfilProfesionalCommand, Unit>
{
    public async Task<Unit> Handle(ValidarPerfilProfesionalCommand request, CancellationToken cancellationToken)
    {
        var perfil = await perfilColaboradorRepository.ObtenerPorUsuarioIdAsync(request.UsuarioId, cancellationToken);

        if (perfil == null)
        {
            throw new InvalidOperationException("El perfil del colaborador no existe.");
        }

        perfil.ValidarPerfil();
        perfil.ActualizarEvaluacionCalidad(request.EvaluacionCalidad);

        await perfilColaboradorRepository.ActualizarAsync(perfil, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);

        return Unit.Value;
    }
}
