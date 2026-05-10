namespace GrupoXpert.Domain.Identidad;

/// <summary>
/// Estados de validación del perfil de un usuario colaborador (Asesor).
/// </summary>
public enum EstadoVerificacion
{
    Pendiente = 1,
    Verificado = 2,
    Rechazado = 3,
    Aprobado = 4,
    EnRevision = 5
}
