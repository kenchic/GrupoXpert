using Microsoft.Extensions.DependencyInjection;

namespace GrupoXpert.Application;

/// <summary>
/// Clase estática para registrar los servicios de la capa de Aplicación.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection servicios)
    {
        // Registrar MediatR de este ensamblado
        servicios.AddMediatR(configuracion =>
        {
            configuracion.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        return servicios;
    }
}

