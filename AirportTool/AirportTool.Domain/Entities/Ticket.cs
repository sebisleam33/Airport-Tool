namespace AirportTool.Domain.Entities;

public class Ticket
{
    public long TicketId { get; set; }
    public int FlightId { get; set; }
    public string FareClass { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public decimal Taxes { get; set; }
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = null!;
    public bool IsRefundable { get; set; }
    public int SeatInventory { get; set; }

    public Flight Flight { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
