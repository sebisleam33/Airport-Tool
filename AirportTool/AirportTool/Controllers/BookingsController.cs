using AirportTool.Application.Bookings;
using Microsoft.AspNetCore.Mvc;

namespace AirportTool.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>> Create(
        [FromBody] CreateBookingRequestDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _bookingService.CreateBookingAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetByCode), new { code = result.ConfirmationCode }, result);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<BookingResponseDto>> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingService.GetByCodeAsync(code, cancellationToken);
        if (booking is null) return NotFound();
        return Ok(booking);
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult> Cancel(string code, CancellationToken cancellationToken)
    {
        await _bookingService.CancelBookingAsync(code, cancellationToken);
        return NoContent();
    }
}
