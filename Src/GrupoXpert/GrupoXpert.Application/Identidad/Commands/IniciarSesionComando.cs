using GrupoXpert.Application.Identidad.Dtos;
using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

/// <summary>
/// Comando para iniciar sesión en la aplicación.
/// </summary>
public record IniciarSesionComando(
    string NombreUsuario,
    string Clave) : IRequest<ResultadoSesionDto>;

