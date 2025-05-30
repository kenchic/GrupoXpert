using System.ComponentModel.DataAnnotations;

namespace GrupoXpert.Blazor.Models
{
    public class CotizacionPago
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El ID de la solicitud es requerido")]
        public int SolicitudId { get; set; }
        
        [Required(ErrorMessage = "El tipo de trabajo es requerido")]
        public string TipoTrabajo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El nivel académico es requerido")]
        public string NivelAcademico { get; set; } = string.Empty;
        
        [Range(1, int.MaxValue, ErrorMessage = "El número de páginas debe ser mayor a 0")]
        public int NumeroPaginas { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "El número de palabras debe ser mayor a 0")]
        public int NumeroWords { get; set; }
        
        public DateTime FechaEntrega { get; set; }
        
        public bool EsUrgente { get; set; }
        
        public bool EntregaPorFases { get; set; }
        
        // Cálculos de costos
        public decimal CostoPorPagina { get; set; }
        public decimal CostoBase { get; set; }
        public decimal CostoUrgencia { get; set; }
        public decimal CostoFases { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        
        // Opciones de pago
        [Required(ErrorMessage = "Debe seleccionar una modalidad de pago")]
        public ModalidadPago ModalidadPago { get; set; }
        
        public List<DetalleFase> FasesPago { get; set; } = new List<DetalleFase>();
        
        // Información de pago
        public string? MetodoPago { get; set; }
        public string? PlataformaPago { get; set; }
        public string? TransactionId { get; set; }
        public EstadoPago EstadoPago { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaPago { get; set; }
    }
    
    public class DetalleFase
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public int Porcentaje { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public EstadoPago Estado { get; set; }
        public string? TransactionId { get; set; }
    }
    
    public enum ModalidadPago
    {
        PagoUnico,
        PagoPorFases
    }
    
    public enum EstadoPago
    {
        Pendiente,
        Procesando,
        Completado,
        Fallido,
        Cancelado
    }
    
    public static class TarifasAcademicas
    {
        public static readonly Dictionary<string, decimal> CostoPorNivel = new()
        {
            { "Secundaria", 8.00m },
            { "Preparatoria", 10.00m },
            { "Universidad", 12.00m },
            { "Maestría", 15.00m },
            { "Doctorado", 20.00m }
        };
        
        public static readonly Dictionary<string, decimal> MultiplicadorTipo = new()
        {
            { "Ensayo", 1.0m },
            { "Investigación", 1.2m },
            { "Tesis", 1.5m },
            { "Proyecto", 1.3m },
            { "Reporte", 0.9m },
            { "Presentación", 0.8m }
        };
        
        public const decimal DescuentoPagoUnico = 0.15m; // 15% descuento
        public const decimal RecargoUrgencia = 0.50m; // 50% recargo
        public const decimal RecargoFases = 0.10m; // 10% recargo
    }
    
    public static class MetodosPago
    {
        public static readonly List<string> Disponibles = new()
        {
            "Tarjeta de Crédito",
            "Tarjeta de Débito",
            "PayPal",
            "Transferencia Bancaria",
            "Mercado Pago",
            "Stripe"
        };
    }
    
    public static class FasesEstandar
    {
        public static readonly List<DetalleFase> FasesDefault = new()
        {
            new DetalleFase
            {
                Nombre = "Avance",
                Descripcion = "Primer avance del trabajo (30%)",
                Porcentaje = 30,
                Estado = EstadoPago.Pendiente
            },
            new DetalleFase
            {
                Nombre = "Borrador",
                Descripcion = "Borrador completo del trabajo (40%)",
                Porcentaje = 40,
                Estado = EstadoPago.Pendiente
            },
            new DetalleFase
            {
                Nombre = "Final",
                Descripcion = "Entrega final del trabajo (30%)",
                Porcentaje = 30,
                Estado = EstadoPago.Pendiente
            }
        };
    }
}