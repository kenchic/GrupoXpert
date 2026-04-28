namespace GrupoXpert.Domain.Identidad;

/// <summary>
/// Interfaz del repositorio para el agregado Usuario.
/// Define las operaciones de persistencia que la capa de Infraestructura debe implementar.
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>
    /// Obtiene un usuario por su identificador único.
    /// </summary>
    Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default);

    /// <summary>
    /// Obtiene un usuario por su nombre de usuario (normalizado a minúsculas).
    /// </summary>
    Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken cancelacion = default);

    /// <summary>
    /// Verifica si un nombre de usuario ya está registrado.
    /// </summary>
    Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, CancellationToken cancelacion = default);

    /// <summary>
    /// Agrega un nuevo usuario al repositorio.
    /// </summary>
    Task AgregarAsync(Usuario usuario, CancellationToken cancelacion = default);

    /// <summary>
    /// Actualiza un usuario existente en el repositorio.
    /// </summary>
    Task ActualizarAsync(Usuario usuario, CancellationToken cancelacion = default);
}

