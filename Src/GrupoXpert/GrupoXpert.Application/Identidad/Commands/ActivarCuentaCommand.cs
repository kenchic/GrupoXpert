using MediatR;

namespace GrupoXpert.Application.Identidad.Commands;

/// <summary>
/// Comando para activar una cuenta de usuario mediante un token.
/// </summary>
/// <param name="Token">Token de activación recibido por correo.</param>
public sealed record ActivarCuentaCommand(string Token) : IRequest<bool>;
