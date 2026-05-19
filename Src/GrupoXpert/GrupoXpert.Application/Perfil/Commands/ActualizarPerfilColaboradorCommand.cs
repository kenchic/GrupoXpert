using System;
using System.Collections.Generic;
using MediatR;
using GrupoXpert.Domain.Perfil;

namespace GrupoXpert.Application.Perfil.Commands;

public record ActualizarPerfilColaboradorCommand(
    Guid UsuarioId,
    int NivelAcademico,
    int DisponibilidadHorasSemana,
    int CargaAcademicaIdeal,
    List<string> AreasConocimiento,
    List<string> TiposTrabajo,
    List<string> Idiomas,
    List<string> NormasCitacion
) : IRequest<Unit>;
