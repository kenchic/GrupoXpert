using GrupoXpert.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace GrupoXpert.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de correo electrónico.
/// Actualmente simula el envío logueando el contenido (Mock/Development).
/// En producción, esta clase se integrará con SendGrid, Amazon SES o SMTP.
/// </summary>
public sealed class CorreoElectronicoService(ILogger<CorreoElectronicoService> logger) : ICorreoElectronicoService
{
    private readonly ILogger<CorreoElectronicoService> _logger = logger;

    public async Task EnviarActivacionCuentaAsync(
        string destinatario, 
        string nombre, 
        string enlaceActivacion, 
        CancellationToken cancelacion = default)
    {
        // Simulamos un retraso de red
        await Task.Delay(500, cancelacion);

        _logger.LogInformation(
            "📧 [SIMULACIÓN CORREO] Enviando activación a: {Destinatario}\n" +
            "Hola {Nombre},\n" +
            "Gracias por registrarte en GrupoXpert. Para activar tu cuenta, haz clic aquí: {Enlace}",
            destinatario, nombre, enlaceActivacion);

        await Task.CompletedTask;
    }
}
