using AirportTool.Application.Schedules;
using AirportTool.WebApi.Controllers.Models;
using Microsoft.AspNetCore.Mvc;

namespace AirportTool.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleImportService _importService;

    public SchedulesController(IScheduleImportService importService)
    {
        _importService = importService;
    }

    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Import(
        [FromForm] ScheduleImportRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest(Problem("File is required", statusCode: 400));

        if (request.File.Length > 2 * 1024 * 1024)
            return BadRequest(Problem("File too large", statusCode: 400));

        await using var stream = request.File.OpenReadStream();
        var result = await _importService.ImportAsync(stream, cancellationToken);

        if (result.Errors.Count > 0 && result.Created + result.Updated > 0)
            return StatusCode(StatusCodes.Status207MultiStatus, result);

        if (result.Errors.Count > 0)
            return BadRequest(result);

        return StatusCode(StatusCodes.Status201Created, result);
    }
}
