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