using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings
{
    public class CidadeMapping : IEntityTypeConfiguration<Cidade>
    {
        public void Configure(EntityTypeBuilder<Cidade> builder)
        {
            builder.ToTable("Cidade");

            builder.HasKey(c => c.CidadeId);

            builder.Property(c => c.CidadeId)
                   .ValueGeneratedOnAdd();

            builder.Property(c => c.Nome).HasMaxLength(100).IsRequired();

            builder.Property(c => c.Pais).HasMaxLength(100).IsRequired();
        }
    }
}
