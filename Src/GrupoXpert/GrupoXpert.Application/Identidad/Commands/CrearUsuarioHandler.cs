using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

/// <summary>
/// Manejador del comando CrearUsuarioComando.
/// Orquesta: validación de unicidad de email, hashing de clave, generación de token,
/// persistencia y envío del correo de activación.
/// </summary>
public sealed class CrearUsuarioHandler(
    IUsuarioRepository usuarioRepository,
    IHashClaveService hashClaveService,
    ICorreoElectronicoService correoService,
    IGeneradorTokenService generadorToken,
    IUrlActivacionService urlActivacion,
    IUnidadDeTrabajo unidadDeTrabajo) : IRequestHandler<CrearUsuarioCommand, Guid>
{
    private readonly IUsuarioRepository _usuarioRepositorio = usuarioRepository;
    private readonly IHashClaveService _servicioHashClave = hashClaveService;
    private readonly ICorreoElectronicoService _servicioCorreo = correoService;
    private readonly IGeneradorTokenService _generadorToken = generadorToken;
    private readonly IUrlActivacionService _urlActivacion = urlActivacion;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo = unidadDeTrabajo;

    public async Task<Guid> Handle(CrearUsuarioCommand solicitud, CancellationToken cancelacion)
    {
        var correoNormalizado = solicitud.Correo.Trim().ToLowerInvariant();

        if (await _usuarioRepositorio.ExisteCorreoAsync(correoNormalizado, cancelacion))
            throw new ExcepcionDominio($"El correo electrónico '{solicitud.Correo}' ya está registrado.");

        var hashClave = _servicioHashClave.GenerarHash(solicitud.Clave);

        var tokenActivacion = _generadorToken.GenerarToken();

        var usuario = Usuario.Crear(
            correo: correoNormalizado,
            hashClave: hashClave,
            nombre: solicitud.Nombre,
            tokenActivacion: tokenActivacion,
            imagen: solicitud.Imagen,
            tipo: solicitud.Tipo);

        await _usuarioRepositorio.AgregarAsync(usuario, cancelacion);

        if (solicitud.ActivacionAutomatica)
        {
            usuario.ActivarCuenta(tokenActivacion);
        }
        else
        {
            var enlaceActivacion = _urlActivacion.Construir(tokenActivacion);
            
            await _servicioCorreo.EnviarActivacionCuentaAsync(
                destinatario: correoNormalizado,
                nombre: solicitud.Nombre,
                enlaceActivacion: enlaceActivacion,
                cancelacion: cancelacion);
        }

        await _unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        return usuario.Id;
    }
}
