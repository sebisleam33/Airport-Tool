using AirportTool.Domain.Entities;

namespace AirportTool.Application.Common.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IFlightRepository Flights { get; }
    IRepository<FlightSchedule> FlightSchedules { get; }
    IRepository<Ticket> Tickets { get; }
    IRepository<Booking> Bookings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
