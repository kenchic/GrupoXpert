using GrupoXpert.Application.Perfil.Dtos;
using GrupoXpert.Domain.Perfil;
using MediatR;

namespace GrupoXpert.Application.Perfil.Queries;

public class ObtenerPerfilPorUsuarioHandler(IPerfilClienteRepository perfilRepository) 
    : IRequestHandler<ObtenerPerfilPorUsuarioQuery, PerfilClienteDto?>
{
    public async Task<PerfilClienteDto?> Handle(ObtenerPerfilPorUsuarioQuery request, CancellationToken cancellationToken)
    {
        var perfil = await perfilRepository.ObtenerPorUsuarioIdAsync(request.UsuarioId, cancellationToken);
        
        if (perfil == null) return null;

        return new PerfilClienteDto(
            perfil.Id,
            perfil.UsuarioId,
            (int)perfil.NivelAcademico,
            (int)perfil.UrgenciaEntrega,
            perfil.Telefono?.Numero,
            perfil.AreasInteres.ToList());
    }
}
