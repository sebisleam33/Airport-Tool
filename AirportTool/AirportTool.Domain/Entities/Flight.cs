using System.Net.Sockets;

namespace AirportTool.Domain.Entities;

public class Flight
{
    public int FlightId { get; set; }
    public int AirlineId { get; set; }
    public string FlightNumber { get; set; } = null!;
    public int OriginAirportId { get; set; }
    public int DestinationAirportId { get; set; }
    public int? DefaultAircraftId { get; set; }
    public bool IsActive { get; set; } = true;

    public Airline Airline { get; set; } = null!;
    public Airport OriginAirport { get; set; } = null!;
    public Airport DestinationAirport { get; set; } = null!;
    public Aircraft? DefaultAircraft { get; set; }

    public ICollection<FlightSchedule> Schedules { get; set; } = new List<FlightSchedule>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
