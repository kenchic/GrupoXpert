using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

public sealed class ValidarPerfilColaboradorHandler(
    IUsuarioRepository usuarioRepository,
    IUnidadDeTrabajo unidadDeTrabajo,
    ICorreoElectronicoService correoService) : IRequestHandler<ValidarPerfilColaboradorCommand>
{
    public async Task Handle(ValidarPerfilColaboradorCommand comando, CancellationToken cancelacion)
    {
        var colaborador = await usuarioRepository.ObtenerPorIdAsync(comando.ColaboradorId, cancelacion)
            ?? throw new ExcepcionDominio("Colaborador no encontrado.");

        colaborador.ValidarPerfil();

        await usuarioRepository.ActualizarAsync(colaborador, cancelacion);
        await unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        await correoService.EnviarValidacionPerfilAsync(colaborador.Correo.Valor, colaborador.Nombre, cancelacion);
    }
}
