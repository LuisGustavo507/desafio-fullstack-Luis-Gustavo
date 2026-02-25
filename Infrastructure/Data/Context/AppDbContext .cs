using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Cidade> Cidades => Set<Cidade>();
        public DbSet<RegistroClima> Climas => Set<RegistroClima>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                modelBuilder.Entity<RegistroClima>(entity =>
                {
                    entity.Property(e => e.DataHoraRegistro)
                        .HasColumnType("timestamp without time zone");
                });
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}