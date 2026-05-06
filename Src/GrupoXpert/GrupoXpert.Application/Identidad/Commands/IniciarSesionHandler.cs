using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Application.Identidad.Dtos;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

/// <summary>
/// Manejador para el comando IniciarSesionComando.
/// Orquesta la validación de credenciales y generación de token.
/// </summary>
public sealed class IniciarSesionHandler(
    IUsuarioRepository repositorioUsuario,
    IHashClaveService servicioHashClave,
    ITokenAccesoService servicioToken,
    IUnidadDeTrabajo unidadDeTrabajo) : IRequestHandler<IniciarSesionCommand, ResultadoSesionDto>
{
    private readonly IUsuarioRepository _usuarioRepositorio = repositorioUsuario;
    private readonly IHashClaveService _servicioHashClave = servicioHashClave;
    private readonly ITokenAccesoService _servicioToken = servicioToken;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo = unidadDeTrabajo;

    public async Task<ResultadoSesionDto> Handle(IniciarSesionCommand solicitud, CancellationToken cancelacion)
    {
        var usuario = await _usuarioRepositorio.ObtenerPorCorreoAsync(solicitud.Correo, cancelacion);

        if (usuario is null)
            throw new ExcepcionDominio("Credenciales inválidas.");

        if (!_servicioHashClave.Verificar(solicitud.Clave, usuario.Clave.HashClave))
            throw new ExcepcionDominio("Credenciales inválidas.");

        if (!usuario.EstaActivo)
            throw new ExcepcionDominio("La cuenta no está activa. Por favor verifica tu correo para activarla.");

        usuario.RegistrarInicioSesion();

        await _unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        var token = _servicioToken.GenerarToken(usuario);

        return new ResultadoSesionDto(
            token,
            usuario.Correo.Valor,
            usuario.Nombre,
            usuario.Imagen);
    }
}
