using GrupoXpert.Domain.Identidad;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence.Repositorios;

/// <summary>
/// Implementación del repositorio de Usuarios usando EF Core.
/// Todos los campos de email se consultan normalizados a minúsculas.
/// </summary>
public sealed class UsuarioRepository(AppDbContext contexto) : IUsuarioRepository
{
    private readonly AppDbContext _contexto = contexto;

    public async Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default)
    {
        return await _contexto.Usuarios.FindAsync([id], cancellationToken: cancelacion);
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancelacion = default)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        return await _contexto.Usuarios
            .FirstOrDefaultAsync(u => u.Email.Valor == emailNormalizado, cancelacion);
    }

    public async Task<Usuario?> ObtenerPorTokenActivacionAsync(string token, CancellationToken cancelacion = default)
    {
        return await _contexto.Usuarios
            .FirstOrDefaultAsync(u => u.TokenActivacion == token, cancelacion);
    }

    public async Task<bool> ExisteEmailAsync(string email, CancellationToken cancelacion = default)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        return await _contexto.Usuarios
            .AnyAsync(u => u.Email.Valor == emailNormalizado, cancelacion);
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
