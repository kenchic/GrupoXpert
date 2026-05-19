# Especificaciones para el DBA: Contexto Academia

De acuerdo al diseño del Dominio (DDD) para el caso de uso de "Solicitud Académica", a continuación se detalla la estructura física que debe ser implementada en SQL Server.

## Esquema Propuesto: `Academia`

### Tabla: `Academia.SolicitudesAcademicas`

Esta tabla almacenará el Aggregate Root `SolicitudAcademica`.

| Columna | Tipo de Dato SQL | Restricciones | Descripción |
| :--- | :--- | :--- | :--- |
| `Id` | `UNIQUEIDENTIFIER` | PRIMARY KEY | Identificador único de la solicitud (GUID). |
| `ClienteId` | `UNIQUEIDENTIFIER` | NOT NULL | FK hacia el Cliente que originó la solicitud. |
| `NivelAcademico` | `INT` | NOT NULL | Enum: (0=Secundaria, 1=Pregrado, 2=Especializacion, 3=Maestria, 4=Doctorado). |
| `TipoTrabajo` | `INT` | NOT NULL | Enum: (0=Ensayo, 1=Tesis, 2=Articulo, 3=Monografia, 4=ProyectoGrado, 5=Resumen, 6=Otro). |
| `AreaTematica` | `NVARCHAR(200)` | NOT NULL | El área de conocimiento de la solicitud. |
| `FechaEntrega` | `DATETIME2` | NOT NULL | Fecha límite en la que debe entregarse el trabajo. |
| `NumeroPaginasOPalabras` | `INT` | NOT NULL | Cantidad de páginas o palabras solicitadas. Check > 0. |
| `NormaCitacion` | `INT` | NOT NULL | Enum: (0=APA, 1=IEEE, 2=Vancouver, 3=Chicago, 4=Harvard, 5=Otra). |
| `Idioma` | `INT` | NOT NULL | Enum: (0=Espanol, 1=Ingles, 2=Portugues, 3=Frances, 4=Otro). |
| `FormatoRequerido` | `NVARCHAR(500)` | NOT NULL | Especificaciones del formato del documento (fuente, espaciado, etc.). |
| `MaterialBase` | `NVARCHAR(MAX)` | NOT NULL | Información, enlaces o contenido base suministrado por el estudiante. |
| `EsUrgente` | `BIT` | NOT NULL | Indica si el cliente requiere entrega urgente (1) o normal (0). |
| `EntregaPorFases` | `BIT` | NOT NULL | Indica si el cliente ha optado por un servicio de entrega escalonado. |

### Consideraciones adicionales:
1. **Auditoría:** Se sugiere agregar columnas de auditoría estándar del sistema (e.g., `FechaCreacion`, `UsuarioCreacion`, `FechaModificacion`, `UsuarioModificacion`, `EstadoAuditoria`) si es el estándar del proyecto.
2. **Índices:** 
   - Crear un índice Non-Clustered para `FechaEntrega`, ya que se utilizará frecuentemente para ordenamientos o búsquedas de tareas próximas a vencer.
   - Si `AreaTematica` será objeto de búsquedas frecuentes, podría considerarse un índice adicional.

Por favor, generar los scripts DDL siguiendo los estándares de SQL Server 2022+ para `GrupoXpert`.
