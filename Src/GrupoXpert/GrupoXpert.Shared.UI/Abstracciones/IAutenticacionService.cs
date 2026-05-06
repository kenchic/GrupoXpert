using GrupoXpert.Shared.UI.Componentes.Identidad;

namespace GrupoXpert.Shared.UI.Abstracciones;

/// <summary>
/// Interfaz para el servicio de autenticación y gestión de identidad en el frontend.
/// </summary>
public interface IAutenticacionService
{
    /// <summary>
    /// Envía una solicitud de registro de nuevo usuario al backend.
    /// </summary>
    Task RegistrarAsync(FormularioRegistro.ModeloRegistro modelo);

    /// <summary>
    /// Envía una solicitud de inicio de sesión.
    /// </summary>
    Task<string> IniciarSesionAsync(string correo, string clave);

    /// <summary>
    /// Activa una cuenta usando el token.
    /// </summary>
    Task ActivarCuentaAsync(string token);
}
