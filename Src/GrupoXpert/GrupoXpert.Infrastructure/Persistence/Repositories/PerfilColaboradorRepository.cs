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
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PerfilColaborador?> ObtenerPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await context.Set<PerfilColaborador>()
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId, cancellationToken);
    }

    public async Task AgregarAsync(PerfilColaborador perfil, CancellationToken cancellationToken = default)
    {
        await context.Set<PerfilColaborador>().AddAsync(perfil, cancellationToken);
    }

    public Task ActualizarAsync(PerfilColaborador perfil, CancellationToken cancellationToken = default)
    {
        context.Set<PerfilColaborador>().Update(perfil);
        return Task.CompletedTask;
    }
}
