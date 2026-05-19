using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GrupoXpert.Domain.Perfil;
using GrupoXpert.Application.Common.Interfaces;

namespace GrupoXpert.Application.Perfil.Commands;

public class ActualizarPerfilColaboradorHandler(
    IPerfilColaboradorRepository perfilColaboradorRepository,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<ActualizarPerfilColaboradorCommand, Unit>
{
    public async Task<Unit> Handle(ActualizarPerfilColaboradorCommand request, CancellationToken cancellationToken)
    {
        var perfil = await perfilColaboradorRepository.ObtenerPorUsuarioIdAsync(request.UsuarioId, cancellationToken);

        if (perfil == null)
        {
            perfil = PerfilColaborador.Crear(request.UsuarioId);
            await perfilColaboradorRepository.AgregarAsync(perfil, cancellationToken);
        }

        perfil.ActualizarPerfil(
            (NivelAcademico)request.NivelAcademico,
            request.DisponibilidadHorasSemana,
            request.CargaAcademicaIdeal);

        // Actualizar Áreas de Conocimiento
        var areasActuales = perfil.AreasConocimiento.ToList();
        var areasNuevas = request.AreasConocimiento ?? new();
        
        foreach (var area in areasActuales.Except(areasNuevas))
            perfil.RemoverAreaConocimiento(area);
            
        foreach (var area in areasNuevas.Except(areasActuales))
            perfil.AgregarAreaConocimiento(area);

        // Actualizar Tipos de Trabajo
        var tiposActuales = perfil.TiposTrabajo.ToList();
        var tiposNuevos = request.TiposTrabajo ?? new();

        foreach (var tipo in tiposActuales.Except(tiposNuevos))
            perfil.RemoverTipoTrabajo(tipo);

        foreach (var tipo in tiposNuevos.Except(tiposActuales))
            perfil.AgregarTipoTrabajo(tipo);

        // Actualizar Idiomas
        var idiomasActuales = perfil.Idiomas.ToList();
        var idiomasNuevos = request.Idiomas ?? new();

        foreach (var idioma in idiomasActuales.Except(idiomasNuevos))
            perfil.RemoverIdioma(idioma);

        foreach (var idioma in idiomasNuevos.Except(idiomasActuales))
            perfil.AgregarIdioma(idioma);

        // Actualizar Normas de Citación
        var normasActuales = perfil.NormasCitacion.ToList();
        var normasNuevas = request.NormasCitacion ?? new();

        foreach (var norma in normasActuales.Except(normasNuevas))
            perfil.RemoverNormaCitacion(norma);

        foreach (var norma in normasNuevas.Except(normasActuales))
            perfil.AgregarNormaCitacion(norma);

        await perfilColaboradorRepository.ActualizarAsync(perfil, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);

        return Unit.Value;
    }
}
