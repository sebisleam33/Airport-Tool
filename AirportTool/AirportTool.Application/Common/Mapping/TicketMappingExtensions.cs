using AirportTool.Application.Tickets;
using AirportTool.Domain.Entities;

namespace AirportTool.Application.Mapping;

public static class TicketMappingExtensions
{
    public static TicketDto ToTicketDto(this Ticket ticket)
    {
        return new TicketDto
        {
            TicketId = ticket.TicketId,
            FlightId = ticket.FlightId,
            FareClass = ticket.FareClass,
            BasePrice = ticket.BasePrice,
            Taxes = ticket.Taxes,
            TotalPrice = ticket.TotalPrice,
            Currency = ticket.Currency,
            IsRefundable = ticket.IsRefundable,
            SeatInventory = ticket.SeatInventory
        };
    }

    public static Ticket ToTicket(this CreateTicketDto dto)
    {
        var total = dto.BasePrice + dto.Taxes;

        return new Ticket
        {
            FlightId = dto.FlightId,
            FareClass = dto.FareClass,
            BasePrice = dto.BasePrice,
            Taxes = dto.Taxes,
            TotalPrice = total,
            Currency = dto.Currency,
            IsRefundable = dto.IsRefundable,
            SeatInventory = dto.SeatInventory
        };
    }

    public static void ApplyUpdate(this Ticket ticket, UpdateTicketDto dto)
    {
        ticket.BasePrice = dto.BasePrice;
        ticket.Taxes = dto.Taxes;
        ticket.TotalPrice = dto.BasePrice + dto.Taxes;
        ticket.Currency = dto.Currency;
        ticket.IsRefundable = dto.IsRefundable;
    }

    public static void ApplyInventoryUpdate(this Ticket ticket, UpdateTicketInventoryDto dto)
    {
        ticket.SeatInventory = dto.SeatInventory;
    }
}
