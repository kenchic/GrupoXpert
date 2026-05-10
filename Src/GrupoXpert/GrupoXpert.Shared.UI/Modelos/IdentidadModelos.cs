namespace GrupoXpert.Shared.UI.Modelos;

public enum EstadoVerificacionUI
{
    Pendiente = 1,
    Verificado = 2,
    Rechazado = 3,
    Aprobado = 4,
    EnRevision = 5
}

public enum TipoUsuarioUI
{
    Estudiante = 1,
    Asesor = 2,
    Administrador = 3
}

public class UsuarioListaDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public TipoUsuarioUI Tipo { get; set; } // 1: Estudiante, 2: Asesor, 3: Administrador
    public bool EstaActivo { get; set; }
    public bool EstaAprobado { get; set; }
    public DateTimeOffset FechaCreacion { get; set; }
    public DateTimeOffset? FechaAprobacion { get; set; }
    public EstadoVerificacionUI EstadoVerificacion { get; set; }
}

public class ResultadoPaginadoDto<T>
{
    public IReadOnlyList<T> Elementos { get; set; } = Array.Empty<T>();
    public int TotalRegistros { get; set; }
    public int Pagina { get; set; }
    public int TamanoPagina { get; set; }
}
