---
name: code-frontend
description: |
  Acts as a Frontend Developer Specialist in Blazor, Radzen, and MAUI Hybrid (.NET 10) for GrupoXpert.
  Implements the UI following the Project's General Structure (Web, Maui, Shared.UI) and the LinkedIn-style premium design (3-column layout on Web, Bottom Tab Bar on Mobile).
  Activated when the user asks to "create page", "make form", "implement feed", "add UI component", "design interface", or redesign tasks.
  Includes logic to prevent common errors in MAUI connectivity, Android permissions, and API contract consistency.
author: German Alvarez
version: 3.0.0
---

# Goal
Build premium, responsive, and consistent interfaces for GrupoXpert on Web and Mobile using **exclusively Radzen Blazor** and the **LinkedIn** style layout architecture (Top Navbar + 3 columns on desktop / Bottom Tabs on mobile), respecting the corporate palette (Magenta, Blue, Violet) and SPANISH nomenclature for domain logic.

# Instructions

## 1. Layout Architecture (LinkedIn Style)
All frontend development must adapt to the corporate structure:
- **Web (`GrupoXpert.Web`):** Uses `<BarraNavegacion />` at the top and a main 3-column layout: `<aside class="gx-sidebar-left">`, `<main class="gx-main-content">`, `<aside class="gx-sidebar-right">`.
- **Mobile (`GrupoXpert.Maui`):** Uses a compact topbar (`gx-mobile-topbar`), a scrollable main area (`gx-mobile-content`), and `<nav class="gx-tab-bar">` at the bottom for navigation.
- **Shared Components (`GrupoXpert.Shared.UI`):** Use pre-built UI components (e.g., `BarraNavegacion.razor`, `TarjetaPerfil.razor`, `MenuRapido.razor`, `PanelLateral.razor`, `TarjetaFeed.razor`). If a component serves both Web and Mobile, it must be created here.

## 2. Mandatory Radzen and Environment Configuration
To ensure components function and connectivity is successful:
- **Dependencies in `_Imports.razor`:** `@using Radzen`, `@using Radzen.Blazor`, `@using GrupoXpert.Shared.UI.Componentes.Layout`, `@using Microsoft.AspNetCore.Components.Authorization`, `@using Microsoft.Maui.Storage`.
- **Main Layouts:** Use `<RadzenComponents />` for notifications and dialogs.
- **MAUI Connectivity (Android):** In `MauiProgram.cs`, detect the environment to use `http://10.0.2.2:PORT/` on emulator instead of `localhost`.
- **Android Permissions:** In MAUI's `AndroidManifest.xml`, always ensure `android:usesCleartextTraffic="true"` for HTTP traffic in development.
- **Fonts and Icons:** In `index.html` (Web/MAUI), the link order must be: Preconnects -> Google Fonts -> Material Icons -> Bootstrap -> Radzen -> app.css. This avoids rendering failures on emulators.

## 3. Session Persistence and Security
Token management varies by platform:
- **Web:** Use Cookies or SessionStorage.
- **MAUI:** Use `Preferences.Default.Set("authToken", token)` to persist the session.
- **Navigation Guards:** In `Home.razor` or protected pages, always verify the token in `OnInitialized` before allowing access, but **NEVER** leave unconditional redirects that cause loops.

## 4. Contract Consistency (Nomenclature)
To avoid deserialization errors (nulls in the backend):
- **Spanish Contracts:** If the backend uses `IniciarSesionCommand(string Correo, string Clave)`, the frontend **MUST** send an object with the same keys (`{ Correo, Clave }`), never English names like `Email` or `Password`.
- **Validation:** Always implement `string.IsNullOrWhiteSpace` checks on the frontend before sending requests.

## 5. Mandatory Component Catalog
**ALWAYS USE** the corresponding Radzen component instead of pure HTML for interactive controls, as Radzen handles accessibility, color themes, and validations by default:
- **Tables:** `<RadzenDataGrid>`
- **Forms:** `<RadzenTemplateForm>`, `<RadzenTextBox>`, `<RadzenDropDown>`, `<RadzenDatePicker>`
- **Buttons:** `<RadzenButton>`
- **Alerts:** `NotificationService`, `DialogService`

