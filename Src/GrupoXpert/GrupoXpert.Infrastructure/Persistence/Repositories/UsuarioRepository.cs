using GrupoXpert.Domain.Identidad;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence.Repositorios;

/// <summary>
/// Implementación del repositorio de Usuarios usando EF Core.
/// </summary>
public sealed class UsuarioRepository(AppDbContext contexto) : IUsuarioRepository
{
    private readonly AppDbContext _contexto = contexto;

    public async Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default)
    {
        return await _contexto.Usuarios.FindAsync([id], cancellationToken: cancelacion);
    }

    public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken cancelacion = default)
    {
        var nombreNormalizado = nombreUsuario.Trim().ToLowerInvariant();
        return await _contexto.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreNormalizado, cancelacion);
    }

    public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, CancellationToken cancelacion = default)
    {
        var nombreNormalizado = nombreUsuario.Trim().ToLowerInvariant();
        return await _contexto.Usuarios
            .AnyAsync(u => u.NombreUsuario == nombreNormalizado, cancelacion);
    }

    public async Task AgregarAsync(Usuario usuario, CancellationToken cancelacion = default)
    {
        await _contexto.Usuarios.AddAsync(usuario, cancelacion);
    }

    public async Task ActualizarAsync(Usuario usuario, CancellationToken cancelacion = default)
    {
        _contexto.Usuarios.Update(usuario);
        await Task.CompletedTask;
    }
}

