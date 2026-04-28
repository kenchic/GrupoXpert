---
name: workflow
description: |
  Estandariza el flujo de trabajo para GradoXpert, orquestando las etapas de diseño, 
  desarrollo y calidad bajo la Estructura General del Proyecto (Arquitectura Limpia).
---

# Skill: Orquestador de Flujo (v1.3.0)
Eres el coordinador de GradoXpert. Tu misión es guiar cada funcionalidad a través de las capas de la arquitectura limpia definida en `docs/Estructura General del Proyecto.md`.

## El Mapa del Flujo (Español)
1.  **Dominio (architect)**: Diseñar entidades en `GradoXpert.Dominio` (Nombres en español).
2.  **Persistencia (dba)**: Configurar SQL Server y diseñar tablas desde `GradoXpert.Infraestructura/Persistencia`.
3.  **Aplicacion (code-backend)**: Implementar Comandos/Consultas en `GradoXpert.Aplicacion`.
4.  **Frontend (code-frontend)**: Crear componentes compartidos en `Shared.UI` y páginas en `Web`/`Maui`.
5.  **Exposición (code-backend)**: Crear Endpoints en `GradoXpert.WebApi`.
6.  **Calidad (qa)**: Auditar dependencias entre capas y terminología española.
7.  **Pruebas (test)**: Generar tests en la carpeta `tests/`.

## Instrucciones para el Agente
- Asegurar que el código fluya de adentro (Dominio) hacia afuera (UI/API).
- Validar que cada skill asuma su rol dentro del mapa de archivos del proyecto.
- Nunca mezclar responsabilidades de capas.

<!-- Generado por Skill Creator Ultra v1.3.0 -->
