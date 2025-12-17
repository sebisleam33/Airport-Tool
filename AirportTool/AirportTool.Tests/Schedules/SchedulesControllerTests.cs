using AirportTool.Application.Schedules;
using AirportTool.WebApi.Controllers;
using AirportTool.WebApi.Controllers.Models;  
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AirportTool.Tests.Schedules;

public class SchedulesControllerTests
{
    [Fact]
    public async Task Import_MixedResults_Returns207()
    {
        var serviceMock = new Mock<IScheduleImportService>();
        var result = new ScheduleImportResultDto
        {
            Total = 2,
            Created = 1,
            Updated = 0,
            Errors = new()
            {
                new ScheduleImportErrorDto { Row = 2, Message = "Gate overlap" }
            }
        };

        serviceMock
            .Setup(s => s.ImportAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = new SchedulesController(serviceMock.Object);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        var request = new ScheduleImportRequest
        {
            File = fileMock.Object
        };

        var response = await controller.Import(request, CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(response);
        Assert.Equal(StatusCodes.Status207MultiStatus, objectResult.StatusCode);
    }
}
