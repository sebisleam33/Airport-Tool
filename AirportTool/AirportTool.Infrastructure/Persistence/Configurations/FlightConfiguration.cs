using AirportTool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirportTool.Infrastructure.Persistence.Configurations;

public class FlightConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        builder.HasKey(f => f.FlightId);

        builder.Property(f => f.FlightNumber)
            .IsRequired()
            .HasMaxLength(8);

        builder.HasIndex(f => new { f.AirlineId, f.FlightNumber });
        builder.HasIndex(f => new { f.OriginAirportId, f.DestinationAirportId });

        builder.Property(f => f.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(f => f.Airline)
            .WithMany(a => a.Flights)
            .HasForeignKey(f => f.AirlineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.OriginAirport)
            .WithMany(a => a.OriginFlights)
            .HasForeignKey(f => f.OriginAirportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.DestinationAirport)
            .WithMany(a => a.DestinationFlights)
            .HasForeignKey(f => f.DestinationAirportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.DefaultAircraft)
            .WithMany(a => a.Flights)
            .HasForeignKey(f => f.DefaultAircraftId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
