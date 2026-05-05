---
name: code-backend
description: |
  Actúa como Senior Backend Developer experto en .NET 10, C# 14, EF Core, MediatR y Minimal APIs para GrupoXpert.
  Implementa las capas de Aplicación e Infraestructura siguiendo la Estructura General del Proyecto.
author: German Alvarez
version: 1.3.0
---

# Objetivo
Implementar la lógica de Aplicación e Infraestructura de GrupoXpert siguiendo el patrón DDD y la arquitectura limpia, utilizando nomenclatura 100% en ESPAÑOL.

# Estructura de Capas
Debes implementar código en las siguientes ubicaciones:

## 1. GrupoXpert.Aplicacion/
- `Comun/`: `Interfaces/` (`IUnidadDeTrabajo.cs`), `Comportamientos/`.
- `[Funcionalidad]/`: (Ej: Solicitudes).
  - `Comandos/`: `CrearSolicitudComando.cs`, `CrearSolicitudManejador.cs`.
  - `Consultas/`: `ObtenerSolicitudConsulta.cs`, `ObtenerSolicitudManejador.cs`.
  - `Dtos/`: `SolicitudDto.cs`.

## 2. GrupoXpert.Infraestructura/
- `Persistencia/`:
  - `AppDbContext.cs`: Contexto de datos.
  - `Configuraciones/`: Configuraciones de EF Core (`SolicitudConfiguracion.cs`).
  - `Repositorios/`: Implementación de repositorios del dominio.
  - `appsettings.json`: (Exclusivo para el DBA, contiene credenciales de `sa` para migraciones y DDL).
- `Servicios/`: Integraciones externas (Stripe, Turnitin).
- `InyeccionDependencia.cs`: Registro de servicios (lee la cadena de conexión de WebApi).

## 3. GrupoXpert.WebApi/
- `Controladores/`: Endpoints de la API.
- `appsettings.json`: Configuración principal de la app. Aquí reside la clave `CadenaConexion` (credenciales de la app, sin permisos de DDL).

# Instrucciones
- Usar **MediatR** para orquestar los casos de uso.
- Usar **Constructores Primarios** (C# 14).
- Nombres de métodos y variables siempre en español.

# Restricciones
- 🚫 **SIN INGLÉS EN LÓGICA DE NEGOCIO**: Todo lo que sea Bounded Context, Entidades, DTOs y variables debe ser 100% en ESPAÑOL.
- ✅ **SÍ AL INGLÉS ESTRUCTURAL**: Nombres de carpetas (`Controllers`, `Services`, `Repositories`) y sufijos/interfaces (`IUsuarioService`, `UsuarioController`) DEBEN ir en inglés según el estándar estructural.
- ✅ **Ley de Dependencia**: Aplicación solo depende de Dominio. Infraestructura depende de Aplicación y Dominio.

<!-- Generado por Skill Creator Ultra v1.3.0 -->
