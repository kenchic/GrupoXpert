using GrupoXpert.Application.Calidad.Dtos;
using MediatR;

namespace GrupoXpert.Application.Calidad.Commands;

public sealed record OtorgarVistoBuenoCommand(Guid RevisionId) : IRequest<RevisionCalidadDto>;
