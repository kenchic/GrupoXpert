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
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancelacion = default);

    /// <summary>
    /// Obtiene un usuario por su token de activación de cuenta.
    /// </summary>
    Task<Usuario?> ObtenerPorTokenActivacionAsync(string token, CancellationToken cancelacion = default);

    /// <summary>
    /// Verifica si un correo electrónico ya está registrado en el sistema.
    /// </summary>
    Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancelacion = default);

    /// <summary>
    /// Agrega un nuevo usuario al repositorio.
    /// </summary>
    Task AgregarAsync(Usuario usuario, CancellationToken cancelacion = default);

    /// <summary>
    /// Actualiza un usuario existente en el repositorio.
    /// </summary>
    Task ActualizarAsync(Usuario usuario, CancellationToken cancelacion = default);

    /// <summary>
    /// Obtiene una lista paginada de usuarios filtrados por tipo, estado de aprobación y estado de verificación.
    /// </summary>
    Task<(IReadOnlyList<Usuario> Usuarios, int TotalRegistros)> ObtenerPaginadoAsync(
        TipoUsuario? tipo = null,
        bool? estaAprobado = null,
        EstadoVerificacion? estadoVerificacion = null,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancelacion = default);
}
