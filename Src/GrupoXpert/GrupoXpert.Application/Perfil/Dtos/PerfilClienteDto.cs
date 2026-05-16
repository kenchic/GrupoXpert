namespace GrupoXpert.Application.Perfil.Dtos;

public record PerfilClienteDto(
    Guid Id,
    Guid UsuarioId,
    int NivelAcademico,
    int UrgenciaEntrega,
    string? Telefono,
    List<string> AreasInteres);
