namespace AirportTool.Application.Flights;

public class FlightDto
{
    public int FlightId { get; set; }
    public string AirlineIata { get; set; } = null!;
    public string FlightNumber { get; set; } = null!;
    public string OriginIata { get; set; } = null!;
    public string DestinationIata { get; set; } = null!;
    public bool IsActive { get; set; }
    public string? DefaultAircraftTail { get; set; } = null!;
}

public class CreateFlightDto
{
    public string AirlineIata { get; set; } = null!;
    public string FlightNumber { get; set; } = null!;
    public string OriginIata { get; set; } = null!;
    public string DestinationIata { get; set; } = null!;
    public string? DefaultAircraftTail { get; set; }
    public bool IsActive { get; set; } = true;
}

public class FlightScheduleDto
{
    public int FlightId { get; set; }
    public int FlightScheduleId { get; set; }
    public string AirlineIata { get; set; } = null!;
    public string FlightNumber { get; set; } = null!;
    public string OriginIata { get; set; } = null!;
    public string DestinationIata { get; set; } = null!;
    public DateTime ScheduledDepartureUtc { get; set; }
    public DateTime ScheduledArrivalUtc { get; set; }
    public string? GateCode { get; set; }
    public string? AircraftTail { get; set; }
    public string Status { get; set; } = null!;
}
