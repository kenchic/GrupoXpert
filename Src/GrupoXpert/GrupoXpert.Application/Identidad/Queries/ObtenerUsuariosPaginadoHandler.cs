using GrupoXpert.Application.Common.Dtos;
using GrupoXpert.Application.Identidad.Dtos;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Identidad.Queries;

public sealed class ObtenerUsuariosPaginadoHandler(
    IUsuarioRepository usuarioRepository) : IRequestHandler<ObtenerUsuariosPaginadoQuery, ResultadoPaginadoDto<UsuarioListaDto>>
{
    public async Task<ResultadoPaginadoDto<UsuarioListaDto>> Handle(ObtenerUsuariosPaginadoQuery consulta, CancellationToken cancelacion)
    {
        var (usuarios, totalRegistros) = await usuarioRepository.ObtenerPaginadoAsync(
            consulta.Tipo,
            consulta.EstaAprobado,
            consulta.EstadoVerificacion,
            consulta.Pagina,
            consulta.TamanoPagina,
            cancelacion);

        var elementosDto = usuarios.Select(u => new UsuarioListaDto(
            u.Id,
            u.Nombre,
            u.Correo.Valor,
            u.Tipo,
            u.EstaActivo,
            u.EstaAprobado,
            u.EstadoVerificacion,
            u.FechaCreacion,
            u.FechaAprobacion
        )).ToList();

        return new ResultadoPaginadoDto<UsuarioListaDto>(
            elementosDto,
            totalRegistros,
            consulta.Pagina,
            consulta.TamanoPagina
        );
    }
}
