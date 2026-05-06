using System.Text;
using GrupoXpert.Application;
using GrupoXpert.Infrastructure;
using GrupoXpert.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GrupoXpert.Application.Identidad.Commands;

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
    
    builder.Services.AddOpenApi();
    
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
    
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR CRITICO EN STARTUP: {ex}");
    throw;
}
