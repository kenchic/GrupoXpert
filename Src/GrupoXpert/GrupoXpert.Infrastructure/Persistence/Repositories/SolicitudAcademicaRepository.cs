using GrupoXpert.Domain.Academia;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence.Repositories;

public sealed class SolicitudAcademicaRepository(AppDbContext context) : ISolicitudAcademicaRepository
{
    public async Task<SolicitudAcademica?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Set<SolicitudAcademica>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AgregarAsync(SolicitudAcademica solicitud, CancellationToken cancellationToken = default)
    {
        await context.Set<SolicitudAcademica>().AddAsync(solicitud, cancellationToken);
    }

    public async Task ActualizarAsync(SolicitudAcademica solicitud, CancellationToken cancellationToken = default)
    {
        context.Set<SolicitudAcademica>().Update(solicitud);
        await Task.CompletedTask;
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
}
