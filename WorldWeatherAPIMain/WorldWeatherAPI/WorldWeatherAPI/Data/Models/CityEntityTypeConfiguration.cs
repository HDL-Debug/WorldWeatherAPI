using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorldWeatherAPI.Data.Models
{
    public class CityEntityTypeConfiguration
        : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("Cities");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder
                .HasOne(x => x.Country)
                .WithMany(x => x.Cities)
                .HasForeignKey(x => x.CountryId);
            builder.Property(x => x.Temp).HasColumnType("decimal(7,4)");
            builder.Property(x => x.Perci).HasColumnType("decimal(7,4)");
        }
    }
}