using GrupoXpert.Domain.Academia;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoXpert.Infrastructure.Persistence.Configurations;

public sealed class ArchivoAdjuntoConfiguracion : IEntityTypeConfiguration<ArchivoAdjunto>
{
    public void Configure(EntityTypeBuilder<ArchivoAdjunto> builder)
    {
        builder.ToTable("ArchivosAdjuntos", "Academia");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AvanceId)
            .IsRequired();

        builder.Property(x => x.NombreArchivo)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.TamanioBytes)
            .IsRequired();

        builder.Property(x => x.TipoContenido)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.FechaSubida)
            .IsRequired();
    }
}
