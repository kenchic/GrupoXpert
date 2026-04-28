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
    IUsuarioRepository UsuarioRepository,
    IHashClaveService HashClaveService,
    ITokenService TokenService,
    IUnidadDeTrabajo unidadDeTrabajo) : IRequestHandler<IniciarSesionComando, ResultadoSesionDto>
{
    private readonly IUsuarioRepository _usuarioRepositorio = UsuarioRepository;
    private readonly IHashClaveService _servicioHashClave = HashClaveService;
    private readonly ITokenService _servicioToken = TokenService;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo = unidadDeTrabajo;

    public async Task<ResultadoSesionDto> Handle(IniciarSesionComando solicitud, CancellationToken cancelacion)
    {
        // 1. Obtener usuario del repositorio
        var usuario = await _usuarioRepositorio.ObtenerPorNombreUsuarioAsync(solicitud.NombreUsuario, cancelacion);

        if (usuario is null)
        {
            throw new ExcepcionDominio("Credenciales inválidas.");
        }

        // 2. Verificar clave usando el servicio de infraestructura
        if (!_servicioHashClave.Verificar(solicitud.Clave, usuario.Clave.HashClave))
        {
            throw new ExcepcionDominio("Credenciales inválidas.");
        }

        // 3. Lógica de negocio del dominio (marcar inicio de sesión)
        usuario.RegistrarInicioSesion();

        // 4. Persistir cambios (si hay eventos de dominio o estados que guardar)
        await _unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        // 5. Generar token de seguridad
        var token = _servicioToken.GenerarToken(usuario);

        // 6. Retornar DTO de resultado
        return new ResultadoSesionDto(
            token,
            usuario.NombreUsuario,
            usuario.Nombre,
            usuario.Imagen);
    }
}

