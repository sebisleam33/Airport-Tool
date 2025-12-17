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

    /// <summary>
    /// Get tickets for a given flight (availability & prices).
    /// </summary>
    [HttpGet("by-flight/{flightId:int}")]
    public async Task<ActionResult<IReadOnlyList<TicketDto>>> GetByFlight(
        int flightId,
        CancellationToken cancellationToken)
    {
        var tickets = await _ticketService.GetByFlightIdAsync(flightId, cancellationToken);
        return Ok(tickets);
    }

    /// <summary>
    /// Create a ticket (fare product) for a flight.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> Create(
        [FromBody] CreateTicketDto dto,
        CancellationToken cancellationToken)
    {
        var id = await _ticketService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetByFlight), new { flightId = dto.FlightId }, new { ticketId = id });
    }

    /// <summary>
    /// Update ticket price & basic data (not inventory).
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdateTicketDto dto,
        CancellationToken cancellationToken)
    {
        await _ticketService.UpdateAsync(id, dto, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Set seat inventory for a ticket.
    /// </summary>
    [HttpPut("{id:long}/inventory")]
    public async Task<IActionResult> UpdateInventory(
        long id,
        [FromBody] UpdateTicketInventoryDto dto,
        CancellationToken cancellationToken)
    {
        await _ticketService.UpdateInventoryAsync(id, dto, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Delete ticket if no bookings exist.
    /// </summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken)
    {
        await _ticketService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
