using GrupoXpert.Domain.Academia;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoXpert.Infrastructure.Persistence.Configurations;

public sealed class ComentarioConfiguracion : IEntityTypeConfiguration<Comentario>
{
    public void Configure(EntityTypeBuilder<Comentario> builder)
    {
        builder.ToTable("Comentarios", "Academia");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AvanceId)
            .IsRequired();

        builder.Property(x => x.AutorId)
            .IsRequired();

        builder.Property(x => x.Contenido)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.FechaCreacion)
            .IsRequired();
    }
}
