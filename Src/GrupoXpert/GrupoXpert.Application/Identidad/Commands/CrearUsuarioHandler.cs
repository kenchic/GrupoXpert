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
        // 1. Verificar que el email no esté ya registrado (unicidad)
        var emailNormalizado = solicitud.Email.Trim().ToLowerInvariant();

        if (await _usuarioRepositorio.ExisteEmailAsync(emailNormalizado, cancelacion))
            throw new ExcepcionDominio($"El correo electrónico '{solicitud.Email}' ya está registrado.");

        // 2. Hashear la clave antes de persistir (nunca guardar texto plano)
        var hashClave = _servicioHashClave.GenerarHash(solicitud.Clave);

        // 3. Generar token de activación único (UUID v4 o similar)
        var tokenActivacion = _generadorToken.GenerarToken();

        // 4. Crear el agregado de dominio (valida invariantes)
        var usuario = Usuario.Crear(
            email: emailNormalizado,
            hashClave: hashClave,
            nombre: solicitud.Nombre,
            tokenActivacion: tokenActivacion,
            imagen: solicitud.Imagen);

        // 5. Persistir el usuario
        await _usuarioRepositorio.AgregarAsync(usuario, cancelacion);

        // 5b. ACTIVACIÓN FORZADA (Deshabilitando temporalmente flujo de correo para desarrollo)
        usuario.ActivarCuenta(tokenActivacion);

        await _unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        // 6. El envío de correo se deshabilita temporalmente por solicitud del usuario
        /*
        var enlace = _urlActivacion.Construir(tokenActivacion);

        await _servicioCorreo.EnviarActivacionCuentaAsync(
            destinatario: emailNormalizado,
            nombre: solicitud.Nombre,
            enlaceActivacion: enlace,
            cancelacion: cancelacion);
        */

        return usuario.Id;
    }
}
