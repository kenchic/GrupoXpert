using System.Linq;
using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Perfil.Events;

namespace GrupoXpert.Domain.Perfil;

public class PerfilCliente : AggregateRoot
{
    public Guid UsuarioId { get; private set; }
    public NivelAcademico NivelAcademico { get; private set; }
    public UrgenciaEntrega UrgenciaEntrega { get; private set; }
    public Telefono? Telefono { get; private set; }
    
    private readonly List<AreaInteres> _areasInteres = new();
    public IReadOnlyList<string> AreasInteres => _areasInteres.Select(x => x.Area).ToList().AsReadOnly();

    private PerfilCliente() { } // Para EF Core

    private PerfilCliente(Guid id, Guid usuarioId, NivelAcademico nivelAcademico, UrgenciaEntrega urgenciaEntrega, Telefono? telefono) : base(id)
    {
        UsuarioId = usuarioId;
        NivelAcademico = nivelAcademico;
        UrgenciaEntrega = urgenciaEntrega;
        Telefono = telefono;
    }

    public static PerfilCliente Crear(Guid usuarioId)
    {
        return new PerfilCliente(Guid.NewGuid(), usuarioId, NivelAcademico.NoEspecificado, UrgenciaEntrega.NoEspecificada, null);
    }

    public void ActualizarPerfil(NivelAcademico nivelAcademico, UrgenciaEntrega urgenciaEntrega, Telefono? telefono)
    {
        NivelAcademico = nivelAcademico;
        UrgenciaEntrega = urgenciaEntrega;
        Telefono = telefono;

        AgregarEventoDominio(new PerfilClienteActualizadoEvent(Id));
    }

    public void AgregarAreaInteres(string area)
    {
        if (string.IsNullOrWhiteSpace(area))
            throw new ArgumentException("El área de interés no puede estar vacía.");

        if (!_areasInteres.Any(x => x.Area == area))
        {
            _areasInteres.Add(new AreaInteres(area));
        }
    }

    public void RemoverAreaInteres(string area)
    {
        var item = _areasInteres.FirstOrDefault(x => x.Area == area);
        if (item != null)
        {
            _areasInteres.Remove(item);
        }
    }
}
