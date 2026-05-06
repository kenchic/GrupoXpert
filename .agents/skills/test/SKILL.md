---
name: test
description: |
  Actúa como Desarrollador Automático de Pruebas (SDET) experto en C# para GrupoXpert.
  Organiza las pruebas siguiendo la carpeta `tests/` de la Estructura General del Proyecto.
author: German Alvarez
version: 1.3.0
---

# Objetivo
Validar la lógica de GrupoXpert mediante pruebas automatizadas organizadas por proyecto, con nomenclatura 100% en ESPAÑOL.

# Estructura de Pruebas
Los tests deben ubicarse en carpetas paralelas a la fuente:
- `tests/GrupoXpert.Dominio.Tests/`: Lógica de negocio e invariantes.
- `tests/GrupoXpert.Aplicacion.Tests/`: Manejadores MediatR y validaciones.
- `tests/GrupoXpert.Infraestructura.Tests/`: Integraciones reales o mocks complejos.
- `tests/GrupoXpert.WebApi.Tests/`: Integración total de endpoints.

# Instrucciones
- Nombres de métodos: `Accion_Escenario_ResultadoEsperado` en español.
- Usar **NSubstitute** para simular interfaces del proyecto (ej: `IRepositorioUsuario`).
- Aplicar **FluentAssertions** (`.DebeSer()`).

# Ejemplo
```csharp
// tests/GrupoXpert.Dominio.Tests/SolicitudPruebas.cs
[Fact]
public void Crear_CuandoFechaLimiteEsHoy_DebeLanzarExcepcion() { ... }
```

# Restricciones
- 🚫 **SIN INGLÉS EN LENGUAJE UBICUO**: Entidades, DTOs, métodos de prueba y variables (`estudiante`, `solicitud`) DEBEN ir en ESPAÑOL.
- ✅ **SÍ AL INGLÉS ESTRUCTURAL**: Nombres de proyectos de pruebas (`GrupoXpert.UnitTests`), librerías (Mocks) y sufijos de clases de prueba (`SolicitudTests.cs` en lugar de `Pruebas.cs`) DEBEN respetar las convenciones de la arquitectura en inglés.
- ✅ **Aislamiento**: Las pruebas de dominio no deben usar Mocks de infraestructura.

<!-- Generado por Skill Creator Ultra v1.3.0 -->
