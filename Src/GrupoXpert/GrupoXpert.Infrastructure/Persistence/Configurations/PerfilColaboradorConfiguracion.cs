using GrupoXpert.Domain.Perfil;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoXpert.Infrastructure.Persistence.Configurations;

public class PerfilColaboradorConfiguracion : IEntityTypeConfiguration<PerfilColaborador>
{
    public void Configure(EntityTypeBuilder<PerfilColaborador> builder)
    {
        builder.ToTable("PerfilesColaboradores", "Perfil");

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.UsuarioId).IsUnique();

        builder.Property(p => p.UsuarioId).IsRequired();
        builder.Property(p => p.ValidadoPorAdmin).IsRequired();
        builder.Property(p => p.NivelAcademico).HasConversion<int>().IsRequired();
        builder.Property(p => p.DisponibilidadHorasSemana).IsRequired();
        builder.Property(p => p.CargaAcademicaIdeal).IsRequired();
        builder.Property(p => p.EvaluacionCalidad).HasColumnType("decimal(3,1)").IsRequired();

        // Configuración de AreasConocimiento
        builder.OwnsMany<AreaConocimiento>("_areasConocimiento", area =>
        {
            area.ToTable("PerfilesColaboradores_AreasConocimiento", "Perfil");
            area.WithOwner().HasForeignKey("PerfilColaboradorId");
            area.Property<int>("Id").ValueGeneratedOnAdd();
            area.HasKey("PerfilColaboradorId", "Id");
            area.Property(x => x.Area).HasColumnName("Area").HasMaxLength(100).IsRequired();
        });

        // Configuración de TiposTrabajo
        builder.OwnsMany<TipoTrabajo>("_tiposTrabajo", tipo =>
        {
            tipo.ToTable("PerfilesColaboradores_TiposTrabajo", "Perfil");
            tipo.WithOwner().HasForeignKey("PerfilColaboradorId");
            tipo.Property<int>("Id").ValueGeneratedOnAdd();
            tipo.HasKey("PerfilColaboradorId", "Id");
            tipo.Property(x => x.Tipo).HasColumnName("Tipo").HasMaxLength(100).IsRequired();
        });

        // Configuración de Idiomas
        builder.OwnsMany<Idioma>("_idiomas", idioma =>
        {
            idioma.ToTable("PerfilesColaboradores_Idiomas", "Perfil");
            idioma.WithOwner().HasForeignKey("PerfilColaboradorId");
            idioma.Property<int>("Id").ValueGeneratedOnAdd();
            idioma.HasKey("PerfilColaboradorId", "Id");
            idioma.Property(x => x.Nombre).HasColumnName("Nombre").HasMaxLength(50).IsRequired();
        });

        // Configuración de NormasCitacion
        builder.OwnsMany<NormaCitacion>("_normasCitacion", norma =>
        {
            norma.ToTable("PerfilesColaboradores_NormasCitacion", "Perfil");
            norma.WithOwner().HasForeignKey("PerfilColaboradorId");
            norma.Property<int>("Id").ValueGeneratedOnAdd();
            norma.HasKey("PerfilColaboradorId", "Id");
            norma.Property(x => x.Norma).HasColumnName("Norma").HasMaxLength(50).IsRequired();
        });
    }
}
