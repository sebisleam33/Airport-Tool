using AirportTool.Application.Common;
using AirportTool.Application.Common.Interfaces;
using AirportTool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AirportTool.Application.Flights;

public class FlightService : IFlightService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<FlightService> _logger;

    public FlightService(IUnitOfWork uow, ILogger<FlightService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<int> CreateFlightAsync(CreateFlightDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.OriginIata == dto.DestinationIata)
            throw new InvalidOperationException("Origin and destination must differ.");

        var flight = new Flight
        {
            AirlineId = 0,
            FlightNumber = dto.FlightNumber,
            OriginAirportId = 0,
            DestinationAirportId = 0,
            IsActive = dto.IsActive
        };

        await _uow.Flights.AddAsync(flight, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Flight created {FlightId}", flight.FlightId);

        return flight.FlightId;
    }

    public async Task<FlightDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var flight = await _uow.Flights.GetByIdAsync(id, cancellationToken);
        if (flight == null) return null;

        return new FlightDto
        {
            FlightId = flight.FlightId,
            AirlineIata = flight.Airline.IATACode,
            FlightNumber = flight.FlightNumber,
            OriginIata = flight.OriginAirport.IATACode,
            DestinationIata = flight.DestinationAirport.IATACode,
            IsActive = flight.IsActive
        };
    }

    public async Task<PagedResult<FlightScheduleDto>> SearchSchedulesAsync(
        string? originIata,
        string? destinationIata,
        DateTime? date,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (originIata is null || destinationIata is null || date is null)
            return new PagedResult<FlightScheduleDto> { Page = page, PageSize = pageSize, TotalCount = 0 };

        var schedules = await _uow.Flights.SearchSchedulesAsync(
            originIata, destinationIata, date.Value.Date, page, pageSize, cancellationToken);

        var items = schedules.Select(fs => new FlightScheduleDto
        {
            FlightScheduleId = fs.FlightScheduleId,
            AirlineIata = fs.Flight.Airline.IATACode,
            FlightNumber = fs.Flight.FlightNumber,
            OriginIata = fs.Flight.OriginAirport.IATACode,
            DestinationIata = fs.Flight.DestinationAirport.IATACode,
            ScheduledDepartureUtc = fs.ScheduledDepartureUtc,
            ScheduledArrivalUtc = fs.ScheduledArrivalUtc,
            GateCode = fs.Gate?.Code,
            AircraftTail = fs.AssignedAircraft?.TailNumber ?? fs.Flight.DefaultAircraft?.TailNumber,
            Status = fs.Status.ToString()
        }).ToList();

        return new PagedResult<FlightScheduleDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = items.Count
        };
    }
}
