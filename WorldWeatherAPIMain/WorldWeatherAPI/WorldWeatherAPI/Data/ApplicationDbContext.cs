using Microsoft.EntityFrameworkCore;
using WorldWeatherAPI.Data.Models;

namespace WorldWeatherAPI.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext() : base()
        {
        }

        public ApplicationDbContext(DbContextOptions options)
         : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>().ToTable("Cities");
            modelBuilder.Entity<City>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<City>()
                .Property(x => x.Id).IsRequired();
            modelBuilder.Entity<City>()
                .Property(x => x.Temp).HasColumnType("decimal(7,4)");
            modelBuilder.Entity<City>()
                .Property(x => x.Perci).HasColumnType("decimal(7,4)");

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Country>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<Country>()
                .Property(x => x.Id).IsRequired();
            modelBuilder.Entity<City>()
                .HasOne(x => x.Country)
                .WithMany(y => y.Cities)
                .HasForeignKey(x => x.CountryId);

            // add the EntityTypeConfiguration classes
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly
                );
        }

        public DbSet<City> Cities => Set<City>();
        public DbSet<Country> Countries => Set<Country>();

    }
}