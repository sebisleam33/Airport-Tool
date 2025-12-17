using AirportTool.Domain.Entities;

namespace AirportTool.Application.Common.Interfaces;

public interface IFlightRepository : IRepository<Flight>
{
    Task<IReadOnlyList<FlightSchedule>> SearchSchedulesAsync(
        string originIata,
        string destinationIata,
        DateTime dateUtc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
