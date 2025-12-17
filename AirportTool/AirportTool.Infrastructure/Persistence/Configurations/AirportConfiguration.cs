using AirportTool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirportTool.Infrastructure.Persistence.Configurations;

public class AirportConfiguration : IEntityTypeConfiguration<Airport>
{
    public void Configure(EntityTypeBuilder<Airport> builder)
    {
        builder.HasKey(a => a.AirportId);

        builder.Property(a => a.IATACode)
            .IsRequired()
            .HasColumnType("nchar(3)");

        builder.HasIndex(a => a.IATACode)
            .IsUnique();

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(a => a.TimeZone)
            .IsRequired()
            .HasMaxLength(64);
    }
}
