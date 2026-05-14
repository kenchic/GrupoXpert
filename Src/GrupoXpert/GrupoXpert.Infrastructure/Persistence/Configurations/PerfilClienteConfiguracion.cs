using GrupoXpert.Domain.Perfil;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoXpert.Infrastructure.Persistence.Configurations;

public class PerfilClienteConfiguracion : IEntityTypeConfiguration<PerfilCliente>
{
    public void Configure(EntityTypeBuilder<PerfilCliente> builder)
    {
        builder.ToTable("PerfilesCliente", "Perfil");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UsuarioId)
            .IsRequired();

        builder.Property(p => p.NivelAcademico)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.UrgenciaEntrega)
            .HasConversion<int>()
            .IsRequired();

        // Configuración de Value Object Telefono
        builder.OwnsOne(p => p.Telefono, telefono =>
        {
            telefono.Property(t => t.Numero)
                .HasColumnName("Telefono")
                .HasMaxLength(50);
        });

        // Configuración de colección de objetos AreaInteres (mapeados a tabla separada con PK compuesta)
        builder.OwnsMany<AreaInteres>("_areasInteres", area =>
        {
            area.ToTable("PerfilClienteAreasInteres", "Perfil");
            area.WithOwner().HasForeignKey("PerfilClienteId");
            area.Property(x => x.Area).HasColumnName("Area").HasMaxLength(200);
            area.HasKey("PerfilClienteId", "Area");
        });
        
        // El DBA sugirió PK compuesta para AreasInteres, pero EF Core OwnsMany con strings a veces requiere un Shadow Property Id
        // Ajustamos para que coincida con la tabla física o lo más cercano
    }
}
