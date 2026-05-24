using GrupoXpert.Domain.Academia;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence.Repositories;

public sealed class AvanceRepository(AppDbContext context) : IAvanceRepository
{
    public async Task<Avance?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Set<Avance>()
            .Include(x => x.Comentarios)
            .Include(x => x.ArchivosAdjuntos)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AgregarAsync(Avance avance, CancellationToken cancellationToken = default)
    {
        await context.Set<Avance>().AddAsync(avance, cancellationToken);
    }

    public async Task ActualizarAsync(Avance avance, CancellationToken cancellationToken = default)
    {
        var idsComentarios = avance.Comentarios.Select(c => c.Id).ToList();
        if (idsComentarios.Count != 0)
        {
            var idsExistentes = await context.Set<Comentario>()
                .AsNoTracking()
                .Where(c => idsComentarios.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            foreach (var comentario in avance.Comentarios)
            {
                if (!idsExistentes.Contains(comentario.Id))
                    context.Entry(comentario).State = EntityState.Added;
            }
        }

        var idsArchivos = avance.ArchivosAdjuntos.Select(a => a.Id).ToList();
        if (idsArchivos.Count != 0)
        {
            var idsExistentes = await context.Set<ArchivoAdjunto>()
                .AsNoTracking()
                .Where(a => idsArchivos.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync(cancellationToken);

            foreach (var archivo in avance.ArchivosAdjuntos)
            {
                if (!idsExistentes.Contains(archivo.Id))
                    context.Entry(archivo).State = EntityState.Added;
            }
        }
    }

    public async Task<IReadOnlyList<Avance>> ObtenerPorSolicitudAsync(Guid solicitudId, CancellationToken cancellationToken = default)
    {
        return await context.Set<Avance>()
            .Include(x => x.Comentarios)
            .Include(x => x.ArchivosAdjuntos)
            .Where(x => x.SolicitudId == solicitudId)
            .OrderBy(x => x.FechaSubida)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Avance>> ObtenerPorAsesorAsync(Guid asesorId, CancellationToken cancellationToken = default)
    {
        return await context.Set<Avance>()
            .Where(x => x.AsesorId == asesorId)
            .OrderByDescending(x => x.FechaSubida)
            .ToListAsync(cancellationToken);
    }
}
