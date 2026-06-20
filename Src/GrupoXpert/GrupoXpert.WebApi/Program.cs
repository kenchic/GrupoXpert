using System.Text;
using GrupoXpert.Application;
using GrupoXpert.Application.Identidad.Commands;
using GrupoXpert.Application.Perfil.Commands;
using GrupoXpert.Application.Perfil.Dtos;
using GrupoXpert.Application.Perfil.Queries;
using GrupoXpert.Infrastructure;
using GrupoXpert.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // 1. Registro de Capas (DDD)
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // 2. Configuración de Autenticación JWT
    var secreto = builder.Configuration["Jwt:Secreto"] ?? "GrupoXpertSecretoSuperSeguro2026";
    var emisor = builder.Configuration["Jwt:Emisor"] ?? "GrupoXpert";
    var audiencia = builder.Configuration["Jwt:Audiencia"] ?? "GrupoXpertUsers";

    builder.Services.AddAuthentication(opciones =>
    {
        opciones.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opciones.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = emisor,
            ValidAudience = audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secreto))
        };
    });

    builder.Services.AddAuthorization();

    builder.Services.AddOpenApi();
    builder.Services.AddControllers();

    var app = builder.Build();

    // 3. Pipeline de solicitudes
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    // app.UseHttpsRedirection(); // Comentado para desarrollo local sin SSL

    app.MapGet("/", () => Results.Ok(new { Mensaje = "GrupoXpert API Funcionando", Entorno = app.Environment.EnvironmentName }));

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseStaticFiles(); // wwwroot por defecto

    var rutaUploads = Path.Combine(builder.Environment.ContentRootPath, "uploads");
    if (!Directory.Exists(rutaUploads))
    {
        Directory.CreateDirectory(rutaUploads);
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(rutaUploads),
        RequestPath = "/uploads"
    });

    // 4. Endpoints de Identidad (Minimal APIs)
    var authGroup = app.MapGroup("/api/autenticacion").WithTags("Autenticación");

    authGroup.MapPost("/iniciar-sesion", async (IniciarSesionCommand comando, ISender mediador, CancellationToken ct) =>
    {
        try
        {
            var resultado = await mediador.Send(comando, ct);
            return Results.Ok(resultado);
        }
        catch (Exception ex)
        {
            return Results.Json(new { mensaje = ex.Message }, statusCode: 401);
        }
    }).AllowAnonymous();

    authGroup.MapPost("/registrar", async ([Microsoft.AspNetCore.Mvc.FromBody] CrearUsuarioCommand comando, ISender mediador, CancellationToken ct) =>
    {
        if (comando == null)
            return Results.BadRequest(new { mensaje = "Los datos del usuario son obligatorios." });

        try
        {
            var usuarioId = await mediador.Send(comando, ct);
            return Results.Created($"/api/usuarios/{usuarioId}", new { mensaje = "Usuario registrado exitosamente." });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { mensaje = ex.Message });
        }
    }).AllowAnonymous();

    authGroup.MapGet("/diagnostico", async (AppDbContext contexto) =>
    {
        try
        {
            var puedeConectar = await contexto.Database.CanConnectAsync();
            var conteo = await contexto.Usuarios.CountAsync();
            return Results.Ok(new { Conectado = puedeConectar, Usuarios = conteo, Database = contexto.Database.GetDbConnection().Database });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }).AllowAnonymous();

    authGroup.MapGet("/activar", async (string token, ISender mediador, CancellationToken ct) =>
    {
        try
        {
            await mediador.Send(new ActivarCuentaCommand(token), ct);
            return Results.Ok(new { mensaje = "Cuenta activada exitosamente." });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { mensaje = ex.Message });
        }
    }).AllowAnonymous();

    // 5. Endpoints de Administración (Protegidos)
    var adminGroup = app.MapGroup("/api/admin")
        .WithTags("Administración")
        .RequireAuthorization(policy => policy.RequireRole("Administrador"));

    adminGroup.MapGet("/usuarios", async ([Microsoft.AspNetCore.Mvc.FromQuery] GrupoXpert.Domain.Identidad.TipoUsuario? tipo, [Microsoft.AspNetCore.Mvc.FromQuery] bool? estaAprobado, [Microsoft.AspNetCore.Mvc.FromQuery] GrupoXpert.Domain.Identidad.EstadoVerificacion? estadoVerificacion, [Microsoft.AspNetCore.Mvc.FromQuery] int pagina, [Microsoft.AspNetCore.Mvc.FromQuery] int tamanoPagina, ISender mediador, CancellationToken ct) =>
    {
        var consulta = new GrupoXpert.Application.Identidad.Queries.ObtenerUsuariosPaginadoQuery(tipo, estaAprobado, estadoVerificacion, pagina > 0 ? pagina : 1, tamanoPagina > 0 ? tamanoPagina : 20);
        var resultado = await mediador.Send(consulta, ct);
        return Results.Ok(resultado);
    });

    adminGroup.MapPost("/colaboradores/{id:guid}/aprobar", async (Guid id, System.Security.Claims.ClaimsPrincipal user, ISender mediador, CancellationToken ct) =>
    {
        try
        {
            var adminIdString = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(adminIdString, out var adminId)) return Results.Unauthorized();

            await mediador.Send(new AprobarColaboradorCommand(id, adminId), ct);
            return Results.Ok(new { mensaje = "Colaborador aprobado exitosamente." });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { mensaje = ex.Message });
        }
    });

    adminGroup.MapPost("/colaboradores/{id:guid}/revocar", async (Guid id, System.Security.Claims.ClaimsPrincipal user, ISender mediador, CancellationToken ct) =>
    {
        try
        {
            var adminIdString = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(adminIdString, out var adminId)) return Results.Unauthorized();

            await mediador.Send(new RevocarAprobacionCommand(id, adminId), ct);
            return Results.Ok(new { mensaje = "Aprobación revocada exitosamente." });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { mensaje = ex.Message });
        }
    });

    adminGroup.MapPost("/colaboradores/{id:guid}/validar-perfil", async (Guid id, System.Security.Claims.ClaimsPrincipal user, ISender mediador, CancellationToken ct) =>
    {
        try
        {
            var adminIdString = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(adminIdString, out var adminId)) return Results.Unauthorized();

            await mediador.Send(new ValidarPerfilColaboradorCommand(id, adminId), ct);
            return Results.Ok(new { mensaje = "Perfil enviado a validación exitosamente." });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { mensaje = ex.Message });
        }
    });
    
    // 6. Endpoints de Perfil (Protegidos)
    var perfilGroup = app.MapGroup("/api/perfil")
        .WithTags("Perfil")
        .RequireAuthorization();

    perfilGroup.MapGet("/", async (System.Security.Claims.ClaimsPrincipal user, ISender mediador, CancellationToken ct) =>
    {
        var usuarioIdString = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(usuarioIdString, out var usuarioId)) return Results.Unauthorized();

        var perfil = await mediador.Send(new ObtenerPerfilPorUsuarioQuery(usuarioId), ct);
        return perfil is not null ? Results.Ok(perfil) : Results.NotFound(new { mensaje = "El perfil no existe." });
    });

    perfilGroup.MapPost("/", async (ActualizarPerfilCommand comando, System.Security.Claims.ClaimsPrincipal user, ISender mediador, CancellationToken ct) =>
    {
        var usuarioIdString = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(usuarioIdString, out var usuarioId)) return Results.Unauthorized();

        // Aseguramos que el comando use el ID del usuario autenticado para seguridad
        var comandoSeguro = comando with { UsuarioId = usuarioId };
        
        try
        {
            await mediador.Send(comandoSeguro, ct);
            return Results.Ok(new { mensaje = "Perfil actualizado exitosamente." });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { mensaje = ex.Message });
        }
    });

    // 7. Endpoint de Dashboard Colaborador
    var dashboardGroup = app.MapGroup("/api/dashboard")
        .WithTags("Dashboard")
        .RequireAuthorization();

    dashboardGroup.MapGet("/colaborador", async (System.Security.Claims.ClaimsPrincipal user, ISender mediador, CancellationToken ct) =>
    {
        var usuarioIdString = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(usuarioIdString, out var usuarioId)) return Results.Unauthorized();

        var dashboard = await mediador.Send(new ObtenerDashboardColaboradorQuery(usuarioId), ct);
        return dashboard is not null ? Results.Ok(dashboard) : Results.NotFound(new { mensaje = "No se encontró el perfil de colaborador." });
    });

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR CRITICO EN STARTUP: {ex}");
    throw;
}
