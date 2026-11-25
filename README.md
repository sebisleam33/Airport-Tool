# AirportTool

AirportTool is a .NET 8 Web API for airport operations. It provides endpoints for flights and flight schedules, ticket inventory, passenger bookings, and importing schedule data from a JSON file. The solution uses ASP.NET Core, Entity Framework Core, and SQL Server.

## Features

- Create and look up flights
- Search flight schedules by origin, destination, and date with paging
- Create, retrieve, update, and cancel bookings
- Manage ticket details and seat inventory
- Upload JSON schedule files for processing
- Centralized API exception handling
- Swagger UI for interactive API exploration in Development

## Solution structure

| Project | Responsibility |
| --- | --- |
| `AirportTool` | ASP.NET Core API, controllers, middleware, Swagger, and application startup |
| `AirportTool.Application` | Use cases, services, DTOs, options, and application interfaces |
| `AirportTool.Domain` | Airport, airline, aircraft, flight, schedule, ticket, booking, and gate entities |
| `AirportTool.Infrastructure` | EF Core database context, entity configuration, repositories, unit of work, and schedule import service |
| `AirportTool.Tests` | xUnit tests for services, controllers, and schedule import endpoints |

## Requirements

- .NET 8 SDK
- SQL Server (local SQL Server, LocalDB, or another reachable SQL Server instance)

## Database setup

The default connection string is in `AirportTool/appsettings.json`:

```text
Server=localhost;Database=AirportTool;Trusted_Connection=True;TrustServerCertificate=True;
```

Update `ConnectionStrings:DefaultConnection` to match your SQL Server instance. You can override it with the environment variable `ConnectionStrings__DefaultConnection`, for example:

```text
ConnectionStrings__DefaultConnection="Server=localhost;Database=AirportTool;Trusted_Connection=True;TrustServerCertificate=True;"
```

The EF Core model seeds a small set of reference records when the database is created: TAROM, Bucharest Otopeni (OTP), London Heathrow (LHR), gate A12 at OTP, and aircraft YR-BGA. The repository does not include EF Core migration files or create/update the database at API startup. Create the database schema before running the API, or add and apply migrations for your environment.

## Build and run

From the directory containing `AirportTool.sln`:

```bash
dotnet restore AirportTool.sln
dotnet build AirportTool.sln
dotnet run --project AirportTool/AirportTool.WebApi.csproj
```

The included launch profiles run the API at `http://localhost:5130` or `https://localhost:7209`. With the `Development` environment enabled, Swagger UI is available at:

```text
http://localhost:5130/swagger
```

or

```text
https://localhost:7209/swagger
```

## API endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/flights` | Search schedules; accepts optional `origin`, `destination`, `date`, `page`, and `pageSize` query parameters |
| `GET` | `/api/flights/{id}` | Get flight details |
| `POST` | `/api/flights` | Create a flight |
| `POST` | `/api/bookings` | Create a booking |
| `GET` | `/api/bookings/{code}` | Retrieve a booking by confirmation code |
| `DELETE` | `/api/bookings/{code}` | Cancel a booking |
| `GET` | `/api/tickets/by-flight/{flightId}` | List tickets for a flight |
| `POST` | `/api/tickets` | Create a ticket |
| `PUT` | `/api/tickets/{id}` | Update ticket price and fare details |
| `PUT` | `/api/tickets/{id}/inventory` | Update ticket seat inventory |
| `DELETE` | `/api/tickets/{id}` | Delete a ticket |
| `POST` | `/api/schedules/import` | Upload a schedule JSON file as multipart form data; form field name is `file` |

See Swagger for request and response schemas. The schedule import endpoint rejects empty files and files larger than 2 MB. The configured import limit is 1,000 rows.

### Example: search schedules

```http
GET /api/flights?origin=OTP&destination=LHR&page=1&pageSize=20
Accept: application/json
```

### Example: create a booking

```http
POST /api/bookings
Content-Type: application/json
```

```json
{
  "flightScheduleId": 1,
  "ticketId": 1,
  "passengerFullName": "Alex Example",
  "passengerEmail": "alex@example.com",
  "quantity": 1
}
```

### Example schedule import JSON

Upload a JSON array in the `file` form field. Each entry uses the following shape:

```json
[
  {
    "flightNumber": "RO101",
    "airlineIata": "RO",
    "originIata": "OTP",
    "destinationIata": "LHR",
    "scheduledDepartureUtc": "2026-12-01T08:00:00Z",
    "scheduledArrivalUtc": "2026-12-01T10:30:00Z",
    "gateCode": "A12",
    "assignedAircraftTail": "YR-BGA"
  }
]
```

Import timestamps should be supplied in UTC. The current implementation checks the arrival/departure order and, when a gate is supplied, verifies that the gate exists and does not overlap another schedule. Review and complete persistence behavior in `ScheduleImportService` before relying on the endpoint to create or update schedule records: the current implementation does not save the imported rows.

## Run tests

From the solution directory:

```bash
dotnet test AirportTool.sln
```

The test project uses xUnit, Moq, and FluentAssertions.

## Configuration

- `ConnectionStrings:DefaultConnection`: SQL Server connection string
- `Paging:DefaultPage`: default schedule page (1)
- `Paging:DefaultPageSize`: default page size (20)
- `Paging:MaxPageSize`: maximum page size (100)
- `Import:MaxRows`: maximum number of rows processed per import (1,000)

No authentication is configured in the current API startup. Configure authentication and authorization before exposing operational or passenger data outside a trusted development environment.
