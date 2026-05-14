using GrupoXpert.Domain.Perfil;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence.Repositories;

public class PerfilClienteRepository(AppDbContext context) : IPerfilClienteRepository
{
    public async Task<PerfilCliente?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Set<PerfilCliente>()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PerfilCliente?> ObtenerPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await context.Set<PerfilCliente>()
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId, cancellationToken);
    }

    public async Task AgregarAsync(PerfilCliente perfil, CancellationToken cancellationToken = default)
    {
        await context.Set<PerfilCliente>().AddAsync(perfil, cancellationToken);
    }

    public Task ActualizarAsync(PerfilCliente perfil, CancellationToken cancellationToken = default)
    {
        context.Set<PerfilCliente>().Update(perfil);
        return Task.CompletedTask;
    }
}
