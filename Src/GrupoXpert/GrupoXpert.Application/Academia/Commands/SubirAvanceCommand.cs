using GrupoXpert.Application.Academia.Dtos;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed record ArchivoAdjuntoInput(
    string NombreArchivo,
    string TipoContenido,
    long TamanioBytes,
    Stream Contenido
);

public sealed record SubirAvanceCommand(
    Guid SolicitudId,
    Guid AsesorId,
    string Descripcion,
    int NumeroFase,
    int Tipo,
    IReadOnlyList<ArchivoAdjuntoInput> Archivos
) : IRequest<AvanceDto>;
