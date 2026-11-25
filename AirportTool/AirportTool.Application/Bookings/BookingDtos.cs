namespace AirportTool.Application.Bookings;

public class CreateBookingRequestDto
{
    public int FlightScheduleId { get; set; }
    public long TicketId { get; set; }
    public string PassengerFullName { get; set; } = null!;
    public string PassengerEmail { get; set; } = null!;
    public int Quantity { get; set; }
}

public class BookingResponseDto
{
    public string ConfirmationCode { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
}

public class BookingDetailsDto
{
    public string ConfirmationCode { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string PassengerFullName { get; set; } = null!;
    public string PassengerEmail { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public int FlightScheduleId { get; set; }
    public long TicketId { get; set; }
    public DateTime CreatedUtc { get; set; }
}
