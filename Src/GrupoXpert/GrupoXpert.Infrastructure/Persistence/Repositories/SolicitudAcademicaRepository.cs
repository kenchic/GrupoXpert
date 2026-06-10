using GrupoXpert.Domain.Academia;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence.Repositories;

public sealed class SolicitudAcademicaRepository(AppDbContext context) : ISolicitudAcademicaRepository
{
    public async Task<SolicitudAcademica?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Set<SolicitudAcademica>()
            .Include(x => x.Postulaciones)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AgregarAsync(SolicitudAcademica solicitud, CancellationToken cancellationToken = default)
    {
        await context.Set<SolicitudAcademica>().AddAsync(solicitud, cancellationToken);
    }

    public async Task ActualizarAsync(SolicitudAcademica solicitud, CancellationToken cancellationToken = default)
    {
        // En EF Core, las entidades recuperadas y modificadas dentro de la misma Unidad de Trabajo
        // son rastreadas automáticamente por el ChangeTracker. Llamar a .Update(solicitud) en una entidad
        // ya rastreada puede cambiar erróneamente el estado de sus colecciones dependientes a 'Modified',
        // provocando DbUpdateConcurrencyException al intentar ejecutar UPDATEs en filas que aún no existen
        // o que deben ser insertadas/eliminadas de forma normal.
        //
        // Sin embargo, cuando se agrega una nueva entidad hija (ej. Postulacion) a una colección de navegación
        // de una entidad ya rastreada, EF Core puede marcarla como 'Modified' en lugar de 'Added'
        // porque detecta una PK asignada (Guid.NewGuid()) y no sabe que es una entidad nueva.
        // Por eso, forzamos explícitamente el estado 'Added' para las postulaciones que no existen en BD.
        var idsPostulaciones = solicitud.Postulaciones.Select(p => p.Id).ToList();
        if (idsPostulaciones.Count != 0)
        {
            var idsExistentes = await context.Set<Postulacion>()
                .AsNoTracking()
                .Where(p => idsPostulaciones.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            foreach (var postulacion in solicitud.Postulaciones)
            {
                if (!idsExistentes.Contains(postulacion.Id))
                {
                    context.Entry(postulacion).State = EntityState.Added;
                }
            }
        }
    }

    public async Task EliminarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var solicitud = await ObtenerPorIdAsync(id, cancellationToken);
        if (solicitud is null) return;
        
        context.Set<SolicitudAcademica>().Remove(solicitud);
    }

    public async Task<IReadOnlyList<SolicitudAcademica>> ObtenerPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await context.Set<SolicitudAcademica>()
            .Where(x => x.ClienteId == clienteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SolicitudAcademica>> ObtenerPendientesYAsignadasAAsesorAsync(Guid asesorId, CancellationToken cancellationToken = default)
    {
        return await context.Set<SolicitudAcademica>()
            .Where(x => x.AsesorId == asesorId || (x.Estado == EstadoSolicitud.Pendiente && x.AsesorId == null))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SolicitudAcademica>> ObtenerPorEstadosAsync(IEnumerable<EstadoSolicitud> estados, CancellationToken cancellationToken = default)
    {
        return await context.Set<SolicitudAcademica>()
            .Where(x => estados.Contains(x.Estado))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SolicitudAcademica>> ObtenerConAvanceFinalPendienteRevisionAsync(CancellationToken cancellationToken = default)
    {
        var solicitudIds = await context.Set<Avance>()
            .Where(a => a.Tipo == TipoAvance.Final && a.Estado == EstadoAvance.PendienteRevision)
            .Select(a => a.SolicitudId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await context.Set<SolicitudAcademica>()
            .Where(s => solicitudIds.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SolicitudAcademica>> ObtenerEnLiberacionPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await context.Set<SolicitudAcademica>()
            .Include(x => x.Postulaciones)
            .Where(x => x.ClienteId == clienteId && x.Estado == EstadoSolicitud.Liberacion)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SolicitudAcademica>> ObtenerEnLiberacionAsync(CancellationToken cancellationToken = default)
    {
        return await context.Set<SolicitudAcademica>()
            .Include(x => x.Postulaciones)
            .Where(x => x.Estado == EstadoSolicitud.Liberacion)
            .ToListAsync(cancellationToken);
    }
}
