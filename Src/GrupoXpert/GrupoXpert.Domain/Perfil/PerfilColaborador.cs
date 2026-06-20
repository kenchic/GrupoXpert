using System;
using System.Collections.Generic;
using System.Linq;
using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Perfil.Events;

namespace GrupoXpert.Domain.Perfil;

public class PerfilColaborador : AggregateRoot
{
    public Guid UsuarioId { get; private set; }
    public bool ValidadoPorAdmin { get; private set; }
    public NivelAcademico NivelAcademico { get; private set; }
    public int DisponibilidadHorasSemana { get; private set; }
    public int CargaAcademicaIdeal { get; private set; }
    public decimal EvaluacionCalidad { get; private set; }
    public ReputacionAcademica Reputacion { get; private set; }

    private readonly List<AreaConocimiento> _areasConocimiento = new();
    public IReadOnlyList<string> AreasConocimiento => _areasConocimiento.Select(x => x.Area).ToList().AsReadOnly();

    private readonly List<TipoTrabajo> _tiposTrabajo = new();
    public IReadOnlyList<string> TiposTrabajo => _tiposTrabajo.Select(x => x.Tipo).ToList().AsReadOnly();

    private readonly List<Idioma> _idiomas = new();
    public IReadOnlyList<string> Idiomas => _idiomas.Select(x => x.Nombre).ToList().AsReadOnly();

    private readonly List<NormaCitacion> _normasCitacion = new();
    public IReadOnlyList<string> NormasCitacion => _normasCitacion.Select(x => x.Norma).ToList().AsReadOnly();

    private PerfilColaborador() { EvaluacionCalidad = default; Reputacion = default!; }

    private PerfilColaborador(Guid id, Guid usuarioId) : base(id)
    {
        UsuarioId = usuarioId;
        ValidadoPorAdmin = false;
        NivelAcademico = NivelAcademico.NoEspecificado;
        DisponibilidadHorasSemana = 0;
        CargaAcademicaIdeal = 0;
        EvaluacionCalidad = 0m;
        Reputacion = ReputacionAcademica.SinCalificaciones;
    }

    public static PerfilColaborador Crear(Guid usuarioId)
    {
        return new PerfilColaborador(Guid.NewGuid(), usuarioId);
    }

    public void ActualizarPerfil(
        NivelAcademico nivelAcademico, 
        int disponibilidadHorasSemana, 
        int cargaAcademicaIdeal)
    {
        if (disponibilidadHorasSemana < 0)
            throw new ArgumentException("La disponibilidad no puede ser negativa.");
            
        if (cargaAcademicaIdeal < 0 || cargaAcademicaIdeal > disponibilidadHorasSemana)
            throw new ArgumentException("La carga ideal debe ser positiva y no mayor a la disponibilidad total.");

        NivelAcademico = nivelAcademico;
        DisponibilidadHorasSemana = disponibilidadHorasSemana;
        CargaAcademicaIdeal = cargaAcademicaIdeal;

        AgregarEventoDominio(new PerfilColaboradorActualizadoEvent(Id));
    }

    public void ValidarPerfil()
    {
        if (!ValidadoPorAdmin)
        {
            ValidadoPorAdmin = true;
            AgregarEventoDominio(new PerfilColaboradorValidadoEvent(Id));
        }
    }
    
    public void ActualizarEvaluacionCalidad(decimal nuevaEvaluacion)
    {
        if (nuevaEvaluacion < 0 || nuevaEvaluacion > 5)
            throw new ArgumentException("La evaluación debe estar entre 0 y 5.");
            
        EvaluacionCalidad = nuevaEvaluacion;
        AgregarEventoDominio(new PerfilColaboradorActualizadoEvent(Id));
    }

    public void ActualizarReputacion(ReputacionAcademica nuevaReputacion)
    {
        if (nuevaReputacion is null)
            throw new ArgumentException("La reputación académica no puede ser nula.", nameof(nuevaReputacion));

        Reputacion = nuevaReputacion;

        AgregarEventoDominio(new ReputacionAcademicaActualizadaEvent(
            Id,
            nuevaReputacion.PuntajePromedio,
            nuevaReputacion.TotalCalificaciones,
            nuevaReputacion.Nivel.ToString()));
    }

    public void AgregarAreaConocimiento(string area)
    {
        if (!_areasConocimiento.Any(x => x.Area.Equals(area, StringComparison.OrdinalIgnoreCase)))
            _areasConocimiento.Add(new AreaConocimiento(area));
    }

    public void RemoverAreaConocimiento(string area)
    {
        var item = _areasConocimiento.FirstOrDefault(x => x.Area.Equals(area, StringComparison.OrdinalIgnoreCase));
        if (item != null) _areasConocimiento.Remove(item);
    }

    public void AgregarTipoTrabajo(string tipo)
    {
        if (!_tiposTrabajo.Any(x => x.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase)))
            _tiposTrabajo.Add(new TipoTrabajo(tipo));
    }

    public void RemoverTipoTrabajo(string tipo)
    {
        var item = _tiposTrabajo.FirstOrDefault(x => x.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase));
        if (item != null) _tiposTrabajo.Remove(item);
    }

    public void AgregarIdioma(string idioma)
    {
        if (!_idiomas.Any(x => x.Nombre.Equals(idioma, StringComparison.OrdinalIgnoreCase)))
            _idiomas.Add(new Idioma(idioma));
    }

    public void RemoverIdioma(string idioma)
    {
        var item = _idiomas.FirstOrDefault(x => x.Nombre.Equals(idioma, StringComparison.OrdinalIgnoreCase));
        if (item != null) _idiomas.Remove(item);
    }

    public void AgregarNormaCitacion(string norma)
    {
        if (!_normasCitacion.Any(x => x.Norma.Equals(norma, StringComparison.OrdinalIgnoreCase)))
            _normasCitacion.Add(new NormaCitacion(norma));
    }

    public void RemoverNormaCitacion(string norma)
    {
        var item = _normasCitacion.FirstOrDefault(x => x.Norma.Equals(norma, StringComparison.OrdinalIgnoreCase));
        if (item != null) _normasCitacion.Remove(item);
    }
}
