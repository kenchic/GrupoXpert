---
name: code-frontend
description: |
  Actúa como Desarrollador Frontend Especialista en Blazor, Radzen y MAUI Hybrid (.NET 10) para GradoXpert.
  Implementa la UI siguiendo la Estructura General del Proyecto (Web, Maui, Shared.UI).
  Se activa cuando el usuario pide "crear página", "hacer formulario", "implementar tabla",
  "agregar componente UI", "construir pantalla", "diseñar interfaz" o cualquier tarea de frontend.
author: German Alvarez
version: 2.0.0
---

# Objetivo

Construir interfaces premium para GradoXpert en Web y Móvil usando **exclusivamente Radzen Blazor**
como librería de componentes, respetando la estructura de proyectos compartidos y nomenclatura en **ESPAÑOL**.

---

# Estructura del Frontend

Organiza el código siguiendo esta jerarquía:

```
GradoXpert.Web/                    → Blazor Web App (.NET 10)
  Componentes/Paginas/             → Páginas principales

GradoXpert.Maui/                   → Blazor Hybrid (.NET 10 MAUI)
  → Interfaz móvil multiplataforma

GradoXpert.Compartido.UI/          → Razor Class Library (.NET 10)
  Componentes/[Funcionalidad]/     → TODOS los componentes reutilizables aquí
    Ej: Componentes/Solicitudes/TablaSolicitudes.razor
  Abstracciones/                   → Interfaces para servicios de plataforma
```

> **Regla de oro:** Si un componente puede ser usado en más de un lugar → va en `Compartido.UI`.

---

# Instrucciones

## 1. Configuración obligatoria de Radzen

Antes de crear cualquier componente, verifica que `Program.cs` tenga:

```csharp
// Program.cs (Web y Maui)
builder.Services.AddRadzenComponents();
```

Y en `app.css` o el layout principal:

```html
<!-- _Imports.razor o MainLayout.razor -->
<link rel="stylesheet" href="_content/Radzen.Blazor/css/material-base.css" />
@* Tema de color del proyecto (Magenta/Azul) *@
```

En `_Imports.razor` del proyecto:

```razor
@using Radzen
@using Radzen.Blazor
```

Y al final del layout, el componente de notificaciones:

```razor
<RadzenComponents />
```

## 2. Catálogo obligatorio de componentes Radzen

**USA SIEMPRE** el componente Radzen correspondiente. Prohibido crear equivalentes en HTML puro.

| Necesidad UI | Componente Radzen | Ejemplo de uso |
|---|---|---|
| Tabla de datos | `<RadzenDataGrid>` | Listas, catálogos, reportes |
| Formulario de entrada texto | `<RadzenTextBox>` / `<RadzenTextArea>` | Nombre, descripción |
| Selector numérico | `<RadzenNumeric>` | Notas, cantidades |
| Fecha/hora | `<RadzenDatePicker>` | Fechas de solicitud, nacimiento |
| Dropdown / Select | `<RadzenDropDown>` | Catálogos, estados |
| Autocomplete | `<RadzenAutoComplete>` | Búsqueda de estudiantes, materias |
| Checkbox | `<RadzenCheckBox>` | Activar/desactivar opciones |
| Radio button | `<RadzenRadioButtonList>` | Opciones excluyentes |
| Botón de acción | `<RadzenButton>` | Guardar, cancelar, filtrar |
| Diálogo / Modal | `<RadzenDialog>` vía `DialogService` | Confirmaciones, edición rápida |
| Notificaciones | `<RadzenNotification>` vía `NotificationService` | Éxito, error, advertencia |
| Tabs / Pestañas | `<RadzenTabs>` | Secciones de formulario |
| Panel / Card | `<RadzenPanel>` | Agrupación visual |
| Spinner / Carga | `<RadzenProgressBarCircular>` | Estados de carga async |
| Menú lateral | `<RadzenSidebar>` | Navegación principal |
| Paginación | Propiedad `AllowPaging` en `RadzenDataGrid` | Tablas largas |

## 3. Paleta de colores

Aplica la paleta del proyecto mediante variables CSS en `app.css`:

```css
:root {
  --color-primario: #C2185B;      /* Magenta */
  --color-secundario: #1565C0;    /* Azul */
  --color-acento: #7B1FA2;        /* Violeta */
  --color-fondo: #F5F5F5;
  --color-texto: #212121;
}
```

En componentes Radzen, usa la propiedad `ButtonStyle` o `class` para alinearte:

```razor
<RadzenButton ButtonStyle="ButtonStyle.Primary" Text="Guardar" />
<RadzenButton ButtonStyle="ButtonStyle.Light" Text="Cancelar" />
```

## 4. Nomenclatura

- Archivos Razor: PascalCase en **español** → `FormularioRegistro.razor`, `TablaSolicitudes.razor`
- Parámetros de componente: camelCase en español → `@param estudianteSeleccionado`
- Variables en código `@code { }`: camelCase en español

---

# Ejemplos

## Ejemplo 1 — Tabla con RadzenDataGrid

