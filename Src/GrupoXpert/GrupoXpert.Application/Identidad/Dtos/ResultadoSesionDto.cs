namespace GrupoXpert.Application.Identidad.Dtos;

/// <summary>
/// DTO que representa el resultado de un inicio de sesión exitoso.
/// </summary>
public record ResultadoSesionDto(
    string Token,
    string NombreUsuario,
    string Nombre,
    string? Imagen);
