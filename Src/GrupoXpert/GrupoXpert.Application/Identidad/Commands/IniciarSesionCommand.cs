using GrupoXpert.Application.Identidad.Dtos;
using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

/// <summary>
/// Comando para iniciar sesión en el sistema.
/// </summary>
/// <param name="Email">Correo electrónico del usuario.</param>
/// <param name="Clave">Clave en texto plano.</param>
public sealed record IniciarSesionCommand(string Email, string Clave) : IRequest<ResultadoSesionDto>;
