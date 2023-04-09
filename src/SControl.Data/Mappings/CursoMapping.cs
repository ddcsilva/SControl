using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SControl.Business.Models;

namespace SControl.Data.Mappings;

public class CursoMapping : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.Property(c => c.DuracaoPorSemestre)
            .IsRequired()
            .HasColumnType("int");

        builder.Property(c => c.Modalidade)
            .IsRequired()
            .HasColumnType("varchar(50)");

        builder.Property(c => c.Ativo)
            .IsRequired()
            .HasColumnType("bit");

        builder.ToTable("Cursos");
    }
}
