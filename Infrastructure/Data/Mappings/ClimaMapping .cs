using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Data.Mappings
{
    public class ClimaMapping : IEntityTypeConfiguration<RegistroClima>
    {
        public void Configure(EntityTypeBuilder<RegistroClima> builder)
        {

            builder.ToTable("RegistroClima");

            builder.HasKey(c => c.RegistroClimaId);

            builder.Property(c => c.RegistroClimaId)
                   .ValueGeneratedOnAdd();

            builder.Property(c => c.Latitude)
                   .HasPrecision(10, 6)
                   .IsRequired();

            builder.Property(c => c.Longitude)
                   .HasPrecision(10, 6)
                   .IsRequired();

            builder.Property(c => c.Temperatura)
                   .IsRequired();

            builder.Property(c => c.TemperaturaMin)
                   .IsRequired();

            builder.Property(c => c.TemperaturaMax)
                   .IsRequired();

            builder.Property(c => c.Condicao)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(c => c.DataHoraRegistro)
                   .IsRequired();
        }
   
    }
}
