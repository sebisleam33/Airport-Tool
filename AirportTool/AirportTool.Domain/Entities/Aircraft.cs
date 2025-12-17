namespace AirportTool.Domain.Entities;

public class Aircraft
{
    public int AircraftId { get; set; }
    public string TailNumber { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int SeatCapacity { get; set; }
    public int? OwnedByAirlineId { get; set; }

    public Airline? OwnedByAirline { get; set; }
    public ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
