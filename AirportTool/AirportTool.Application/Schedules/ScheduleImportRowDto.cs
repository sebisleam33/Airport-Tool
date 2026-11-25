namespace AirportTool.Application.Schedules;

public class ScheduleImportRowDto
{
    public string FlightNumber { get; set; } = null!;
    public string AirlineIata { get; set; } = null!;
    public string OriginIata { get; set; } = null!;
    public string DestinationIata { get; set; } = null!;
    public DateTime ScheduledDepartureUtc { get; set; }
    public DateTime ScheduledArrivalUtc { get; set; }
    public string? GateCode { get; set; }
    public string? AssignedAircraftTail { get; set; }
}

public class ScheduleImportResultDto
{
    public int Total { get; set; }
    public int Created { get; set; }
    public int Updated { get; set; }
    public List<ScheduleImportErrorDto> Errors { get; set; } = new();
}

public class ScheduleImportErrorDto
{
    public int Row { get; set; }
    public string Message { get; set; } = string.Empty;
}
