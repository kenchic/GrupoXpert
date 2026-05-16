using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Perfil;
using MediatR;

namespace GrupoXpert.Application.Perfil.Commands;

public class ActualizarPerfilHandler(
    IPerfilClienteRepository perfilRepository,
    IUnidadDeTrabajo unidadDeTrabajo) 
    : IRequestHandler<ActualizarPerfilCommand, bool>
{
    public async Task<bool> Handle(ActualizarPerfilCommand request, CancellationToken cancellationToken)
    {
        var perfil = await perfilRepository.ObtenerPorUsuarioIdAsync(request.UsuarioId, cancellationToken);
        
        if (perfil == null)
        {
            perfil = PerfilCliente.Crear(request.UsuarioId);
            await perfilRepository.AgregarAsync(perfil, cancellationToken);
        }

        perfil.ActualizarPerfil(
            (NivelAcademico)request.NivelAcademico, 
            (UrgenciaEntrega)request.UrgenciaEntrega, 
            !string.IsNullOrWhiteSpace(request.Telefono) ? Telefono.Crear(request.Telefono) : null);

        // Reemplazo total de áreas de interés
        var areasActuales = perfil.AreasInteres.ToList();
        foreach (var area in areasActuales)
        {
            perfil.RemoverAreaInteres(area);
        }

        foreach (var area in request.AreasInteres)
        {
            perfil.AgregarAreaInteres(area);
        }

        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        
        return true;
    }
}
