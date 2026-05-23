using System;
using System.Threading;
using System.Threading.Tasks;
using GrupoXpert.Domain.Perfil;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence.Repositories;

public class PerfilColaboradorRepository(AppDbContext context) : IPerfilColaboradorRepository
{
    public async Task<PerfilColaborador?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Set<PerfilColaborador>()
            .Include("_areasConocimiento")
            .Include("_tiposTrabajo")
            .Include("_idiomas")
            .Include("_normasCitacion")
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PerfilColaborador?> ObtenerPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await context.Set<PerfilColaborador>()
            .Include("_areasConocimiento")
            .Include("_tiposTrabajo")
            .Include("_idiomas")
            .Include("_normasCitacion")
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId, cancellationToken);
    }

    public async Task AgregarAsync(PerfilColaborador perfil, CancellationToken cancellationToken = default)
    {
        await context.Set<PerfilColaborador>().AddAsync(perfil, cancellationToken);
    }

    public Task ActualizarAsync(PerfilColaborador perfil, CancellationToken cancellationToken = default)
    {
        // En EF Core, las entidades recuperadas y modificadas dentro de la misma Unidad de Trabajo
        // son rastreadas automáticamente por el ChangeTracker. Llamar a .Update(perfil) en una entidad
        // ya rastreada cambia erróneamente el estado de sus colecciones dependientes (Owned Collections)
        // a 'Modified', forzando a EF a ejecutar UPDATEs en filas que aún no existen (o que deben ser
        // insertadas/eliminadas de forma normal), provocando DbUpdateConcurrencyException.
        // Por tanto, dejamos que el ChangeTracker de EF detecte automáticamente los cambios al guardar.
        return Task.CompletedTask;
    }
}
