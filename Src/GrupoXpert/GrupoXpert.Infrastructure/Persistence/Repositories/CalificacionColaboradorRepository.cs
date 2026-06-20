using GrupoXpert.Domain.Calificacion;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence.Repositories;

public sealed class CalificacionColaboradorRepository(AppDbContext context) : ICalificacionColaboradorRepository
{
    public async Task<CalificacionColaborador?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Set<CalificacionColaborador>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CalificacionColaborador?> ObtenerPorSolicitudAsync(Guid solicitudId, CancellationToken cancellationToken = default)
    {
        return await context.Set<CalificacionColaborador>()
            .FirstOrDefaultAsync(x => x.SolicitudId == solicitudId, cancellationToken);
    }

    public async Task AgregarAsync(CalificacionColaborador calificacion, CancellationToken cancellationToken = default)
    {
        await context.Set<CalificacionColaborador>().AddAsync(calificacion, cancellationToken);
    }

    public async Task ActualizarAsync(CalificacionColaborador calificacion, CancellationToken cancellationToken = default)
    {
        context.Set<CalificacionColaborador>().Update(calificacion);
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<CalificacionColaborador>> ObtenerPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await context.Set<CalificacionColaborador>()
            .Where(x => x.ClienteId == clienteId)
            .OrderByDescending(x => x.FechaCalificacion)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CalificacionColaborador>> ObtenerPorColaboradorAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        return await context.Set<CalificacionColaborador>()
            .Where(x => x.ColaboradorId == colaboradorId)
            .OrderByDescending(x => x.FechaCalificacion)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CalificacionColaborador>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        return await context.Set<CalificacionColaborador>()
            .OrderByDescending(x => x.FechaCalificacion)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> ObtenerPuntajePromedioPorColaboradorAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var calificaciones = await context.Set<CalificacionColaborador>()
            .Where(x => x.ColaboradorId == colaboradorId)
            .Select(x => x.Puntaje.Valor)
            .ToListAsync(cancellationToken);

        if (calificaciones.Count == 0)
            return 0m;

        return Math.Round((decimal)calificaciones.Average(), 2);
    }

    public async Task<int> ContarCalificacionesPorColaboradorAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        return await context.Set<CalificacionColaborador>()
            .Where(x => x.ColaboradorId == colaboradorId)
            .CountAsync(cancellationToken);
    }
}