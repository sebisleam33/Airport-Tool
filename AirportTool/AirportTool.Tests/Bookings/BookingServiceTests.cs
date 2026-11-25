using AirportTool.Application.Bookings;
using AirportTool.Application.Common.Interfaces;
using AirportTool.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace AirportTool.Tests.Bookings;

public class BookingServiceTests
{
    [Fact]
    public async Task CreateBooking_QuantityExceedsInventory_ThrowsInvalidOperationException()
    {
        var uowMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<ILogger<BookingService>>();

        var ticket = new Ticket
        {
            TicketId = 1,
            FlightId = 1,
            BasePrice = 100m,
            Taxes = 20m,
            TotalPrice = 120m,
            Currency = "EUR",
            FareClass = "Y",
            SeatInventory = 3
        };

        var schedule = new FlightSchedule
        {
            FlightScheduleId = 10,
            FlightId = 1,
            ScheduledDepartureUtc = DateTime.UtcNow.AddDays(1),
            ScheduledArrivalUtc = DateTime.UtcNow.AddDays(1).AddHours(2)
        };

        uowMock.Setup(u => u.Tickets.GetByIdAsync(1L, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        uowMock.Setup(u => u.FlightSchedules.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        var service = new BookingService(uowMock.Object, loggerMock.Object);

        var request = new CreateBookingRequestDto
        {
            FlightScheduleId = 10,
            TicketId = 1,
            PassengerFullName = "Test Passenger",
            PassengerEmail = "test@example.com",
            Quantity = 4
        };

        Func<Task> act = async () => await service.CreateBookingAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>();

        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
