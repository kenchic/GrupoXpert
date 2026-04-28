using GrupoXpert.Domain.Identidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoXpert.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de EF Core para la entidad Usuario.
/// </summary>
public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.NombreUsuario)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(u => u.NombreUsuario)
            .IsUnique();

        builder.Property(u => u.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.Imagen)
            .HasMaxLength(500);

        builder.Property(u => u.EstaActivo)
            .IsRequired();

        builder.Property(u => u.FechaCreacion)
            .IsRequired();

        builder.Property(u => u.UltimoInicioSesion);

        // Mapeo del Objeto de Valor ClaveAcceso (Owned Entity)
        builder.OwnsOne(u => u.Clave, clave =>
        {
            clave.Property(c => c.HashClave)
                .HasColumnName("HashClave")
                .HasMaxLength(500)
                .IsRequired();
        });
    }
}

