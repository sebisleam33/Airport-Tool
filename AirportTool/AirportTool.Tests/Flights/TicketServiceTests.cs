using AirportTool.Application.Common.Interfaces;
using AirportTool.Application.Tickets;
using AirportTool.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirportTool.Tests.Tickets;

public class TicketServiceTests
{
    [Fact]
    public async Task CreateAsync_NegativeBasePrice_ThrowsInvalidOperationException()
    {
        var uowMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<ILogger<TicketService>>();

        var service = new TicketService(uowMock.Object, loggerMock.Object);

        var dto = new CreateTicketDto
        {
            FlightId = 1,
            FareClass = "Y",
            BasePrice = -10m,
            Taxes = 5m,
            Currency = "EUR",
            IsRefundable = false,
            SeatInventory = 10
        };

        Func<Task> act = async () => await service.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_TicketHasBookings_ThrowsInvalidOperationException()
    {
        var uowMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<ILogger<TicketService>>();
        var accessorMock = uowMock.As<ITicketQueryAccessor>();

        accessorMock.Setup(a => a.TicketHasBookingsAsync(1L, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new TicketService(uowMock.Object, loggerMock.Object);

        Func<Task> act = async () => await service.DeleteAsync(1L);

        await act.Should().ThrowAsync<InvalidOperationException>();
        uowMock.Verify(u => u.Tickets.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateInventoryAsync_NegativeInventory_ThrowsInvalidOperationException()
    {
        var uowMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<ILogger<TicketService>>();

        var service = new TicketService(uowMock.Object, loggerMock.Object);

        var dto = new UpdateTicketInventoryDto { SeatInventory = -1 };

        Func<Task> act = async () => await service.UpdateInventoryAsync(1L, dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
        uowMock.Verify(u => u.Tickets.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
