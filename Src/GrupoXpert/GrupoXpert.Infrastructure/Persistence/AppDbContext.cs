using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Perfil;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence;

/// <summary>
/// Contexto de base de datos para GrupoXpert.
/// Implementa IUnidadDeTrabajo para coordinar transacciones.
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options), IUnidadDeTrabajo
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PerfilCliente> PerfilesCliente => Set<PerfilCliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Identidad");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    public async Task<int> GuardarCambiosAsync(CancellationToken cancelacion = default)
    {
        // Aquí se podrían publicar eventos de dominio antes o después de guardar
        return await SaveChangesAsync(cancelacion);
    }
}

