using AirportTool.Application.Tickets;
using AirportTool.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AirportTool.Tests.Tickets;

public class TicketsControllerTests
{
    [Fact]
    public async Task Create_Returns201_AndCallsService()
    {
        var serviceMock = new Mock<ITicketService>();
        serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateTicketDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42L);

        var controller = new TicketsController(serviceMock.Object);

        var dto = new CreateTicketDto
        {
            FlightId = 1,
            FareClass = "Y",
            BasePrice = 100m,
            Taxes = 20m,
            Currency = "EUR",
            IsRefundable = true,
            SeatInventory = 50
        };

        var result = await controller.Create(dto, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(TicketsController.GetByFlight), created.ActionName);
        serviceMock.Verify(s => s.CreateAsync(It.IsAny<CreateTicketDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByFlight_ReturnsOk()
    {
        var serviceMock = new Mock<ITicketService>();
        serviceMock
            .Setup(s => s.GetByFlightIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TicketDto>());

        var controller = new TicketsController(serviceMock.Object);

        var result = await controller.GetByFlight(1, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<IReadOnlyList<TicketDto>>(okResult.Value);
    }
}
