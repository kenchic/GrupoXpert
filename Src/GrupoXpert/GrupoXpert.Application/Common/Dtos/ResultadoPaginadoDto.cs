namespace GrupoXpert.Application.Common.Dtos;

public sealed record ResultadoPaginadoDto<T>(
    IReadOnlyList<T> Elementos,
    int TotalRegistros,
    int Pagina,
    int TamanoPagina
);