## 6. Premium Styles and Containers
Follow the Design System in `app.css`:
- For main information containers, wrap content in `<div class="gx-card">`.
- Apply corporate colors to components using the Radzen API: `ButtonStyle="ButtonStyle.Primary"` (to automatically take the Magenta).
- **Material Icons:** Ensure icons are inside `<span class="material-icons">...</span>` and that `app.css` has the `font-family: 'Material Icons' !important` fix.

# Examples

## Video 1: Creating a Feed-style Page (LinkedIn Style)
**Input:** "Create the main view for the Academy showing a feed of new courses."
**Output:**
```razor
@page "/academia"
@using GrupoXpert.Shared.UI.Componentes.Feed

<PageTitle>Academy - GrupoXpert</PageTitle>

<div class="gx-card" style="margin-bottom: 1rem; padding: 1.5rem;">
    <h2 style="margin: 0;">Academic Updates</h2>
</div>

<TarjetaFeed identificador="curso-1"
             autor="Academic Department"
             subtitulo="Degree Management"
             tiempo="1 hour ago"
             avatarUrl="images/logo.jpg">
    <ContenidoHijo>
        <p>The new <strong>Advanced Programming</strong> course has been enabled in the 2026 curriculum. Sign up from your portal.</p>
        <RadzenButton Text="View Details" ButtonStyle="ButtonStyle.Primary" Size="ButtonSize.Small" />
    </ContenidoHijo>
</TarjetaFeed>
```

## Video 2: Form with Spanish Contract (MAUI)
**Input:** "Implement the submission logic for login in MAUI."
**Output:**
```razor
private async Task ManejarLogin(LoginModelo modelo)
{
    // FIX: Backend contract expects 'Correo', not 'Email'
    var respuesta = await Http.PostAsJsonAsync("api/autenticacion/iniciar-sesion", new 
    { 
        Correo = modelo.Correo, 
        Clave = modelo.Clave 
    });

    if (respuesta.IsSuccessStatusCode)
    {
        var resultado = await respuesta.Content.ReadFromJsonAsync<ResultadoSesion>();
        // MAUI-specific persistence
        Preferences.Default.Set("authToken", resultado.TokenAcceso);
        Navegador.NavigateTo("/");
    }
}
```

# Constraints

## Visual Architecture Rules (Non-negotiable)
- 🚫 **NEVER** use the old collapsible left sidebar pattern (AdminLTE-style) in the Web version. Strictly respect the LinkedIn pattern (Top Navbar + 3 Columns).
- 🚫 **NEVER** use side menus in MAUI (Mobile). Mobile navigation must always be done through the Bottom Tab Bar (`gx-tab-bar`).
- ✅ Always use the `.gx-card` class for information panels in the main body. Do not use inline styles for card shadows or borders.

## Component Usage (Non-negotiable)
- 🚫 **Prohibited to use pure HTML `<input>`, `<select>`, `<button>`** for collecting or sending data. The Radzen suite is mandatory to ensure theme consistency.
- 🚫 **Do not duplicate structure components.** If you need to render user activity, use `TarjetaFeed`. If you need a user info box, use `TarjetaPerfil`. Do not reinvent them.

## Nomenclature (Non-negotiable)
- ✅ **100% Spanish in UI and Logic:** Labels (`Text="Guardar"`), variable names `@code`, parameters `@param`, and Razor filenames (`TarjetaFeed.razor`) go in SPANISH.
- ✅ **Structural English Allowed:** Exclusively for base folders generated by the framework (e.g., `Pages`, `Components`, `Layout`). Everything inside follows the Spanish standard.

<!-- Generated and optimized by Skill Creator Ultra v1.0 — Adapted to GrupoXpert's LinkedIn standard -->
