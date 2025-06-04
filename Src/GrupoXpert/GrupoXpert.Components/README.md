# GrupoXpert Components

Este proyecto contiene componentes reutilizables de Blazor para la plataforma GrupoXpert, diseñados para funcionar tanto en aplicaciones Web como MAUI.

## Componentes Disponibles

### 1. SolicitudAcademicaForm

Componente para crear solicitudes de trabajos académicos con funcionalidades avanzadas.

#### Características:
- Formulario completo para solicitudes académicas
- Validación en tiempo real
- Carga de archivos con drag & drop
- Interfaz responsive y moderna
- Notificaciones integradas

#### Uso:
```razor
<SolicitudAcademicaForm OnSolicitudEnviada="@HandleSolicitudEnviada" />
```

#### Propiedades:
- `OnSolicitudEnviada`: Callback que se ejecuta cuando se envía la solicitud
- `SolicitudInicial`: Solicitud preconfigurada (opcional)

#### Modelo de Datos:
```csharp
public class SolicitudAcademica
{
    public string NivelAcademico { get; set; }
    public string TipoTrabajo { get; set; }
    public string AreaConocimiento { get; set; }
    public string NormaCitacion { get; set; }
    public string Tema { get; set; }
    public int NumeroPaginas { get; set; }
    public int NumeroWords { get; set; }
    public DateTime FechaEntrega { get; set; }
    public bool EsUrgente { get; set; }
    public bool EntregaPorFases { get; set; }
    public string InstruccionesAdicionales { get; set; }
    public List<ArchivoAdjunto> ArchivosAdjuntos { get; set; }
}
```

### 2. CotizacionPagoForm

Componente para cotización automática y procesamiento de pagos con múltiples modalidades.

#### Características:
- Cotización automática en tiempo real
- Cálculo dinámico de costos
- Modalidades de pago flexibles (único con descuento, por fases)
- Integración con múltiples plataformas de pago
- Visualización detallada de costos
- Cronograma de pagos por fases
- Interfaz moderna y responsive

#### Uso:
```razor
<CotizacionPagoForm OnCotizacionGenerada="@HandleCotizacionGenerada"
                   OnPagoIniciado="@HandlePagoIniciado"
                   OnCancelado="@HandleCancelado"
                   CotizacionInicial="@cotizacionActual" />
```

#### Propiedades:
- `OnCotizacionGenerada`: Callback ejecutado cuando se actualiza la cotización
- `OnPagoIniciado`: Callback ejecutado cuando se inicia el proceso de pago
- `OnCancelado`: Callback ejecutado cuando se cancela la operación
- `CotizacionInicial`: Cotización preconfigurada (opcional)

#### Modelo de Datos:
```csharp
public class CotizacionPago
{
    // Información del trabajo
    public string SolicitudId { get; set; }
    public string TipoTrabajo { get; set; }
    public string NivelAcademico { get; set; }
    public int NumeroPaginas { get; set; }
    public int NumeroWords { get; set; }
    public DateTime FechaEntrega { get; set; }
    public bool EsUrgente { get; set; }
    public bool EntregaPorFases { get; set; }
    
    // Cálculos de costo
    public decimal CostoPorPagina { get; set; }
    public decimal CostoBase { get; set; }
    public decimal Subtotal { get; set; }
    public decimal RecargoUrgencia { get; set; }
    public decimal RecargoFases { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    
    // Opciones de pago
    public ModalidadPago ModalidadPago { get; set; }
    public List<DetalleFase> FasesPago { get; set; }
    public string MetodoPago { get; set; }
    public string PlataformaPago { get; set; }
    
    // Estado del pago
    public EstadoPago EstadoPago { get; set; }
    public string TransactionId { get; set; }
    public DateTime FechaPago { get; set; }
}
```

#### Modalidades de Pago:
- **Pago Único**: 15% de descuento sobre el total
- **Pago por Fases**: Distribución del pago según el cronograma del trabajo
  - Avance (40%)
  - Borrador (35%)
  - Final (20%)
  - Corrección (5%)

#### Plataformas de Pago Soportadas:
- PayPal
- Stripe
- Mercado Pago
- Transferencia Bancaria
- Tarjeta de Crédito/Débito

### 3. AsignacionSolicitudesForm

Componente dashboard para la gestión y asignación de solicitudes académicas a colaboradores.

#### Características:
- Dashboard completo con estadísticas en tiempo real
- Filtros avanzados por estado, nivel académico, colaborador y búsqueda
- Visualización de todas las solicitudes en formato tabla
- Asignación rápida de colaboradores con información de especialidad
- Gestión de carga de trabajo de colaboradores
- Diálogos para asignación y visualización de detalles
- Estados visuales con chips de colores
- Indicadores de urgencia y prioridad
- Interfaz responsive y moderna

#### Uso:
```razor
<AsignacionSolicitudesForm OnSolicitudAsignada="@HandleSolicitudAsignada"
                          OnSolicitudDesasignada="@HandleSolicitudDesasignada"
                          SolicitudesIniciales="@solicitudes"
                          ColaboradoresIniciales="@colaboradores" />
```

#### Propiedades:
- `OnSolicitudAsignada`: Callback ejecutado cuando se asigna un colaborador
- `OnSolicitudDesasignada`: Callback ejecutado cuando se desasigna un colaborador
- `SolicitudesIniciales`: Lista inicial de solicitudes (opcional)
- `ColaboradoresIniciales`: Lista inicial de colaboradores (opcional)

