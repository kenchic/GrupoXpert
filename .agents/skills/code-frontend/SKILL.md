---
name: code-frontend
description: |
  Actúa como Desarrollador Frontend Especialista en Blazor, Radzen y MAUI Hybrid (.NET 10) para GrupoXpert.
  Implementa la UI siguiendo la Estructura General del Proyecto (Web, Maui, Shared.UI) y el diseño premium estilo LinkedIn (Layout de 3 columnas en Web, Tab Bar inferior en Móvil).
  Se activa cuando el usuario pide "crear página", "hacer formulario", "implementar feed", "agregar componente UI", "diseñar interfaz", o tareas de rediseño.
  Incluye lógica de prevención de errores comunes en conectividad MAUI, permisos de Android y consistencia de contratos API.
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

## 2. Configuración obligatoria de Radzen y Entorno
Para que los componentes funcionen y la conectividad sea exitosa, asegura el entorno:
- **Dependencias en `_Imports.razor`:** `@using Radzen`, `@using Radzen.Blazor`, `@using GrupoXpert.Shared.UI.Componentes.Layout`, `@using Microsoft.AspNetCore.Components.Authorization`, `@using Microsoft.Maui.Storage`.
- **Layouts Principales:** Usa `<RadzenComponents />` para notificaciones y diálogos.
- **Conectividad MAUI (Android):** En `MauiProgram.cs`, detecta el entorno para usar `http://10.0.2.2:PORT/` en emulador en lugar de `localhost`.
- **Permisos Android:** En el `AndroidManifest.xml` de MAUI, asegura siempre `android:usesCleartextTraffic="true"` para tráfico HTTP en desarrollo.
- **Fuentes e Iconos:** En `index.html` (Web/MAUI), el orden de links debe ser: Preconnects -> Google Fonts -> Material Icons -> Bootstrap -> Radzen -> app.css. Esto evita fallos de renderizado en emuladores.

## 3. Persistencia de Sesión y Seguridad
La gestión de tokens varía según la plataforma:
- **Web:** Utiliza Cookies o SessionStorage.
- **MAUI:** Utiliza `Preferences.Default.Set("authToken", token)` para persistir la sesión.
- **Guardias de Navegación:** En `Home.razor` o páginas protegidas, verifica siempre el token en `OnInitialized` antes de permitir el acceso, pero **NUNCA** dejes redirecciones incondicionales que causen bucles.

## 4. Consistencia de Contratos (Nomenclatura)
Para evitar errores de deserialización (nulos en el backend):
- **Contratos en Español:** Si el backend usa `IniciarSesionCommand(string Correo, string Clave)`, el frontend **DEBE** enviar un objeto con las mismas claves (`{ Correo, Clave }`), nunca nombres en inglés como `Email` o `Password`.
- **Validación:** Implementa siempre checks de `string.IsNullOrWhiteSpace` en el frontend antes de enviar peticiones.

## 5. Catálogo obligatorio de componentes
**USA SIEMPRE** el componente Radzen correspondiente en lugar de HTML puro para controles interactivos, ya que Radzen maneja por defecto la accesibilidad, temas de color y validaciones:
- **Tablas:** `<RadzenDataGrid>`
- **Formularios:** `<RadzenTemplateForm>`, `<RadzenTextBox>`, `<RadzenDropDown>`, `<RadzenDatePicker>`
- **Botones:** `<RadzenButton>`
- **Alertas:** `NotificationService`, `DialogService`

## 6. Estilos y Contenedores Premium
Sigue el Design System de `app.css`:
- Para contenedores principales de información, envuelve el contenido en `<div class="gx-card">`.
- Aplica los colores corporativos a los componentes usando la API de Radzen: `ButtonStyle="ButtonStyle.Primary"` (para tomar el Magenta automático).
- **Material Icons:** Asegura que los iconos estén dentro de `<span class="material-icons">...</span>` y que `app.css` tenga el fix de `font-family: 'Material Icons' !important`.

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

## Vídeo 2: Formulario con contrato en Español (MAUI)
**Input:** "Implementa la lógica de envío para el login en MAUI."
**Output:**
```razor
private async Task ManejarLogin(LoginModelo modelo)
{
    // FIX: El contrato del backend espera 'Correo', no 'Email'
    var respuesta = await Http.PostAsJsonAsync("api/autenticacion/iniciar-sesion", new 
    { 
        Correo = modelo.Correo, 
        Clave = modelo.Clave 
    });

    if (respuesta.IsSuccessStatusCode)
    {
        var resultado = await respuesta.Content.ReadFromJsonAsync<ResultadoSesion>();
        // Persistencia específica de MAUI
        Preferences.Default.Set("authToken", resultado.TokenAcceso);
        Navegador.NavigateTo("/");
    }
}
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
