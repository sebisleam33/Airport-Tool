using AirportTool.Application.Common;

namespace AirportTool.Application.Flights;

public interface IFlightService
{
    Task<int> CreateFlightAsync(CreateFlightDto dto, CancellationToken cancellationToken = default);
    Task<FlightDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<FlightScheduleDto>> SearchSchedulesAsync(
        string? originIata,
        string? destinationIata,
        DateTime? date,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
