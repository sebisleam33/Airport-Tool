using AirportTool.Application.Tickets;
using Microsoft.AspNetCore.Mvc;

namespace AirportTool.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet("by-flight/{flightId:int}")]
    public async Task<ActionResult<IReadOnlyList<TicketDto>>> GetByFlight(
        int flightId,
        CancellationToken cancellationToken)
    {
        var tickets = await _ticketService.GetByFlightIdAsync(flightId, cancellationToken);
        return Ok(tickets);
    }

    [HttpPost]
    public async Task<ActionResult> Create(
        [FromBody] CreateTicketDto dto,
        CancellationToken cancellationToken)
    {
        var id = await _ticketService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetByFlight), new { flightId = dto.FlightId }, new { ticketId = id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdateTicketDto dto,
        CancellationToken cancellationToken)
    {
        await _ticketService.UpdateAsync(id, dto, cancellationToken);
        return Ok();
    }

    [HttpPut("{id:long}/inventory")]
    public async Task<IActionResult> UpdateInventory(
        long id,
        [FromBody] UpdateTicketInventoryDto dto,
        CancellationToken cancellationToken)
    {
        await _ticketService.UpdateInventoryAsync(id, dto, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken)
    {
        await _ticketService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
