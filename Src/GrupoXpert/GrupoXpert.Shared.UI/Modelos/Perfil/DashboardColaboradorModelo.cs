namespace GrupoXpert.Shared.UI.Modelos.Perfil;

public class DashboardColaboradorModelo
{
    public Guid PerfilColaboradorId { get; set; }
    public Guid UsuarioId { get; set; }
    public int ProyectosEnCurso { get; set; }
    public int EntregasRealizadas { get; set; }
    public int SolicitudesAbiertas { get; set; }
    public int PostulacionesPendientes { get; set; }
    public ReputacionAcademicaModelo Reputacion { get; set; } = new();
    public List<DetalleSolicitudAsesorModelo> Solicitudes { get; set; } = [];
}

public class ReputacionAcademicaModelo
{
    public decimal PuntajePromedio { get; set; }
    public int TotalCalificaciones { get; set; }
    public string Nivel { get; set; } = string.Empty;
}

public class DetalleSolicitudAsesorModelo
{
    public Guid SolicitudId { get; set; }
    public string TipoTrabajo { get; set; } = string.Empty;
    public string AreaTematica { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEntrega { get; set; }
    public bool EsUrgente { get; set; }
    public int NumeroEntregas { get; set; }
    public int? PuntajeCalificacion { get; set; }
}