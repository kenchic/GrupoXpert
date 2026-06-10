namespace GrupoXpert.Domain.Academia;

/// <summary>
/// Representa los estados del ciclo de vida de una solicitud académica.
/// </summary>
public enum EstadoSolicitud
{
    Pendiente = 1,
    EnProceso = 2,
    Completada = 3,
    Cancelada = 4,
    Asignada = 5,
    Liberacion = 6
}
