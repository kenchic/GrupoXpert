using GrupoXpert.Application.Academia.Dtos;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Perfil;
using MediatR;

namespace GrupoXpert.Application.Academia.Queries;

public sealed class ObtenerAvancesPorSolicitudHandler(
    IAvanceRepository avanceRepositorio,
    IPerfilColaboradorRepository perfilColaboradorRepositorio,
    IUsuarioRepository usuarioRepositorio)
    : IRequestHandler<ObtenerAvancesPorSolicitudQuery, IReadOnlyList<AvanceDto>>
{
    public async Task<IReadOnlyList<AvanceDto>> Handle(
        ObtenerAvancesPorSolicitudQuery consulta,
        CancellationToken cancelacion)
    {
        var avances = await avanceRepositorio.ObtenerPorSolicitudAsync(consulta.SolicitudId, cancelacion);

        var resultado = new List<AvanceDto>();

        foreach (var avance in avances)
        {
            var nombreAsesor = await ObtenerNombreUsuarioAsync(avance.AsesorId, cancelacion);

            var comentariosDto = new List<ComentarioDto>();
            foreach (var comentario in avance.Comentarios)
            {
                var nombreAutor = await ObtenerNombreUsuarioDirectoAsync(comentario.AutorId, cancelacion);
                comentariosDto.Add(new ComentarioDto(
                    comentario.Id,
                    comentario.AvanceId,
                    comentario.AutorId,
                    nombreAutor,
                    comentario.Contenido,
                    comentario.FechaCreacion));
            }

            var archivosDto = avance.ArchivosAdjuntos.Select(a => new ArchivoAdjuntoDto(
                a.Id,
                a.AvanceId,
                a.NombreArchivo,
                a.Url,
                a.TamanioBytes,
                a.TipoContenido,
                a.FechaSubida)).ToList();

            resultado.Add(new AvanceDto(
                avance.Id,
                avance.SolicitudId,
                avance.AsesorId,
                nombreAsesor,
                avance.Descripcion,
                avance.NumeroFase,
                (int)avance.Tipo,
                (int)avance.Estado,
                avance.FechaSubida,
                comentariosDto,
                archivosDto));
        }

        return resultado;
    }

    private async Task<string> ObtenerNombreUsuarioAsync(Guid perfilColaboradorId, CancellationToken cancelacion)
    {
        var perfil = await perfilColaboradorRepositorio.ObtenerPorIdAsync(perfilColaboradorId, cancelacion);
        if (perfil is null) return "Desconocido";

        var usuario = await usuarioRepositorio.ObtenerPorIdAsync(perfil.UsuarioId, cancelacion);
        return usuario?.Nombre ?? "Desconocido";
    }

    private async Task<string> ObtenerNombreUsuarioDirectoAsync(Guid usuarioId, CancellationToken cancelacion)
    {
        var usuario = await usuarioRepositorio.ObtenerPorIdAsync(usuarioId, cancelacion);
        return usuario?.Nombre ?? "Desconocido";
    }
}
