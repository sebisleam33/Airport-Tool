using AirportTool.Application.Bookings;
using AirportTool.Domain.Entities;
using AirportTool.Domain.Enums;

namespace AirportTool.Application.Mapping;

public static class BookingMappingExtensions
{
    public static Booking ToBooking(
        this CreateBookingRequestDto dto,
        Ticket ticket,
        FlightSchedule schedule,
        string confirmationCode)
    {
        return new Booking
        {
            FlightScheduleId = dto.FlightScheduleId,
            TicketId = dto.TicketId,
            PassengerFullName = dto.PassengerFullName,
            PassengerEmail = dto.PassengerEmail,
            ConfirmationCode = confirmationCode,
            Quantity = dto.Quantity,
            Status = BookingStatus.Active,
            CreatedUtc = DateTime.UtcNow
        };
    }

    public static BookingResponseDto ToBookingResponseDto(
        this Booking booking,
        Ticket ticket)
    {
        return new BookingResponseDto
        {
            ConfirmationCode = booking.ConfirmationCode,
            Status = booking.Status.ToString(),
            TotalAmount = booking.Quantity * ticket.TotalPrice
        };
    }

    public static BookingDetailsDto ToBookingDetailsDto(
        this Booking booking,
        Ticket ticket,
        FlightSchedule schedule)
    {
        return new BookingDetailsDto
        {
            ConfirmationCode = booking.ConfirmationCode,
            Status = booking.Status.ToString(),
            PassengerFullName = booking.PassengerFullName,
            PassengerEmail = booking.PassengerEmail,
            Quantity = booking.Quantity,
            TotalAmount = booking.Quantity * ticket.TotalPrice,
            FlightScheduleId = schedule.FlightScheduleId,
            TicketId = ticket.TicketId,
            CreatedUtc = booking.CreatedUtc
        };
    }
}
