using GrupoXpert.Domain.Academia.Events;
using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Academia;

public class SolicitudAcademica : AggregateRoot
{
    public Guid ClienteId { get; private set; }
    public NivelAcademico NivelAcademico { get; private set; }
    public TipoTrabajo TipoTrabajo { get; private set; }
    public string AreaTematica { get; private set; }
    public DateTime FechaEntrega { get; private set; }
    public int NumeroPaginasOPalabras { get; private set; }
    public NormaCitacion NormaCitacion { get; private set; }
    public IdiomaRequerido Idioma { get; private set; }
    public string FormatoRequerido { get; private set; }
    public string MaterialBase { get; private set; }
    public bool EsUrgente { get; private set; }
    public bool EntregaPorFases { get; private set; }

    // Constructor vacío para EF Core
    private SolicitudAcademica() 
    {
        AreaTematica = string.Empty;
        FormatoRequerido = string.Empty;
        MaterialBase = string.Empty;
    }

    private SolicitudAcademica(
        Guid clienteId,
        NivelAcademico nivelAcademico,
        TipoTrabajo tipoTrabajo,
        string areaTematica,
        DateTime fechaEntrega,
        int numeroPaginasOPalabras,
        NormaCitacion normaCitacion,
        IdiomaRequerido idioma,
        string formatoRequerido,
        string materialBase,
        bool esUrgente,
        bool entregaPorFases)
    {
        ClienteId = clienteId;
        NivelAcademico = nivelAcademico;
        TipoTrabajo = tipoTrabajo;
        AreaTematica = areaTematica ?? throw new ArgumentNullException(nameof(areaTematica));
        FechaEntrega = fechaEntrega;
        NumeroPaginasOPalabras = numeroPaginasOPalabras;
        NormaCitacion = normaCitacion;
        Idioma = idioma;
        FormatoRequerido = formatoRequerido ?? throw new ArgumentNullException(nameof(formatoRequerido));
        MaterialBase = materialBase ?? throw new ArgumentNullException(nameof(materialBase));
        EsUrgente = esUrgente;
        EntregaPorFases = entregaPorFases;
    }

    public static SolicitudAcademica Crear(
        Guid clienteId,
        NivelAcademico nivelAcademico,
        TipoTrabajo tipoTrabajo,
        string areaTematica,
        DateTime fechaEntrega,
        int numeroPaginasOPalabras,
        NormaCitacion normaCitacion,
        IdiomaRequerido idioma,
        string formatoRequerido,
        string materialBase,
        bool esUrgente,
        bool entregaPorFases)
    {
        if (fechaEntrega < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("La fecha de entrega no puede ser en el pasado.", nameof(fechaEntrega));
        }

        if (numeroPaginasOPalabras <= 0)
        {
            throw new ArgumentException("El número de páginas o palabras debe ser mayor a cero.", nameof(numeroPaginasOPalabras));
        }

        if (string.IsNullOrWhiteSpace(areaTematica))
        {
            throw new ArgumentException("El área temática no puede estar vacía.", nameof(areaTematica));
        }

        if (clienteId == Guid.Empty)
        {
            throw new ArgumentException("El cliente es obligatorio para realizar la solicitud.", nameof(clienteId));
        }

        var solicitud = new SolicitudAcademica(
            clienteId,
            nivelAcademico,
            tipoTrabajo,
            areaTematica,
            fechaEntrega,
            numeroPaginasOPalabras,
            normaCitacion,
            idioma,
            formatoRequerido,
            materialBase,
            esUrgente,
            entregaPorFases);

        solicitud.AgregarEventoDominio(new SolicitudAcademicaCreadaDomainEvent(solicitud.Id));

        return solicitud;
    }
}
