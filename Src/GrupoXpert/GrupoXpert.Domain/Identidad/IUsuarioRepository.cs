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
    /// Obtiene un usuario por su correo electrónico (normalizado a minúsculas).
    /// </summary>
    Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancelacion = default);

    /// <summary>
    /// Obtiene un usuario por su token de activación de cuenta.
    /// </summary>
    Task<Usuario?> ObtenerPorTokenActivacionAsync(string token, CancellationToken cancelacion = default);

    /// <summary>
    /// Verifica si un correo electrónico ya está registrado en el sistema.
    /// </summary>
    Task<bool> ExisteEmailAsync(string email, CancellationToken cancelacion = default);

    /// <summary>
    /// Agrega un nuevo usuario al repositorio.
    /// </summary>
    Task AgregarAsync(Usuario usuario, CancellationToken cancelacion = default);

    /// <summary>
    /// Actualiza un usuario existente en el repositorio.
    /// </summary>
    Task ActualizarAsync(Usuario usuario, CancellationToken cancelacion = default);
}
