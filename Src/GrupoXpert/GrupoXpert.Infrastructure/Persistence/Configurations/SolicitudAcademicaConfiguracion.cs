using GrupoXpert.Domain.Academia;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoXpert.Infrastructure.Persistence.Configurations;

public sealed class SolicitudAcademicaConfiguracion : IEntityTypeConfiguration<SolicitudAcademica>
{
    public void Configure(EntityTypeBuilder<SolicitudAcademica> builder)
    {
        builder.ToTable("SolicitudesAcademicas", "Academia");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NivelAcademico)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.TipoTrabajo)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.AreaTematica)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.FechaEntrega)
            .IsRequired()
            .HasConversion(
                v => new DateTimeOffset(v, TimeSpan.Zero),
                v => v.UtcDateTime
            );

        builder.Property(x => x.NumeroPaginasOPalabras)
            .IsRequired();

        builder.Property(x => x.NormaCitacion)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Idioma)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.FormatoRequerido)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.MaterialBase)
            .IsRequired();
            
        builder.Property(x => x.ClienteId)
            .IsRequired();
            
        builder.Property(x => x.EsUrgente)
            .IsRequired();
            
        builder.Property(x => x.EntregaPorFases)
            .IsRequired();

        builder.Property(x => x.Estado)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(EstadoSolicitud.Pendiente);

        builder.Property(x => x.AsesorId)
            .IsRequired(false);

        builder.HasOne<GrupoXpert.Domain.Perfil.PerfilColaborador>()
            .WithMany()
            .HasForeignKey(x => x.AsesorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(x => x.EventosDominio);
    }
}
