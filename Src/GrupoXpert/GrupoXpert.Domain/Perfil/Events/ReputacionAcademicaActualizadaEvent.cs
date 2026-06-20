using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Perfil.Events;

public record ReputacionAcademicaActualizadaEvent(
    Guid PerfilColaboradorId,
    decimal PuntajePromedio,
    int TotalCalificaciones,
    string NivelReputacion) : IDomainEvent
{
    public DateTime FechaOcurrencia { get; } = DateTime.UtcNow;
}