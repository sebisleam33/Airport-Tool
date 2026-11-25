using AirportTool.Application.Common.Interfaces;
using AirportTool.Domain.Entities;
using AirportTool.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AirportTool.Infrastructure.Repositories;

public class FlightRepository : Repository<Flight>, IFlightRepository
{
    private readonly AirportDbContext _context;

    public FlightRepository(AirportDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FlightSchedule>> SearchSchedulesAsync(
        string originIata,
        string destinationIata,
        DateTime dateUtc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var start = dateUtc.Date;
        var end = start.AddDays(1);

        var query = _context.FlightSchedules
            .Include(fs => fs.Flight)
                .ThenInclude(f => f.Airline)
            .Include(fs => fs.Flight.OriginAirport)
            .Include(fs => fs.Flight.DestinationAirport)
            .Include(fs => fs.Gate)
            .Include(fs => fs.AssignedAircraft)
            .Include(fs => fs.Flight.DefaultAircraft)
            .Where(fs =>
                fs.Flight.OriginAirport.IATACode == originIata &&
                fs.Flight.DestinationAirport.IATACode == destinationIata &&
                fs.ScheduledDepartureUtc >= start &&
                fs.ScheduledDepartureUtc < end);

        return await query
            .OrderBy(fs => fs.ScheduledDepartureUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
