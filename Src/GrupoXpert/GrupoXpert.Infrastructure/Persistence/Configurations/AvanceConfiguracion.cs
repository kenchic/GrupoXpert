using GrupoXpert.Domain.Academia;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoXpert.Infrastructure.Persistence.Configurations;

public sealed class AvanceConfiguracion : IEntityTypeConfiguration<Avance>
{
    public void Configure(EntityTypeBuilder<Avance> builder)
    {
        builder.ToTable("Avances", "Academia");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SolicitudId)
            .IsRequired();

        builder.Property(x => x.AsesorId)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.NumeroFase)
            .IsRequired();

        builder.Property(x => x.Tipo)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Estado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.FechaSubida)
            .IsRequired()
            .HasConversion(
                v => new DateTimeOffset(v, TimeSpan.Zero),
                v => v.UtcDateTime
            );

        builder.HasOne<GrupoXpert.Domain.Perfil.PerfilColaborador>()
            .WithMany()
            .HasForeignKey(x => x.AsesorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Comentarios)
            .WithOne()
            .HasForeignKey(x => x.AvanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Comentarios)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.ArchivosAdjuntos)
            .WithOne()
            .HasForeignKey(x => x.AvanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.ArchivosAdjuntos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(x => x.EventosDominio);
    }
}
