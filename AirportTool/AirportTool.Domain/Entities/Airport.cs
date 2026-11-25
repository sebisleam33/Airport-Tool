using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AirportTool.Domain.Entities;

public class Airport
{
    public int AirportId { get; set; }
    public string IATACode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? City { get; set; }
    public string? Country { get; set; }
    public string TimeZone { get; set; } = null!;

    public ICollection<Gate> Gates { get; set; } = new List<Gate>();
    public ICollection<Flight> OriginFlights { get; set; } = new List<Flight>();
    public ICollection<Flight> DestinationFlights { get; set; } = new List<Flight>();
}
