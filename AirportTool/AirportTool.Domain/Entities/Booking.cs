using AirportTool.Domain.Enums;

namespace AirportTool.Domain.Entities;

public class Booking
{
    public long BookingId { get; set; }
    public int FlightScheduleId { get; set; }
    public long TicketId { get; set; }
    public string PassengerFullName { get; set; } = null!;
    public string PassengerEmail { get; set; } = null!;
    public string ConfirmationCode { get; set; } = null!;
    public int Quantity { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Active;
    public DateTime CreatedUtc { get; set; }

    public FlightSchedule FlightSchedule { get; set; } = null!;
    public Ticket Ticket { get; set; } = null!;
}
