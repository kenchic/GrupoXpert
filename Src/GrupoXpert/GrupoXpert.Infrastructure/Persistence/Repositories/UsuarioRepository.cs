using GrupoXpert.Domain.Identidad;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de Usuarios usando EF Core.
/// Todos los campos de correo se consultan normalizados a minúsculas.
/// </summary>
public sealed class UsuarioRepository(AppDbContext contexto) : IUsuarioRepository
{
    private readonly AppDbContext _contexto = contexto;

    public async Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default)
    {
        return await _contexto.Usuarios.FindAsync([id], cancellationToken: cancelacion);
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancelacion = default)
    {
        var correoNormalizado = correo.Trim().ToLowerInvariant();
        return await _contexto.Usuarios
            .FirstOrDefaultAsync(u => u.Correo.Valor == correoNormalizado, cancelacion);
    }

    public async Task<Usuario?> ObtenerPorTokenActivacionAsync(string token, CancellationToken cancelacion = default)
    {
        return await _contexto.Usuarios
            .FirstOrDefaultAsync(u => u.TokenActivacion == token, cancelacion);
    }

    public async Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancelacion = default)
    {
        var correoNormalizado = correo.Trim().ToLowerInvariant();
        return await _contexto.Usuarios
            .AnyAsync(u => u.Correo.Valor == correoNormalizado, cancelacion);
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

    public async Task<(IReadOnlyList<Usuario> Usuarios, int TotalRegistros)> ObtenerPaginadoAsync(
        TipoUsuario? tipo = null,
        bool? estaAprobado = null,
        EstadoVerificacion? estadoVerificacion = null,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancelacion = default)
    {
        var consulta = _contexto.Usuarios.AsNoTracking();

        if (tipo.HasValue)
        {
            consulta = consulta.Where(u => u.Tipo == tipo.Value);
        }

        if (estaAprobado.HasValue)
        {
            consulta = consulta.Where(u => u.EstaAprobado == estaAprobado.Value);
        }

        if (estadoVerificacion.HasValue)
        {
            consulta = consulta.Where(u => u.EstadoVerificacion == estadoVerificacion.Value);
        }

        var totalRegistros = await consulta.CountAsync(cancelacion);

        var usuarios = await consulta
            .OrderByDescending(u => u.FechaCreacion)
            .Skip((pagina -1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancelacion);

        return (usuarios, totalRegistros);
    }     
    
}
