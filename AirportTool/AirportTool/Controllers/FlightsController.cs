using AirportTool.Application.Common;
using AirportTool.Application.Flights;
using Microsoft.AspNetCore.Mvc;

namespace AirportTool.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightsController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<FlightScheduleDto>>> Get(
        [FromQuery] string? origin,
        [FromQuery] string? destination,
        [FromQuery] DateTime? date,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _flightService.SearchSchedulesAsync(
            origin, destination, date, page, pageSize, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create(
        [FromBody] CreateFlightDto dto,
        CancellationToken cancellationToken)
    {
        var id = await _flightService.CreateFlightAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FlightDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _flightService.GetByIdAsync(id, cancellationToken);
        if (dto is null) return NotFound();
        return Ok(dto);
    }
}
