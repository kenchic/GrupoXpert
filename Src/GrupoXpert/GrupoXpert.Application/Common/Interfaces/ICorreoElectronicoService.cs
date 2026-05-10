namespace GrupoXpert.Application.Common.Interfaces;

/// <summary>
/// Servicio de envío de correos electrónicos.
/// La implementación concreta reside en la capa de Infraestructura.
/// </summary>
public interface ICorreoElectronicoService
{
    /// <summary>
    /// Envía el enlace de activación de cuenta al correo del nuevo usuario.
    /// </summary>
    /// <param name="destinatario">Dirección de correo electrónico del usuario.</param>
    /// <param name="nombre">Nombre del usuario para personalizar el mensaje.</param>
    /// <param name="enlaceActivacion">URL completa con el token de activación.</param>
    Task EnviarActivacionCuentaAsync(
        string destinatario,
        string nombre,
        string enlaceActivacion,
        CancellationToken cancelacion = default);

    Task EnviarAprobacionCuentaAsync(
        string destinatario,
        string nombre,
        CancellationToken cancelacion = default);

    /// <summary>
    /// Envía una notificación indicando que el perfil del colaborador ha sido enviado a validación.
    /// </summary>
    Task EnviarValidacionPerfilAsync(
        string destinatario,
        string nombre,
        CancellationToken cancelacion = default);
}
