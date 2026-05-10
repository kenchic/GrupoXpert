using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

public sealed class RevocarAprobacionHandler(
    IUsuarioRepository usuarioRepository,
    IUnidadDeTrabajo unidadDeTrabajo) : IRequestHandler<RevocarAprobacionCommand>
{
    public async Task Handle(RevocarAprobacionCommand comando, CancellationToken cancelacion)
    {
        var colaborador = await usuarioRepository.ObtenerPorIdAsync(comando.ColaboradorId, cancelacion)
            ?? throw new ExcepcionDominio("Colaborador no encontrado.");

        colaborador.RevocarAprobacion();

        await usuarioRepository.ActualizarAsync(colaborador, cancelacion);
        await unidadDeTrabajo.GuardarCambiosAsync(cancelacion);
    }
}
