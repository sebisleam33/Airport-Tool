using AirportTool.Application.Flights;
using AirportTool.Domain.Entities;

namespace AirportTool.Application.Mapping;

public static class FlightMappingExtensions
{
    public static Flight ToFlight(
        this CreateFlightDto dto,
        int airlineId,
        int originAirportId,
        int destinationAirportId,
        int? defaultAircraftId)
    {
        return new Flight
        {
            AirlineId = airlineId,
            FlightNumber = dto.FlightNumber,
            OriginAirportId = originAirportId,
            DestinationAirportId = destinationAirportId,
            DefaultAircraftId = defaultAircraftId,
            IsActive = dto.IsActive
        };
    }

    public static FlightDto ToFlightDto(
        this Flight flight,
        Airline airline,
        Airport origin,
        Airport destination,
        Aircraft? defaultAircraft)
    {
        return new FlightDto
        {
            FlightId = flight.FlightId,
            AirlineIata = airline.IATACode,
            FlightNumber = flight.FlightNumber,
            OriginIata = origin.IATACode,
            DestinationIata = destination.IATACode,
            DefaultAircraftTail = defaultAircraft?.TailNumber,
            IsActive = flight.IsActive
        };
    }

    public static FlightScheduleDto ToFlightScheduleDto(
        this FlightSchedule schedule,
        Flight flight,
        Airline airline,
        Airport origin,
        Airport destination,
        Gate? gate,
        Aircraft? aircraft)
    {
        return new FlightScheduleDto
        {
            FlightScheduleId = schedule.FlightScheduleId,
            FlightId = schedule.FlightId,
            AirlineIata = airline.IATACode,
            FlightNumber = flight.FlightNumber,
            OriginIata = origin.IATACode,
            DestinationIata = destination.IATACode,
            ScheduledDepartureUtc = schedule.ScheduledDepartureUtc,
            ScheduledArrivalUtc = schedule.ScheduledArrivalUtc,
            GateCode = gate?.Code,
            AircraftTail = aircraft?.TailNumber,
            Status = schedule.Status.ToString()
        };
    }
}
