using GrupoXpert.Domain.Identidad;

namespace GrupoXpert.Application.Identidad.Dtos;

public sealed record UsuarioListaDto(
    Guid Id,
    string Nombre,
    string Correo,
    TipoUsuario Tipo,
    bool EstaActivo,
    bool EstaAprobado,
    EstadoVerificacion EstadoVerificacion,
    DateTimeOffset FechaCreacion,
    DateTimeOffset? FechaAprobacion
);
