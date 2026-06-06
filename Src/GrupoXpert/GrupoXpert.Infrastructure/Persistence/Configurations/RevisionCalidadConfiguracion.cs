using GrupoXpert.Domain.Calidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoXpert.Infrastructure.Persistence.Configurations;

public sealed class RevisionCalidadConfiguracion : IEntityTypeConfiguration<RevisionCalidad>
{
    public void Configure(EntityTypeBuilder<RevisionCalidad> builder)
    {
        builder.ToTable("RevisionesCalidad", "Calidad");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AvanceId).IsRequired();
        builder.Property(x => x.RevisorId).IsRequired();
        builder.Property(x => x.FechaRevision)
            .IsRequired()
            .HasConversion(
                v => new DateTimeOffset(v, TimeSpan.Zero),
                v => v.UtcDateTime);
        builder.Property(x => x.VistoBueno).IsRequired();
        builder.Property(x => x.Observaciones).HasMaxLength(1000);
        builder.Property(x => x.Estado).IsRequired().HasConversion<int>();

        builder.HasIndex(x => x.AvanceId).IsUnique();

        builder.HasOne<GrupoXpert.Domain.Academia.Avance>()
            .WithMany()
            .HasForeignKey(x => x.AvanceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<GrupoXpert.Domain.Identidad.Usuario>()
            .WithMany()
            .HasForeignKey(x => x.RevisorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(x => x.EventosDominio);
    }
}
