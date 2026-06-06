using GrupoXpert.Domain.Calidad;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence.Repositories;

public sealed class RevisionCalidadRepository(AppDbContext context) : IRevisionCalidadRepository
{
    public async Task<RevisionCalidad?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Set<RevisionCalidad>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<RevisionCalidad?> ObtenerPorAvanceAsync(Guid avanceId, CancellationToken cancellationToken = default)
    {
        return await context.Set<RevisionCalidad>()
            .FirstOrDefaultAsync(x => x.AvanceId == avanceId, cancellationToken);
    }

    public async Task AgregarAsync(RevisionCalidad revision, CancellationToken cancellationToken = default)
    {
        await context.Set<RevisionCalidad>().AddAsync(revision, cancellationToken);
    }

    public async Task ActualizarAsync(RevisionCalidad revision, CancellationToken cancellationToken = default)
    {
        context.Set<RevisionCalidad>().Update(revision);
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<RevisionCalidad>> ObtenerPorRevisorAsync(Guid revisorId, CancellationToken cancellationToken = default)
    {
        return await context.Set<RevisionCalidad>()
            .Where(x => x.RevisorId == revisorId)
            .OrderByDescending(x => x.FechaRevision)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RevisionCalidad>> ObtenerPendientesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Set<RevisionCalidad>()
            .Where(x => x.Estado == EstadoRevisionCalidad.Pendiente)
            .OrderByDescending(x => x.FechaRevision)
            .ToListAsync(cancellationToken);
    }
}
