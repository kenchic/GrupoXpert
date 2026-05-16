using FluentAssertions;
using GrupoXpert.Domain.Perfil;
using GrupoXpert.Domain.Perfil.Events;
using Xunit;

namespace GrupoXpert.UnitTests.Domain;

public class PerfilClienteTests
{
    [Fact]
    public void Crear_ConUsuarioId_DebeInicializarConValoresPorDefecto()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        // Act
        var perfil = PerfilCliente.Crear(usuarioId);

        // Assert
        perfil.UsuarioId.Should().Be(usuarioId);
        perfil.NivelAcademico.Should().Be(NivelAcademico.NoEspecificado);
        perfil.UrgenciaEntrega.Should().Be(UrgenciaEntrega.NoEspecificada);
        perfil.Telefono.Should().BeNull();
        perfil.AreasInteres.Should().BeEmpty();
    }

    [Fact]
    public void ActualizarPerfil_ConNuevosDatos_DebeActualizarPropiedadesYRegistrarEvento()
    {
        // Arrange
        var perfil = PerfilCliente.Crear(Guid.NewGuid());
        var nivel = NivelAcademico.Pregrado;
        var urgencia = UrgenciaEntrega.Alta;
        var telefono = Telefono.Crear("+573001234567");

        // Act
        perfil.ActualizarPerfil(nivel, urgencia, telefono);

        // Assert
        perfil.NivelAcademico.Should().Be(nivel);
        perfil.UrgenciaEntrega.Should().Be(urgencia);
        perfil.Telefono.Should().Be(telefono);
        perfil.EventosDominio.Should().ContainItemsAssignableTo<PerfilClienteActualizadoEvent>();
    }

    [Fact]
    public void AgregarAreaInteres_AreaValida_DebeAgregarALista()
    {
        // Arrange
        var perfil = PerfilCliente.Crear(Guid.NewGuid());
        var area = "Inteligencia Artificial";

        // Act
        perfil.AgregarAreaInteres(area);

        // Assert
        perfil.AreasInteres.Should().Contain(area);
        perfil.AreasInteres.Should().HaveCount(1);
    }

    [Fact]
    public void AgregarAreaInteres_AreaVacia_DebeLanzarExcepcion()
    {
        // Arrange
        var perfil = PerfilCliente.Crear(Guid.NewGuid());

        // Act
        var accion = () => perfil.AgregarAreaInteres("");

        // Assert
        accion.Should().Throw<ArgumentException>();
    }
}
