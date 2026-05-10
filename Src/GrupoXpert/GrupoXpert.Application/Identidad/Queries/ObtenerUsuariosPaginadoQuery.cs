using GrupoXpert.Application.Common.Dtos;
using GrupoXpert.Application.Identidad.Dtos;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Identidad.Queries;

public sealed record ObtenerUsuariosPaginadoQuery(
    TipoUsuario? Tipo = null,
    bool? EstaAprobado = null,
    EstadoVerificacion? EstadoVerificacion = null,
    int Pagina = 1,
    int TamanoPagina = 20
) : IRequest<ResultadoPaginadoDto<UsuarioListaDto>>;
