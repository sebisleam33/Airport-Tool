using AirportTool.Application.Common.Interfaces;
using AirportTool.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AirportTool.Application.Tickets;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<TicketService> _logger;

    public TicketService(IUnitOfWork uow, ILogger<TicketService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TicketDto>> GetByFlightIdAsync(
        int flightId,
        CancellationToken cancellationToken = default)
    {
        if (_uow is not ITicketQueryAccessor accessor)
            throw new InvalidOperationException("Ticket query accessor not configured.");

        var tickets = await accessor.GetTicketsByFlightIdAsync(flightId, cancellationToken);

        return tickets.Select(MapToDto).ToList();
    }

    public async Task<long> CreateAsync(
        CreateTicketDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidatePrice(dto.BasePrice, dto.Taxes, dto.BasePrice + dto.Taxes);
        if (dto.SeatInventory < 0)
            throw new InvalidOperationException("Seat inventory cannot be negative.");

        var ticket = new Ticket
        {
            FlightId = dto.FlightId,
            FareClass = dto.FareClass,
            BasePrice = dto.BasePrice,
            Taxes = dto.Taxes,
            TotalPrice = dto.BasePrice + dto.Taxes,
            Currency = dto.Currency,
            IsRefundable = dto.IsRefundable,
            SeatInventory = dto.SeatInventory
        };

        await _uow.Tickets.AddAsync(ticket, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket created {TicketId} for flight {FlightId}",
            ticket.TicketId, ticket.FlightId);

        return ticket.TicketId;
    }

    public async Task UpdateAsync(
        long ticketId,
        UpdateTicketDto dto,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _uow.Tickets.GetByIdAsync(ticketId, cancellationToken)
                     ?? throw new KeyNotFoundException("Ticket not found.");

        var newTotal = dto.BasePrice + dto.Taxes;
        ValidatePrice(dto.BasePrice, dto.Taxes, newTotal);

        ticket.BasePrice = dto.BasePrice;
        ticket.Taxes = dto.Taxes;
        ticket.TotalPrice = newTotal;
        ticket.Currency = dto.Currency;
        ticket.IsRefundable = dto.IsRefundable;

        _uow.Tickets.Update(ticket);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket {TicketId} updated", ticketId);
    }

    public async Task UpdateInventoryAsync(
        long ticketId,
        UpdateTicketInventoryDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.SeatInventory < 0)
            throw new InvalidOperationException("Seat inventory cannot be negative.");

        var ticket = await _uow.Tickets.GetByIdAsync(ticketId, cancellationToken)
                     ?? throw new KeyNotFoundException("Ticket not found.");

        ticket.SeatInventory = dto.SeatInventory;

        _uow.Tickets.Update(ticket);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket {TicketId} inventory set to {Inventory}",
            ticketId, dto.SeatInventory);
    }

    public async Task DeleteAsync(
        long ticketId,
        CancellationToken cancellationToken = default)
    {
        if (_uow is not ITicketQueryAccessor accessor)
            throw new InvalidOperationException("Ticket query accessor not configured.");

        var hasBookings = await accessor.TicketHasBookingsAsync(ticketId, cancellationToken);
        if (hasBookings)
            throw new InvalidOperationException("Cannot delete ticket with existing bookings.");

        var ticket = await _uow.Tickets.GetByIdAsync(ticketId, cancellationToken)
                     ?? throw new KeyNotFoundException("Ticket not found.");

        _uow.Tickets.Remove(ticket);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket {TicketId} deleted", ticketId);
    }

    private static void ValidatePrice(decimal basePrice, decimal taxes, decimal total)
    {
        if (basePrice < 0 || taxes < 0)
            throw new InvalidOperationException("Prices and taxes must be non-negative.");

        if (total != basePrice + taxes)
            throw new InvalidOperationException("TotalPrice must equal BasePrice + Taxes.");
    }

    private static TicketDto MapToDto(Ticket ticket) => new()
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
