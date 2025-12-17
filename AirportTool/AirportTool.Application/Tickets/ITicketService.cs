using AirportTool.Application.Common;

namespace AirportTool.Application.Tickets;

public interface ITicketService
{
    Task<IReadOnlyList<TicketDto>> GetByFlightIdAsync(
        int flightId,
        CancellationToken cancellationToken = default);

    Task<long> CreateAsync(
        CreateTicketDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        long ticketId,
        UpdateTicketDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateInventoryAsync(
        long ticketId,
        UpdateTicketInventoryDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long ticketId,
        CancellationToken cancellationToken = default);
}
