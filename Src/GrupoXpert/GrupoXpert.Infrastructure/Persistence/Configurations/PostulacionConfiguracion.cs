using GrupoXpert.Domain.Academia;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoXpert.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de entidad para Postulacion en EF Core.
/// </summary>
public sealed class PostulacionConfiguracion : IEntityTypeConfiguration<Postulacion>
{
    public void Configure(EntityTypeBuilder<Postulacion> builder)
    {
        builder.ToTable("Postulaciones", "Academia");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.SolicitudId)
            .IsRequired();

        builder.Property(x => x.ColaboradorId)
            .IsRequired();

        builder.Property(x => x.FechaPostulacion)
            .IsRequired();

        builder.Property(x => x.Estado)
            .IsRequired()
            .HasConversion<int>();

        // Relación con PerfilColaborador
        builder.HasOne<GrupoXpert.Domain.Perfil.PerfilColaborador>()
            .WithMany()
            .HasForeignKey(x => x.ColaboradorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
