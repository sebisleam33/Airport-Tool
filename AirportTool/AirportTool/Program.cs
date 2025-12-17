using AirportTool.Application.Bookings;
using AirportTool.Application.Common.Options;
using AirportTool.Application.Flights;
using AirportTool.Application.Schedules;
using AirportTool.Application.Tickets;
using AirportTool.Application.Common.Interfaces;
using AirportTool.Infrastructure.Persistence;
using AirportTool.Infrastructure.Repositories;
using AirportTool.Infrastructure.Services;
using AirportTool.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AirportDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IScheduleImportService, ScheduleImportService>();
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.Configure<PagingOptions>(builder.Configuration.GetSection("Paging"));
builder.Services.Configure<ImportOptions>(builder.Configuration.GetSection("Import"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
