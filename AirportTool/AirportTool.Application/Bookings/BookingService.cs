using AirportTool.Application.Common.Interfaces;
using AirportTool.Domain.Entities;
using AirportTool.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AirportTool.Application.Bookings;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IUnitOfWork uow, ILogger<BookingService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<BookingResponseDto> CreateBookingAsync(
        CreateBookingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _uow.Tickets.GetByIdAsync(request.TicketId, cancellationToken)
                     ?? throw new KeyNotFoundException("Ticket not found");

        if (request.Quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.Quantity));

        if (ticket.BasePrice < 0 || ticket.Taxes < 0 ||
            ticket.TotalPrice != ticket.BasePrice + ticket.Taxes)
            throw new InvalidOperationException("Invalid ticket price");

        if (ticket.SeatInventory < request.Quantity)
            throw new InvalidOperationException("Overbooking not allowed");

        var schedule = await _uow.FlightSchedules.GetByIdAsync(request.FlightScheduleId, cancellationToken)
                       ?? throw new KeyNotFoundException("Schedule not found");

        ticket.SeatInventory -= request.Quantity;
        _uow.Tickets.Update(ticket);

        var booking = new Booking
        {
            FlightScheduleId = schedule.FlightScheduleId,
            TicketId = ticket.TicketId,
            PassengerFullName = request.PassengerFullName,
            PassengerEmail = request.PassengerEmail,
            Quantity = request.Quantity,
            ConfirmationCode = GenerateConfirmationCode(),
            Status = BookingStatus.Active,
            CreatedUtc = DateTime.UtcNow
        };

        await _uow.Bookings.AddAsync(booking, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Booking created {Code}", booking.ConfirmationCode);

        return new BookingResponseDto
        {
            ConfirmationCode = booking.ConfirmationCode,
            Status = booking.Status.ToString(),
            TotalAmount = booking.Quantity * ticket.TotalPrice
        };
    }

    public async Task CancelBookingAsync(string confirmationCode, CancellationToken cancellationToken = default)
    {
        var booking = await GetBookingEntityByCodeAsync(confirmationCode, cancellationToken)
                      ?? throw new KeyNotFoundException("Booking not found");

        if (booking.Status == BookingStatus.Cancelled)
            return;

        booking.Status = BookingStatus.Cancelled;
        var ticket = booking.Ticket;
        ticket.SeatInventory += booking.Quantity;

        _uow.Bookings.Update(booking);
        _uow.Tickets.Update(ticket);

        await _uow.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Booking cancelled {Code}", confirmationCode);
    }

    public async Task<BookingResponseDto?> GetByCodeAsync(
        string confirmationCode,
        CancellationToken cancellationToken = default)
    {
        var booking = await GetBookingEntityByCodeAsync(confirmationCode, cancellationToken);
        if (booking is null) return null;

        return new BookingResponseDto
        {
            ConfirmationCode = booking.ConfirmationCode,
            Status = booking.Status.ToString(),
            TotalAmount = booking.Quantity * booking.Ticket.TotalPrice
        };
    }

    private async Task<Booking?> GetBookingEntityByCodeAsync(
        string confirmationCode,
        CancellationToken token)
    {
        if (_uow is not IBookingQueryAccessor accessor)
            throw new InvalidOperationException("Booking query accessor not configured.");

        return await accessor.GetBookingWithTicketByCodeAsync(confirmationCode, token);
    }

    private static string GenerateConfirmationCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = Random.Shared;
        return new string(Enumerable.Range(0, 6)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }
}
