using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

public sealed class AprobarColaboradorHandler(
    IUsuarioRepository usuarioRepository,
    IUnidadDeTrabajo unidadDeTrabajo,
    ICorreoElectronicoService correoService) : IRequestHandler<AprobarColaboradorCommand>
{
    public async Task Handle(AprobarColaboradorCommand comando, CancellationToken cancelacion)
    {
        var colaborador = await usuarioRepository.ObtenerPorIdAsync(comando.ColaboradorId, cancelacion)
            ?? throw new ExcepcionDominio("Colaborador no encontrado.");

        colaborador.AprobarCuenta(comando.AdministradorId);

        await usuarioRepository.ActualizarAsync(colaborador, cancelacion);
        await unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        // Enviar correo de notificación
        await correoService.EnviarAprobacionCuentaAsync(colaborador.Correo.Valor, colaborador.Nombre, cancelacion);
    }
}
