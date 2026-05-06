using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Infrastructure.Persistence;
using GrupoXpert.Infrastructure.Persistence.Repositorios;
using GrupoXpert.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrupoXpert.Infrastructure;

/// <summary>
/// Clase estática para registrar los servicios de la capa de Infraestructura.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection servicios, 
        IConfiguration configuracion)
    {
        // 1. Persistencia (EF Core)
        var cadenaConexion = configuracion.GetConnectionString("CadenaConexion");
        
        servicios.AddDbContext<AppDbContext>(opciones =>
            opciones.UseSqlServer(cadenaConexion));

        servicios.AddScoped<IUnidadDeTrabajo>(proveedor => 
            proveedor.GetRequiredService<AppDbContext>());

        // 2. Repositorios
        servicios.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // 3. Servicios
        servicios.AddSingleton<IHashClaveService, HashClaveService>();
        servicios.AddScoped<ITokenAccesoService, TokenService>();
        servicios.AddScoped<IGeneradorTokenService, GeneradorTokenService>();
        servicios.AddScoped<IUrlActivacionService, UrlActivacionService>();
        servicios.AddScoped<ICorreoElectronicoService, CorreoElectronicoService>();

        return servicios;
    }
}

