namespace GrupoXpert.Domain.Perfil;

public interface IPerfilClienteRepository
{
    Task<PerfilCliente?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PerfilCliente?> ObtenerPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task AgregarAsync(PerfilCliente perfil, CancellationToken cancellationToken = default);
    Task ActualizarAsync(PerfilCliente perfil, CancellationToken cancellationToken = default);
}
