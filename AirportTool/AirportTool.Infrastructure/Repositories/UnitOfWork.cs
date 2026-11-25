using AirportTool.Application.Common.Interfaces;
using AirportTool.Domain.Entities;
using AirportTool.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AirportTool.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IBookingQueryAccessor, ITicketQueryAccessor
{
    private readonly AirportDbContext _context;

    public IFlightRepository Flights { get; }
    public IRepository<FlightSchedule> FlightSchedules { get; }
    public IRepository<Ticket> Tickets { get; }
    public IRepository<Booking> Bookings { get; }

    public UnitOfWork(AirportDbContext context)
    {
        _context = context;
        Flights = new FlightRepository(context);
        FlightSchedules = new Repository<FlightSchedule>(context);
        Tickets = new Repository<Ticket>(context);
        Bookings = new Repository<Booking>(context);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public ValueTask DisposeAsync()
        => _context.DisposeAsync();

    public Task<Booking?> GetBookingWithTicketByCodeAsync(
        string confirmationCode,
        CancellationToken cancellationToken = default)
    {
        return _context.Bookings
            .Include(b => b.Ticket)
            .FirstOrDefaultAsync(b => b.ConfirmationCode == confirmationCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Ticket>> GetTicketsByFlightIdAsync(
    int flightId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Tickets
            .Where(t => t.FlightId == flightId)
            .OrderBy(t => t.FareClass)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> TicketHasBookingsAsync(
        long ticketId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .AnyAsync(b => b.TicketId == ticketId, cancellationToken);
    }
}
