using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Calificacion;
using GrupoXpert.Domain.Perfil;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoXpert.Infrastructure.Persistence.Configurations;

public sealed class CalificacionColaboradorConfiguracion : IEntityTypeConfiguration<CalificacionColaborador>
{
    public void Configure(EntityTypeBuilder<CalificacionColaborador> builder)
    {
        builder.ToTable("CalificacionesColaborador", "Calificacion");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Puntaje, puntaje =>
        {
            puntaje.Property(p => p.Valor)
                .HasColumnName("Puntaje")
                .IsRequired();
        });

        builder.Property(x => x.SolicitudId).IsRequired();
        builder.Property(x => x.ClienteId).IsRequired();
        builder.Property(x => x.ColaboradorId).IsRequired();
        builder.Property(x => x.Observacion).HasMaxLength(1000);
        builder.Property(x => x.FechaCalificacion)
            .IsRequired()
            .HasConversion(
                v => new DateTimeOffset(v, TimeSpan.Zero),
                v => v.UtcDateTime);

        builder.HasIndex(x => x.SolicitudId).IsUnique();

        builder.HasOne<SolicitudAcademica>()
            .WithMany()
            .HasForeignKey(x => x.SolicitudId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PerfilCliente>()
            .WithMany()
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PerfilColaborador>()
            .WithMany()
            .HasForeignKey(x => x.ColaboradorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(x => x.EventosDominio);
    }
}