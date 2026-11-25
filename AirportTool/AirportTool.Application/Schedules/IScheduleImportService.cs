namespace AirportTool.Application.Schedules;

public interface IScheduleImportService
{
    Task<ScheduleImportResultDto> ImportAsync(Stream jsonStream, CancellationToken cancellationToken = default);
}
