using AirportTool.Application.Common.Options;
using AirportTool.Application.Schedules;
using AirportTool.Domain.Entities;
using AirportTool.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AirportTool.Infrastructure.Services;

public class ScheduleImportService : IScheduleImportService
{
    private readonly AirportDbContext _context;
    private readonly ImportOptions _options;
    private readonly ILogger<ScheduleImportService> _logger;

    public ScheduleImportService(
        AirportDbContext context,
        IOptions<ImportOptions> options,
        ILogger<ScheduleImportService> logger)
    {
        _context = context;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ScheduleImportResultDto> ImportAsync(
        Stream jsonStream,
        CancellationToken cancellationToken = default)
    {
        var schedules = await JsonSerializer.DeserializeAsync<List<ScheduleImportRowDto>>(
            jsonStream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken)
            ?? new List<ScheduleImportRowDto>();

        if (schedules.Count > _options.MaxRows)
            throw new InvalidOperationException($"Max {_options.MaxRows} rows allowed.");

        var result = new ScheduleImportResultDto
        {
            Total = schedules.Count
        };

        var rowIndex = 1;
        foreach (var row in schedules)
        {
            try
            {
                await ProcessRowAsync(row, cancellationToken);
                result.Created++;
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ScheduleImportErrorDto
                {
                    Row = rowIndex,
                    Message = ex.Message
                });
            }

            rowIndex++;
        }

        _logger.LogInformation("Schedule import finished: {@Result}", result);
        return result;
    }

    private async Task ProcessRowAsync(ScheduleImportRowDto row, CancellationToken token)
    {

        if (row.ScheduledArrivalUtc <= row.ScheduledDepartureUtc)
            throw new InvalidOperationException("Arrival must be after departure.");

        if (!string.IsNullOrWhiteSpace(row.GateCode))
        {
            var gate = await _context.Gates
                .Include(g => g.Airport)
                .FirstOrDefaultAsync(g => g.Code == row.GateCode && g.Airport.IATACode == row.OriginIata, token)
                ?? throw new InvalidOperationException("Gate not found.");

            var overlaps = await _context.FlightSchedules
                .AnyAsync(fs =>
                    fs.GateId == gate.GateId &&
                    fs.ScheduledDepartureUtc < row.ScheduledArrivalUtc &&
                    fs.ScheduledArrivalUtc > row.ScheduledDepartureUtc,
                    token);

            if (overlaps)
                throw new InvalidOperationException($"Gate overlap at {row.GateCode}.");
        }
    }
}
