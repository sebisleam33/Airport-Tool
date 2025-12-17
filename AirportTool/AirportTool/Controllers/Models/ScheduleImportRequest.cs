using Microsoft.AspNetCore.Http;

namespace AirportTool.WebApi.Controllers.Models;

public class ScheduleImportRequest
{
    public IFormFile? File { get; set; }
}
