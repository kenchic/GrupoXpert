using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GrupoXpert.Domain.Perfil;
using GrupoXpert.Application.Perfil.Dtos;

namespace GrupoXpert.Application.Perfil.Queries;

public class ObtenerPerfilColaboradorHandler(IPerfilColaboradorRepository perfilColaboradorRepository)
    : IRequestHandler<ObtenerPerfilColaboradorQuery, PerfilColaboradorDto?>
{
    public async Task<PerfilColaboradorDto?> Handle(ObtenerPerfilColaboradorQuery request, CancellationToken cancellationToken)
    {
        var perfil = await perfilColaboradorRepository.ObtenerPorUsuarioIdAsync(request.UsuarioId, cancellationToken);

        if (perfil == null) return null;

        return new PerfilColaboradorDto
        {
            Id = perfil.Id,
            UsuarioId = perfil.UsuarioId,
            ValidadoPorAdmin = perfil.ValidadoPorAdmin,
            NivelAcademico = (int)perfil.NivelAcademico,
            DisponibilidadHorasSemana = perfil.DisponibilidadHorasSemana,
            CargaAcademicaIdeal = perfil.CargaAcademicaIdeal,
            EvaluacionCalidad = perfil.EvaluacionCalidad,
            AreasConocimiento = perfil.AreasConocimiento.ToList(),
            TiposTrabajo = perfil.TiposTrabajo.ToList(),
            Idiomas = perfil.Idiomas.ToList(),
            NormasCitacion = perfil.NormasCitacion.ToList()
        };
    }
}
