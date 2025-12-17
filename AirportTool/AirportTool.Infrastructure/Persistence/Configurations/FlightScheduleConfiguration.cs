using AirportTool.Domain.Entities;
using AirportTool.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirportTool.Infrastructure.Persistence.Configurations;

public class FlightScheduleConfiguration : IEntityTypeConfiguration<FlightSchedule>
{
    public void Configure(EntityTypeBuilder<FlightSchedule> builder)
    {
        builder.HasKey(fs => fs.FlightScheduleId);

        builder.Property(fs => fs.Status)
            .HasConversion<byte>();

        builder.HasIndex(fs => new { fs.FlightId, fs.ScheduledDepartureUtc });

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_FlightSchedule_ArrivalAfterDeparture",
                "[ScheduledArrivalUtc] > [ScheduledDepartureUtc]");
        });

        builder.HasOne(fs => fs.Flight)
            .WithMany(f => f.Schedules)
            .HasForeignKey(fs => fs.FlightId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(fs => fs.Gate)
            .WithMany(g => g.FlightSchedules)
            .HasForeignKey(fs => fs.GateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(fs => fs.AssignedAircraft)
            .WithMany(a => a.FlightSchedules)
            .HasForeignKey(fs => fs.AssignedAircraftId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
