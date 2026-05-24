using GrupoXpert.Application.Academia.Dtos;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed record SubirAvanceCommand(
    Guid SolicitudId,
    Guid AsesorId,
    string Descripcion,
    int NumeroFase,
    int Tipo
) : IRequest<AvanceDto>;
