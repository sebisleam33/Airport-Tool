using AirportTool.Domain.Entities;

namespace AirportTool.Application.Common.Interfaces;

public interface IBookingQueryAccessor
{
    Task<Booking?> GetBookingWithTicketByCodeAsync(
        string confirmationCode,
        CancellationToken cancellationToken = default);
}
