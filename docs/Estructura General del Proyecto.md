# Estructura General del Proyecto
Estructura de la solución que todos los Skills deben conocer:

MyApp.sln
│
├── src/
│   ├── MyApp.Domain/                    # Capa de Dominio (núcleo) [Tipo: Biblioteca .NET 10]
│   │   ├── Common/
│   │   │   ├── Entity.cs
│   │   │   ├── ValueObject.cs
│   │   │   ├── AggregateRoot.cs
│   │   │   └── IDomainEvent.cs
│   │   ├── [BoundedContext]/
│   │   │   ├── [Aggregate].cs           # Aggregate Root
│   │   │   ├── [Entity].cs
│   │   │   ├── [ValueObject].cs
│   │   │   ├── I[Aggregate]Repository.cs
│   │   │   └── Events/
│   │   │       └── [Domain]Event.cs
│   │   └── Exceptions/
│   │       └── DomainException.cs
│   │
│   ├── MyApp.Application/               # Capa de Aplicación [Tipo: Biblioteca .NET 10]
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IUnitOfWork.cs
│   │   │   │   └── ICurrentUser.cs
│   │   │   └── Behaviors/
│   │   │       ├── ValidationBehavior.cs
│   │   │       └── LoggingBehavior.cs
│   │   └── [Feature]/
│   │       ├── Commands/
│   │       │   ├── Create[Feature]Command.cs
│   │       │   └── Create[Feature]Handler.cs
│   │       ├── Queries/
│   │       │   ├── Get[Feature]Query.cs
│   │       │   └── Get[Feature]Handler.cs
│   │       └── DTOs/
│   │           └── [Feature]Dto.cs
│   │
│   ├── MyApp.Infrastructure/            # Capa de Infraestructura [Tipo: Biblioteca .NET 10]
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/
│   │   │   │   └── [Entity]Configuration.cs
│   │   │   └── Repositories/
│   │   │       └── [Aggregate]Repository.cs
│   │   ├── Services/
│   │   │   └── [ExternalService].cs
│   │   └── DependencyInjection.cs
│   │
│   ├── MyApp.WebApi/                    # API REST (ASP.NET Core) [Tipo: Proyecto Web .NET 10]
│   │   ├── Controllers/
│   │   │   └── [Feature]Controller.cs
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── MyApp.Web/                       # Blazor Web App [Tipo: Proyecto Web .NET 10]
│   │   ├── Components/
│   │   │   ├── Pages/
│   │   │   │   └── [Feature]Page.razor
│   │   │   └── Shared/
│   │   ├── Program.cs
│   │   └── App.razor
│   │
│   ├── MyApp.Maui/                      # MAUI Blazor Hybrid [Tipo: Proyecto .NET 10 (Aplicación)]
│   │   ├── Platforms/
│   │   ├── MauiProgram.cs
│   │   └── MainPage.xaml
│   │
│   └── MyApp.Shared.UI/                 # Razor Class Library (RCL) compartida [Tipo: Biblioteca .NET 10 (RCL)]
│       ├── Components/
│       │   └── [Feature]/
│       │       └── [Feature]Component.razor
│       └── Abstractions/
│           └── I[PlatformService].cs
│
└── tests/
    ├── MyApp.UnitTests/                # Domain + Application (lógica pura, sin I/O) [Tipo: Biblioteca .NET 10 (Tests)]
    ├── MyApp.IntegrationTests/         # Infrastructure + WebApi (BD, HTTP) [Tipo: Biblioteca .NET 10 (Tests)]
    └── MyApp.ComponentTests/           # Blazor Web + Shared.UI (bUnit) [Tipo: Biblioteca .NET 10 (Tests)]

# Reglas de dependencia (Dependency Rule):

- Domain no depende de ninguna capa.
- Application depende solo de Domain.
- Infrastructure depende de Application y Domain.
- WebApi, Web, Maui dependen de Application e Infrastructure.
- Shared.UI (RCL) NO depende de Maui ni de plataformas específicas.

