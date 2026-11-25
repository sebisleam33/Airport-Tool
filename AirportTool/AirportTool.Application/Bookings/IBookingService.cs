namespace AirportTool.Application.Bookings;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingAsync(
        CreateBookingRequestDto request,
        CancellationToken cancellationToken = default);

    Task CancelBookingAsync(
        string confirmationCode,
        CancellationToken cancellationToken = default);

    Task<BookingResponseDto?> GetByCodeAsync(
        string confirmationCode,
        CancellationToken cancellationToken = default);
}
