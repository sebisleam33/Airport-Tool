using AirportTool.Domain.Entities;

namespace AirportTool.Application.Common.Interfaces;

public interface ITicketQueryAccessor
{
    Task<IReadOnlyList<Ticket>> GetTicketsByFlightIdAsync(
        int flightId,
        CancellationToken cancellationToken = default);

    Task<bool> TicketHasBookingsAsync(
        long ticketId,
        CancellationToken cancellationToken = default);
}
