using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

/// <summary>
/// Comando para registrar un nuevo usuario en el sistema.
/// El usuario quedará inactivo hasta confirmar su correo electrónico.
/// </summary>
/// <param name="Email">Correo electrónico único del usuario (será su identificador de login).</param>
/// <param name="Clave">Clave en texto plano (se hasheará en el handler).</param>
/// <param name="Nombre">Nombre completo del usuario.</param>
/// <param name="Imagen">URL o ruta de imagen de perfil (opcional).</param>
public sealed record CrearUsuarioCommand(
    string Email,
    string Clave,
    string Nombre,
    string? Imagen = null,
    bool ActivacionAutomatica = false) : IRequest<Guid>;