🏗️ Arquitectura del Layout
Web — MainLayout.razor 
┌──────────────────────────────────────────────────────────────┐
│  TOP NAVBAR (fijo)                                           │
│  [Logo]  [Buscar...]  [🏠 Inicio] [👥 Red] [💼 Academia]    │
│                        [🔔 Notif] [💬 Msg] [👤 Perfil ▾]    │
├──────────────┬─────────────────────────────┬─────────────────┤
│  LEFT SIDEBAR│     MAIN CONTENT AREA       │  RIGHT SIDEBAR  │
│  (~250px)    │     (flexible)              │  (~300px)       │
│              │                             │                 │
│  ┌─────────┐ │  ┌───────────────────────┐  │  ┌───────────┐ │
│  │ Avatar  │ │  │  Card de contenido    │  │  │ Accesos   │ │
│  │ Nombre  │ │  │  principal/feed       │  │  │ Rápidos   │ │
│  │ Rol     │ │  │                       │  │  │           │ │
│  │ ─────── │ │  └───────────────────────┘  │  ├───────────┤ │
│  │ Stats   │ │  ┌───────────────────────┐  │  │ Anuncios  │ │
│  │ • Cursos│ │  │  Card de actividad    │  │  │           │ │
│  │ • Notas │ │  │  reciente             │  │  │ Eventos   │ │
│  │ • Docs  │ │  └───────────────────────┘  │  │ próximos  │ │
│  └─────────┘ │                             │  └───────────┘ │
│              │                             │                 │
│  ┌─────────┐ │                             │  ┌───────────┐ │
│  │ Menú    │ │                             │  │ Sugeridos │ │
│  │ Rápido  │ │                             │  │           │ │
│  └─────────┘ │                             │  └───────────┘ │
└──────────────┴─────────────────────────────┴─────────────────┘
Móvil — MainLayout.razor (MAUI)
┌──────────────────────────────┐
│  [👤]  [🔍 Buscar...]  [💬] │  ← Top bar compacta
├──────────────────────────────┤
│                              │
│  ┌──────────────────────┐    │
│  │ Perfil Card (mini)   │    │
│  └──────────────────────┘    │
│                              │
│  ┌──────────────────────┐    │  ← Scrollable content
│  │ Feed / Contenido     │    │
│  │ adaptable por rol    │    │
│  └──────────────────────┘    │
│                              │
│  ┌──────────────────────┐    │
│  │ Más contenido...     │    │
│  └──────────────────────┘    │
│                              │
├──────────────────────────────┤
│ 🏠  👥  ➕  🔔  👤          │  ← Bottom Tab Bar
│ Inicio Red Post Notif Perfil │
└──────────────────────────────┘
🎨 Paleta de Colores
css
:root {
  /* === Colores principales (se mantienen) === */
  --color-primario: #A62677;        /* Magenta del logo */
  --color-secundario: #4A76B2;      /* Azul del logo */
  --color-acento: #6B2D5C;          /* Violeta oscuro del logo */
  --color-fondo: #F0F2F5;           /* Gris LinkedIn-like (ligeramente más oscuro) */
  --color-texto: #2D3436;           /* Texto principal */
  /* === NUEVAS variables para estructura LinkedIn === */
  --navbar-bg: linear-gradient(135deg, #A62677 0%, #6B2D5C 100%);
  --navbar-height: 52px;
  --sidebar-width: 225px;
  --right-panel-width: 300px;
  --card-bg: #FFFFFF;
  --card-border: #E0E0E0;
  --card-radius: 10px;
  --card-shadow: 0 1px 3px rgba(0,0,0,0.08);
  --hover-bg: rgba(166, 38, 119, 0.06);
  --active-indicator: #A62677;
  --text-secondary: #636E72;
  --text-muted: #B2BEC3;
  --divider: #E8E8E8;
  /* === Bottom Tab (solo móvil) === */
  --tab-bar-bg: #FFFFFF;
  --tab-bar-height: 56px;
  --tab-active-color: #A62677;
  --tab-inactive-color: #636E72;
}
