using MediatR;

namespace GrupoXpert.Application.Perfil.Commands;

public record ActualizarPerfilCommand(
    Guid UsuarioId,
    int NivelAcademico,
    int UrgenciaEntrega,
    string? Telefono,
    List<string> AreasInteres) : IRequest<bool>;
