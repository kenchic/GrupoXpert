using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrupoXpert.Domain.Perfil;

public interface IPerfilColaboradorRepository
{
    Task<PerfilColaborador?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PerfilColaborador?> ObtenerPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task AgregarAsync(PerfilColaborador perfil, CancellationToken cancellationToken = default);
    Task ActualizarAsync(PerfilColaborador perfil, CancellationToken cancellationToken = default);
}
