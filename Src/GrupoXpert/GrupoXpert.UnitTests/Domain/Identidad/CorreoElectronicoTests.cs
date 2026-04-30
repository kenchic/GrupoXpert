using FluentAssertions;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using Xunit;

namespace GrupoXpert.UnitTests.Domain.Identidad;

/// <summary>
/// Pruebas unitarias del Objeto de Valor CorreoElectronico.
/// Validan las reglas de formato, normalización y unicidad semántica.
/// </summary>
public sealed class CorreoElectronicoTests
{
    // ═══════════════════════════════════════════════════════════════════════════
    // CREACIÓN VÁLIDA
    // ═══════════════════════════════════════════════════════════════════════════

    [Theory]
    [InlineData("usuario@grupoxpert.com")]
    [InlineData("ADMIN@GRUPOXPERT.COM")]
    [InlineData("nombre.apellido+tag@empresa.co")]
    [InlineData("test_123@sub.dominio.org")]
    public void Crear_CuandoEmailTieneFormatoValido_DebeCrearseCorrectamente(string email)
    {
        // Act
        var correo = CorreoElectronico.Crear(email);

        // Assert
        correo.Should().NotBeNull();
        correo.Valor.Should().Be(email.Trim().ToLowerInvariant(),
            "el correo debe normalizarse a minúsculas");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // VALIDACIONES DE FORMATO
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Crear_CuandoEmailEsVacio_DebeLanzarExcepcionDominio()
    {
        // Act
        var accion = () => CorreoElectronico.Crear(string.Empty);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("El correo electrónico es obligatorio.");
    }

    [Fact]
    public void Crear_CuandoEmailEsSoloEspacios_DebeLanzarExcepcionDominio()
    {
        // Act
        var accion = () => CorreoElectronico.Crear("   ");

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("El correo electrónico es obligatorio.");
    }

    [Theory]
    [InlineData("no-es-email")]
    [InlineData("@sin-usuario.com")]
    [InlineData("usuario@")]
    [InlineData("usuario@dominio")]
    [InlineData("usuario @dominio.com")]
    public void Crear_CuandoEmailTieneFormatoInvalido_DebeLanzarExcepcionDominio(string emailInvalido)
    {
        // Act
        var accion = () => CorreoElectronico.Crear(emailInvalido);

        // Assert
        accion.Should().Throw<ExcepcionDominio>();
    }

    [Fact]
    public void Crear_CuandoEmailExcede254Caracteres_DebeLanzarExcepcionDominio()
    {
        // Arrange — genera un email de 255 chars
        var nombreLargo = new string('a', 243);
        var emailLargo = $"{nombreLargo}@test.com"; // 243 + 9 = 252... ajustemos
        emailLargo = new string('a', 246) + "@t.co"; // 251 chars
        // Forzar uno de exactamente 255 caracteres
        var parteLocal = new string('x', 244);
        var emailDe255 = $"{parteLocal}@t.co"; // 244+5 = 249, añadir más
        var emailSuperLargo = new string('a', 248) + "@a.co"; // 253
        // Construir 255 chars netos
        emailSuperLargo = new string('b', 249) + "@b.co"; // 254, necesitamos 255
        var emailDe255Chars = new string('c', 250) + "@c.co"; // 255

        // Act
        var accion = () => CorreoElectronico.Crear(emailDe255Chars);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("El correo electrónico no puede exceder 254 caracteres.");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // IGUALDAD SEMÁNTICA (ValueObject)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Igualdad_CuandoDosCorreosTienenElMismoValor_DebenSerIguales()
    {
        // Arrange
        var correo1 = CorreoElectronico.Crear("admin@grupoxpert.com");
        var correo2 = CorreoElectronico.Crear("ADMIN@GRUPOXPERT.COM");

        // Assert
        correo1.Should().Be(correo2,
            "dos correos con el mismo valor normalizado deben ser iguales (semántica de Objeto de Valor)");
    }

    [Fact]
    public void Igualdad_CuandoDosCorreosSonDiferentes_NoDebenSerIguales()
    {
        // Arrange
        var correo1 = CorreoElectronico.Crear("admin@grupoxpert.com");
        var correo2 = CorreoElectronico.Crear("otro@grupoxpert.com");

        // Assert
        correo1.Should().NotBe(correo2);
    }

    [Fact]
    public void ToString_DebeRetornarElValorNormalizado()
    {
        // Arrange
        var correo = CorreoElectronico.Crear("ADMIN@GRUPOXPERT.COM");

        // Assert
        correo.ToString().Should().Be("admin@grupoxpert.com");
    }
}
