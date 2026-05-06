# GrupoXpert

> Plataforma de servicios académicos que conecta estudiantes con expertos para asistencia en tesis, ensayos, trabajos de investigación y más.

## Descripción

**GrupoXpert** es un marketplace de servicios académicos multiplataforma desarrollado con .NET 10. Permite a estudiantes solicitar ayuda académica y ser conectados automáticamente con colaboradores expertos según su área de conocimiento.

La plataforma gestiona todo el flujo: desde la solicitud y cotización automática hasta el pago por hitos, entrega de trabajos y evaluación de calidad.

## Tecnologías

| Capa | Tecnología |
|---|---|
| **Backend** | .NET 10, ASP.NET Core Web API, Minimal APIs |
| **Frontend Web** | Blazor Web App + Radzen.Blazor |
| **Mobile** | .NET MAUI Blazor Hybrid (Android, iOS, macOS, Windows) |
| **Base de datos** | SQL Server + Entity Framework Core 10 |
| **Autenticación** | JWT Bearer + BCrypt |
| **Patrón** | Clean Architecture + CQRS (MediatR) |
| **Validación** | FluentValidation |
| **Testing** | xUnit, NSubstitute, FluentAssertions, bUnit |

## Estructura del Proyecto

```
GrupoXpert.sln
├── GrupoXpert.Domain           # Capa de dominio (entidades, value objects, eventos)
├── GrupoXpert.Application      # Casos de uso (Comandos/Queries CQRS)
├── GrupoXpert.Infrastructure   # Acceso a datos, servicios externos
├── GrupoXpert.WebApi           # API REST
├── GrupoXpert.Web              # Aplicación Blazor Web
├── GrupoXpert.Maui             # Aplicación MAUI Hybrid
├── GrupoXpert.Shared.UI        # Componentes UI compartidos (RCL)
├── GrupoXpert.UnitTests        # Pruebas unitarias
├── GrupoXpert.IntegrationTests # Pruebas de integración
└── GrupoXpert.ComponentTests   # Pruebas de componentes Blazor
```

## Arquitectura

Sigue **Clean Architecture** con dependencia unidireccional hacia el dominio:

```
Presentation → Application → Domain
               Infrastructure ↗
```

- **Domain**: Sin dependencias externas. Contiene agregados, entidades, value objects y eventos de dominio.
- **Application**: Comandos y queries con MediatR. Depende solo del Domain.
- **Infrastructure**: EF Core, repositorios, servicios externos (email, JWT).
- **Presentation**: Web API, Blazor Web y MAUI comparten la misma capa de aplicación.

## Funcionalidades

### Implementadas
- [x] Registro de usuarios con verificación por email
- [x] Autenticación con JWT
- [x] Activación de cuentas

### Planificadas
- [ ] Gestión de perfiles (estudiante y colaborador)
- [ ] Solicitud de servicios con formularios dinámicos
- [ ] Cotización automática
- [ ] Asignación automática de colaboradores
- [ ] Pagos por hitos
- [ ] Chat interno y notificaciones
- [ ] Entrega de trabajos (parcial/final)
- [ ] Revisión de calidad y anti-plagio
- [ ] Evaluación del servicio
- [ ] Panel de administración

## Requisitos

- .NET 10 SDK
- SQL Server 2022+
- Visual Studio 2026 / VS Code
- Node.js (para herramientas de desarrollo)

## Inicio Rápido

```bash
# Restaurar dependencias
dotnet restore

# Ejecutar migraciones
dotnet ef database update --project Src/GrupoXpert.Infrastructure

# Iniciar API
dotnet run --project Src/GrupoXpert.WebApi

# Iniciar Web
dotnet run --project Src/GrupoXpert.Web
```

## Testing

```bash
# Pruebas unitarias
dotnet test --filter UnitTests

# Pruebas de integración
dotnet test --filter IntegrationTests

# Pruebas de componentes
dotnet test --filter ComponentTests
```

## Licencia

Este proyecto está bajo la licencia MIT. Ver [LICENSE](LICENSE) para más detalles.
