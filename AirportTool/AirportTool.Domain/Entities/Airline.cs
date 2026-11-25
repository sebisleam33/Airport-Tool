namespace AirportTool.Domain.Entities;

public class Airline
{
    public int AirlineId { get; set; }
    public string IATACode { get; set; } = null!;
    public string Name { get; set; } = null!;

    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
