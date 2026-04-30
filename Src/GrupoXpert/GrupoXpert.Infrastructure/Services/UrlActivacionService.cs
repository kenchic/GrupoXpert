using GrupoXpert.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GrupoXpert.Infrastructure.Services;

/// <summary>
/// Implementación del servicio para construir URLs de activación.
/// Obtiene la base de la URL desde la configuración de la aplicación (FrontendUrl).
/// </summary>
public sealed class UrlActivacionService(IConfiguration configuracion) : IUrlActivacionService
{
    private readonly IConfiguration _configuracion = configuracion;

    public string Construir(string token)
    {
        var urlBase = _configuracion["Frontend:UrlActivacion"] 
                      ?? "https://localhost:7117/activar-cuenta";
                      
        // Aseguramos que la URL tenga el formato correcto para el query string
        var separador = urlBase.Contains('?') ? "&" : "?";
        
        return $"{urlBase}{separador}token={token}";
    }
}