```razor
@* Componentes/Solicitudes/TablaSolicitudes.razor *@
@using Radzen.Blazor

<RadzenDataGrid Data="@solicitudes"
                TItem="SolicitudDto"
                AllowPaging="true"
                PageSize="10"
                AllowSorting="true"
                AllowFiltering="true"
                FilterMode="FilterMode.Advanced"
                class="tabla-solicitudes">
    <Columns>
        <RadzenDataGridColumn TItem="SolicitudDto" Property="Codigo" Title="Código" Width="120px" />
        <RadzenDataGridColumn TItem="SolicitudDto" Property="Estudiante" Title="Estudiante" />
        <RadzenDataGridColumn TItem="SolicitudDto" Property="Estado" Title="Estado">
            <Template Context="sol">
                <RadzenBadge Text="@sol.Estado" BadgeStyle="BadgeStyle.Info" />
            </Template>
        </RadzenDataGridColumn>
        <RadzenDataGridColumn TItem="SolicitudDto" Title="Acciones" Filterable="false" Sortable="false" Width="120px">
            <Template Context="sol">
                <RadzenButton Icon="edit" ButtonStyle="ButtonStyle.Light" Size="ButtonSize.Small"
                              Click="@(() => AbrirEdicion(sol))" />
            </Template>
        </RadzenDataGridColumn>
    </Columns>
</RadzenDataGrid>

@code {
    [Parameter] public IEnumerable<SolicitudDto> solicitudes { get; set; } = [];

    private void AbrirEdicion(SolicitudDto sol) { /* ... */ }
}
```

## Ejemplo 2 — Formulario con validación usando RadzenTemplateForm

```razor
@* Componentes/Estudiantes/FormularioEstudiante.razor *@
@inject NotificationService Notificaciones

<RadzenTemplateForm TItem="EstudianteDto" Data="@modelo" Submit="@GuardarAsync">
    <RadzenStack Gap="1rem">

        <RadzenFormField Text="Nombre completo" Variant="Variant.Outlined">
            <RadzenTextBox @bind-Value="modelo.NombreCompleto" Placeholder="Ingrese el nombre" />
        </RadzenFormField>

        <RadzenFormField Text="Fecha de nacimiento" Variant="Variant.Outlined">
            <RadzenDatePicker @bind-Value="modelo.FechaNacimiento" DateFormat="dd/MM/yyyy" />
        </RadzenFormField>

        <RadzenFormField Text="Programa académico" Variant="Variant.Outlined">
            <RadzenDropDown @bind-Value="modelo.ProgramaId"
                            Data="@programas"
                            TextProperty="Nombre"
                            ValueProperty="Id"
                            Placeholder="Seleccione un programa" />
        </RadzenFormField>

        <RadzenStack Orientation="Orientation.Horizontal" JustifyContent="JustifyContent.End" Gap="0.5rem">
            <RadzenButton ButtonType="ButtonType.Submit" Text="Guardar" ButtonStyle="ButtonStyle.Primary" />
            <RadzenButton Text="Cancelar" ButtonStyle="ButtonStyle.Light" Click="@Cancelar" />
        </RadzenStack>

    </RadzenStack>
</RadzenTemplateForm>

@code {
    [Parameter] public EstudianteDto modelo { get; set; } = new();
    [Parameter] public IEnumerable<ProgramaDto> programas { get; set; } = [];
    [Parameter] public EventCallback Cancelar { get; set; }

    private async Task GuardarAsync()
    {
        // Llamar al comando via HttpClient / MediatR
        Notificaciones.Notify(NotificationSeverity.Success, "Guardado", "Estudiante registrado correctamente.");
    }
}
```

## Ejemplo 3 — Diálogo de confirmación con DialogService

```razor
@* Uso en una página *@
@inject DialogService Dialogo

<RadzenButton Text="Eliminar" ButtonStyle="ButtonStyle.Danger"
              Click="@ConfirmarEliminacion" />

@code {
    private async Task ConfirmarEliminacion()
    {
        bool? resultado = await Dialogo.Confirm(
            "¿Está seguro de eliminar este registro?",
            "Confirmar eliminación",
            new ConfirmOptions { OkButtonText = "Sí, eliminar", CancelButtonText = "Cancelar" });

        if (resultado == true)
        {
            // Ejecutar comando de eliminación
        }
    }
}
```

---

# Restricciones

## Uso de Radzen (innegociables — el motivo es consistencia visual y mantenimiento)

- 🚫 **Prohibido usar `<input>`, `<select>`, `<button>` HTML puro** cuando existe un equivalente Radzen — Radzen ya maneja accesibilidad, temas y validación.
- 🚫 **Prohibido crear tablas con `<table>/<tr>/<td>`** para datos — usar `<RadzenDataGrid>`.
- 🚫 **Prohibido usar `alert()` o `window.confirm()` de JavaScript** — usar `NotificationService` y `DialogService`.
- ✅ HTML semántico puro (`<section>`, `<article>`, `<h1>`) está permitido para estructura, NO para controles interactivos.

## Arquitectura

- 🚫 **No duplicar componentes**: Si ya existe en `GradoXpert.Compartido.UI`, reutilízalo.
- ✅ Todo componente nuevo potencialmente compartible va en `Compartido.UI`, no en `Web` ni `Maui`.

## Idioma

- ✅ **100% Español en UI y Negocio**: Etiquetas, placeholders, nombres de archivos Razor (`Formulario.razor`), y variables de componente deben ir en ESPAÑOL.
- ✅ Los DTOs y modelos de dominio ya vienen del backend en español — mantenlos igual en la UI.
- ✅ **Inglés Estructural Permitido**: Las carpetas base de la arquitectura (como `Pages`, `Components`, `Shared`) DEBEN ir en inglés según el estándar, pero sus subcarpetas de Bounded Contexts y archivos van en español.

## Calidad

- Cada componente debe manejar el estado de carga (`bool estaCargando`) con `<RadzenProgressBarCircular>`.
- Los errores de operación deben notificarse vía `NotificationService`, nunca silenciarse.

<!-- Generado por Skill Creator Ultra v1.0 — Mejorado v2.0.0 -->
