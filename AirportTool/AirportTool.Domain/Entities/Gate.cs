namespace AirportTool.Domain.Entities;

public class Gate
{
    public int GateId { get; set; }
    public int AirportId { get; set; }
    public string Code { get; set; } = null!;

    public Airport Airport { get; set; } = null!;
    public ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();
}
