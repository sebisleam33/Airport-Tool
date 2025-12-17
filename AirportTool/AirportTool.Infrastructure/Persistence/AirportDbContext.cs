using AirportTool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AirportTool.Infrastructure.Persistence;

public class AirportDbContext : DbContext
{
    public AirportDbContext(DbContextOptions<AirportDbContext> options) : base(options) { }

    public DbSet<Airline> Airlines => Set<Airline>();
    public DbSet<Airport> Airports => Set<Airport>();
    public DbSet<Gate> Gates => Set<Gate>();
    public DbSet<Aircraft> Aircraft => Set<Aircraft>();
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<FlightSchedule> FlightSchedules => Set<FlightSchedule>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AirportDbContext).Assembly);

        modelBuilder.Entity<Airline>().HasData(
            new Airline { AirlineId = 1, IATACode = "RO", Name = "TAROM" }
        );

        modelBuilder.Entity<Airport>().HasData(
            new Airport { AirportId = 1, IATACode = "OTP", Name = "Otopeni", City = "Bucharest", Country = "Romania", TimeZone = "Europe/Bucharest" },
            new Airport { AirportId = 2, IATACode = "LHR", Name = "Heathrow", City = "London", Country = "UK", TimeZone = "Europe/London" }
        );

        modelBuilder.Entity<Gate>().HasData(
            new Gate { GateId = 1, AirportId = 1, Code = "A12" }
        );

        modelBuilder.Entity<Aircraft>().HasData(
            new Aircraft { AircraftId = 1, TailNumber = "YR-BGA", Model = "Boeing 737", SeatCapacity = 180 }
        );
    }
}
