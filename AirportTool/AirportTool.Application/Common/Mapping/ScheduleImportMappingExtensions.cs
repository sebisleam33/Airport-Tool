using AirportTool.Application.Schedules;
using AirportTool.Domain.Entities;
using AirportTool.Domain.Enums;

namespace AirportTool.Application.Mapping;

public static class ScheduleImportMappingExtensions
{
    public static FlightSchedule ToFlightSchedule(
        this ScheduleImportRowDto row,
        int flightId,
        int? gateId,
        int? aircraftId)
    {
        return new FlightSchedule
        {
            FlightId = flightId,
            ScheduledDepartureUtc = row.ScheduledDepartureUtc,
            ScheduledArrivalUtc = row.ScheduledArrivalUtc,
            GateId = gateId,
            AssignedAircraftId = aircraftId,
            Status = FlightScheduleStatus.Planned
        };
    }
}
