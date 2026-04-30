using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

/// <summary>
/// Manejador para el comando ActivarCuentaComando.
/// Busca al usuario por el token, activa su cuenta y persiste los cambios.
/// </summary>
public sealed class ActivarCuentaHandler(
    IUsuarioRepository usuarioRepository,
    IUnidadDeTrabajo unidadDeTrabajo) : IRequestHandler<ActivarCuentaCommand, bool>
{
    private readonly IUsuarioRepository _usuarioRepositorio = usuarioRepository;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo = unidadDeTrabajo;

    public async Task<bool> Handle(ActivarCuentaCommand solicitud, CancellationToken cancelacion)
    {
        // 1. Obtener usuario por el token de activación
        var usuario = await _usuarioRepositorio.ObtenerPorTokenActivacionAsync(solicitud.Token, cancelacion);

        if (usuario is null)
        {
            throw new ExcepcionDominio("El enlace de activación no es válido o ha expirado.");
        }

        // 2. Ejecutar la lógica de activación en el agregado de dominio
        // (Valida si ya está activa, si el token coincide y si no ha expirado)
        usuario.ActivarCuenta(solicitud.Token);

        // 3. Persistir cambios
        await _unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        return true;
    }
}
