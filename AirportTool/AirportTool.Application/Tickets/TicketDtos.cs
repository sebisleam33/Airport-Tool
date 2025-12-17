namespace AirportTool.Application.Tickets;

public class TicketDto
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
}

public class CreateTicketDto
{
    public int FlightId { get; set; }
    public string FareClass { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public decimal Taxes { get; set; }
    public string Currency { get; set; } = null!;
    public bool IsRefundable { get; set; }
    public int SeatInventory { get; set; }
}

public class UpdateTicketDto
{
    public decimal BasePrice { get; set; }
    public decimal Taxes { get; set; }
    public string Currency { get; set; } = null!;
    public bool IsRefundable { get; set; }
}

public class UpdateTicketInventoryDto
{
    public int SeatInventory { get; set; }
}
