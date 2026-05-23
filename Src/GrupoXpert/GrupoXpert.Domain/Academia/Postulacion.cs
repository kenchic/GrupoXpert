using System;
using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Academia;

/// <summary>
/// Representa la postulación de un asesor para resolver una solicitud académica.
/// </summary>
public sealed class Postulacion : Entity
{
    public Guid SolicitudId { get; private set; }
    public Guid ColaboradorId { get; private set; } // Representa el PerfilColaborador.Id
    public DateTime FechaPostulacion { get; private set; }
    public EstadoPostulacion Estado { get; private set; }

    // Constructor requerido para EF Core
    private Postulacion()
    {
        Estado = EstadoPostulacion.Pendiente;
    }

    /// <summary>
    /// Crea una nueva postulación vinculando una solicitud con un colaborador.
    /// </summary>
    public Postulacion(Guid solicitudId, Guid colaboradorId)
    {
        if (solicitudId == Guid.Empty)
        {
            throw new ArgumentException("El ID de la solicitud es requerido.", nameof(solicitudId));
        }

        if (colaboradorId == Guid.Empty)
        {
            throw new ArgumentException("El ID del colaborador es requerido.", nameof(colaboradorId));
        }

        SolicitudId = solicitudId;
        ColaboradorId = colaboradorId;
        FechaPostulacion = DateTime.UtcNow;
        Estado = EstadoPostulacion.Pendiente;
    }

    /// <summary>
    /// Acepta formalmente la postulación del asesor.
    /// </summary>
    public void Aceptar()
    {
        if (Estado != EstadoPostulacion.Pendiente)
        {
            throw new InvalidOperationException("Solo se pueden aceptar postulaciones pendientes.");
        }

        Estado = EstadoPostulacion.Aceptada;
    }

    /// <summary>
    /// Rechaza la postulación del asesor.
    /// </summary>
    public void Rechazar()
    {
        if (Estado != EstadoPostulacion.Pendiente)
        {
            throw new InvalidOperationException("Solo se pueden rechazar postulaciones pendientes.");
        }

        Estado = EstadoPostulacion.Rechazada;
    }
}
