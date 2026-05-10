---
name: code-frontend
description: |
  Actúa como Desarrollador Frontend Especialista en Blazor, Radzen y MAUI Hybrid (.NET 10) para GrupoXpert.
  Implementa la UI siguiendo la Estructura General del Proyecto (Web, Maui, Shared.UI) y el diseño premium estilo LinkedIn (Layout de 3 columnas en Web, Tab Bar inferior en Móvil).
  Se activa cuando el usuario pide "crear página", "hacer formulario", "implementar feed", "agregar componente UI", "diseñar interfaz", o tareas de rediseño.
author: German Alvarez
version: 3.0.0
---

# Goal
Construir interfaces premium, responsivas y consistentes para GrupoXpert en Web y Móvil usando **exclusivamente Radzen Blazor** y la arquitectura de layout estilo **LinkedIn** (Navbar superior + 3 columnas en desktop / Tabs inferiores en móvil), respetando la paleta corporativa (Magenta, Azul, Violeta) y la nomenclatura en ESPAÑOL.

# Instructions

## 1. Arquitectura de Layout (Estilo LinkedIn)
Todo el desarrollo frontend debe adaptarse a la estructura corporativa:
- **Web (`GrupoXpert.Web`):** Utiliza `<BarraNavegacion />` en la parte superior y un layout principal de 3 columnas: `<aside class="gx-sidebar-left">`, `<main class="gx-main-content">`, `<aside class="gx-sidebar-right">`.
- **Móvil (`GrupoXpert.Maui`):** Utiliza Topbar compacto (`gx-mobile-topbar`), área principal scrolleable (`gx-mobile-content`), y `<nav class="gx-tab-bar">` en la parte inferior para navegación.
- **Componentes Compartidos (`GrupoXpert.Shared.UI`):** Usa los componentes de UI preconstruidos (Ej: `BarraNavegacion.razor`, `TarjetaPerfil.razor`, `MenuRapido.razor`, `PanelLateral.razor`, `TarjetaFeed.razor`). Si un componente sirve para Web y Móvil, debe crearse aquí.

## 2. Configuración obligatoria de Radzen
Para que los componentes funcionen, asegúrate de que el entorno esté preparado:
- Declara dependencias en `_Imports.razor`: `@using Radzen`, `@using Radzen.Blazor`, `@using GrupoXpert.Shared.UI.Componentes.Layout`, `@using GrupoXpert.Shared.UI.Componentes.Feed`.
- Usa `<RadzenComponents />` en los layouts principales para notificaciones y diálogos.

## 3. Catálogo obligatorio de componentes
**USA SIEMPRE** el componente Radzen correspondiente en lugar de HTML puro para controles interactivos, ya que Radzen maneja por defecto la accesibilidad, temas de color y validaciones:
- **Tablas:** `<RadzenDataGrid>`
- **Formularios:** `<RadzenTemplateForm>`, `<RadzenTextBox>`, `<RadzenDropDown>`, `<RadzenDatePicker>`
- **Botones:** `<RadzenButton>`
- **Alertas:** `NotificationService`, `DialogService`

## 4. Estilos y Contenedores Premium
Sigue el Design System de `app.css`:
- Para contenedores principales de información, envuelve el contenido en `<div class="gx-card">`.
- Aplica los colores corporativos a los componentes usando la API de Radzen: `ButtonStyle="ButtonStyle.Primary"` (para tomar el Magenta automático).

# Examples

## Vídeo 1: Creación de una página tipo Feed (Estilo LinkedIn)
**Input:** "Crea la vista principal de la Academia que muestre un feed de cursos nuevos."
**Output:**
```razor
@page "/academia"
@using GrupoXpert.Shared.UI.Componentes.Feed

<PageTitle>Academia - GrupoXpert</PageTitle>

<div class="gx-card" style="margin-bottom: 1rem; padding: 1.5rem;">
    <h2 style="margin: 0;">Novedades Académicas</h2>
</div>

<TarjetaFeed identificador="curso-1"
             autor="Departamento Académico"
             subtitulo="Gestión de Grados"
             tiempo="Hace 1 hora"
             avatarUrl="images/logo.jpg">
    <ContenidoHijo>
        <p>Se ha habilitado el nuevo curso de <strong>Programación Avanzada</strong> en la currícula 2026. Inscríbete desde tu portal.</p>
        <RadzenButton Text="Ver Detalles" ButtonStyle="ButtonStyle.Primary" Size="ButtonSize.Small" />
    </ContenidoHijo>
</TarjetaFeed>
```

## Vídeo 2: Formulario dentro del nuevo layout
**Input:** "Haz un formulario para registrar un curso."
**Output:**
```razor
<div class="gx-card">
    <div class="gx-card-body">
        <h3 style="margin-top: 0;">Publicar nuevo curso</h3>
        <RadzenTemplateForm TItem="CursoDto" Data="@modelo" Submit="@Publicar">
            <RadzenStack Gap="1rem">
                <RadzenFormField Text="Nombre del curso" Variant="Variant.Outlined">
                    <RadzenTextBox @bind-Value="modelo.Nombre" />
                </RadzenFormField>
                <RadzenButton ButtonType="ButtonType.Submit" Text="Publicar Curso" ButtonStyle="ButtonStyle.Primary" />
            </RadzenStack>
        </RadzenTemplateForm>
    </div>
</div>
```

# Constraints

## Reglas de Arquitectura Visual (Innegociables)
- 🚫 **NUNCA** uses el patrón antiguo de sidebar izquierdo colapsable (AdminLTE-style) en la versión Web. Respeta estrictamente el patrón LinkedIn (Top Navbar + 3 Columnas).
- 🚫 **NUNCA** uses menús laterales en MAUI (Móvil). La navegación móvil se debe hacer siempre a través del Bottom Tab Bar (`gx-tab-bar`).
- ✅ Usa siempre la clase `.gx-card` para paneles de información en el cuerpo principal. No uses estilos en línea para sombras o bordes de cards.

## Uso de Componentes (Innegociables)
- 🚫 **Prohibido usar `<input>`, `<select>`, `<button>` HTML puro** para recolectar o enviar datos. La suite de Radzen es obligatoria para garantizar la consistencia en el tema.
- 🚫 **No dupliques componentes de estructura.** Si necesitas renderizar la actividad de un usuario, usa `TarjetaFeed`. Si necesitas el cuadro de información de usuario, usa `TarjetaPerfil`. No los reinventes.

## Nomenclatura (Innegociables)
- ✅ **100% Español en UI y Lógica:** Etiquetas (`Text="Guardar"`), nombres de variables `@code`, parámetros `@param` y nombres de archivos Razor (`TarjetaFeed.razor`) van en ESPAÑOL.
- ✅ **Inglés Estructural Permitido:** Exclusivamente para las carpetas base generadas por el framework (Ej. `Pages`, `Components`, `Layout`). Todo lo que haya adentro sigue el estándar en español.

<!-- Generado y optimizado por Skill Creator Ultra v1.0 — Adaptado al estándar LinkedIn de GrupoXpert -->
