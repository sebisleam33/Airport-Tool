using AirportTool.Domain.Enums;

namespace AirportTool.Domain.Entities;

public class FlightSchedule
{
    public int FlightScheduleId { get; set; }
    public int FlightId { get; set; }
    public DateTime ScheduledDepartureUtc { get; set; }
    public DateTime ScheduledArrivalUtc { get; set; }
    public int? GateId { get; set; }
    public int? AssignedAircraftId { get; set; }
    public FlightScheduleStatus Status { get; set; } = FlightScheduleStatus.Planned;

    public Flight Flight { get; set; } = null!;
    public Gate? Gate { get; set; }
    public Aircraft? AssignedAircraft { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