#### Modelos de Datos:
```csharp
public class SolicitudAcademica
{
    // Propiedades básicas
    public int Id { get; set; }
    public string Tema { get; set; }
    public string NivelAcademico { get; set; }
    public string TipoTrabajo { get; set; }
    public string AreaConocimiento { get; set; }
    public DateTime FechaEntrega { get; set; }
    public string Estado { get; set; }
    public bool EsUrgente { get; set; }
    
    // Propiedades de asignación
    public string ColaboradorAsignadoId { get; set; }
    public string ColaboradorAsignadoNombre { get; set; }
    public DateTime? FechaAsignacion { get; set; }
    public string NotasAsignacion { get; set; }
}

public class Usuario
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Rol { get; set; }
    public string Especialidad { get; set; }
    public int CargaTrabajo { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

#### Estados de Solicitud:
- **Pendiente**: Solicitud recién creada, sin asignar
- **Asignada**: Solicitud asignada a un colaborador
- **En Progreso**: Colaborador trabajando en la solicitud
- **En Revisión**: Solicitud en proceso de revisión
- **Completada**: Trabajo finalizado y entregado
- **Cancelada**: Solicitud cancelada por el cliente
- **Rechazada**: Solicitud rechazada por criterios internos

#### Funcionalidades del Dashboard:
- **Estadísticas**: Contadores de total, pendientes, asignadas y completadas
- **Filtros**: Por estado, nivel académico, colaborador asignado y búsqueda por tema
- **Tabla de Datos**: Visualización completa con ordenamiento y paginación
- **Asignación**: Selección de colaborador con información de especialidad y carga
- **Desasignación**: Remoción rápida de asignaciones
- **Detalles**: Vista completa de información de cada solicitud

## Páginas de Ejemplo

### GrupoXpert.Maui
- `Pages/SolicitudAcademica.razor`: Página de ejemplo para solicitudes académicas
- `Pages/CotizacionPago.razor`: Página de ejemplo para cotización y pago
- `Pages/AsignacionSolicitudes.razor`: Página de ejemplo para dashboard de asignación

### GrupoXpert.Web
- `Pages/SolicitudAcademica.razor`: Página de ejemplo para solicitudes académicas
- `Pages/CotizacionPago.razor`: Página de ejemplo para cotización y pago
- `Pages/AsignacionSolicitudes.razor`: Página de ejemplo para dashboard de asignación

## Dependencias

- **MudBlazor**: Framework de componentes UI
- **Microsoft.AspNetCore.Components**: Base de Blazor
- **Microsoft.AspNetCore.Components.Web**: Componentes web de Blazor

## Instalación

1. Asegúrate de que tu proyecto tenga las dependencias de MudBlazor instaladas
2. Agrega la referencia al proyecto `GrupoXpert.Components`
3. Incluye los using necesarios en `_Imports.razor`:

```razor
@using GrupoXpert.Core.Models
@using GrupoXpert.Blazor.Components
@using MudBlazor
```

## Personalización

### Estilos CSS
Cada componente incluye su archivo CSS correspondiente:
- `SolicitudAcademicaForm.razor.css`
- `CotizacionPagoForm.razor.css`
- `AsignacionSolicitudesForm.razor.css`

### Tarifas y Configuración
Las tarifas y configuraciones se pueden personalizar en las clases estáticas:
- `TarifasAcademicas`: Precios por nivel académico y tipo de trabajo
- `MetodosPago`: Métodos de pago disponibles
- `FasesEstandar`: Configuración de fases de pago

### Integración con Backend

Para integrar con un backend real:

1. **Solicitudes Académicas**: Implementa el servicio de envío en el callback `OnSolicitudEnviada`
2. **Cotización y Pago**: 
   - Implementa la lógica de cotización en `OnCotizacionGenerada`
   - Integra con APIs de pago reales en `OnPagoIniciado`
   - Configura webhooks para confirmación de pagos

### Ejemplo de Integración:

```csharp
private async Task HandlePagoIniciado(CotizacionPago cotizacion)
{
    try
    {
        // Llamada a API de pago
        var resultado = await _pagoService.ProcesarPagoAsync(cotizacion);
        
        if (resultado.Exitoso)
        {
            cotizacion.EstadoPago = EstadoPago.Completado;
            cotizacion.TransactionId = resultado.TransactionId;
            
            // Notificar éxito
            Snackbar.Add("Pago procesado exitosamente", Severity.Success);
        }
        else
        {
            throw new Exception(resultado.MensajeError);
        }
    }
    catch (Exception ex)
    {
        cotizacion.EstadoPago = EstadoPago.Fallido;
        Snackbar.Add($"Error en el pago: {ex.Message}", Severity.Error);
    }
}
```

## Contribución

Para contribuir al desarrollo de estos componentes:

1. Mantén la consistencia con los patrones existentes
2. Asegúrate de que los componentes funcionen en ambos proyectos (Web y MAUI)
3. Incluye validaciones apropiadas
4. Actualiza la documentación según sea necesario
5. Sigue las mejores prácticas de seguridad, especialmente en el manejo de pagos

## Notas de Seguridad

- **Nunca** hardcodees claves de API o tokens de pago
- Usa variables de entorno para configuraciones sensibles
- Implementa validación tanto en cliente como en servidor
- Asegúrate de que las transacciones de pago sean verificadas en el backend
- Implementa logging apropiado para auditoría de transacciones