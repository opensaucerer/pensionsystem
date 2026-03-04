# Pension Contribution Management System

This is a .NET 8 Web API for managing pension contributions based on Clean Architecture and Domain-Driven Design (DDD). It features CQRS with MediatR, background job processing with Hangfire, and a robust SQL Server database.

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running.

## Getting Started

1. Clone the repository and navigate to the `PensionSystem` root directory (where `docker-compose.yml` is located).
2. Start the infrastructure and the API using Docker Compose:

   ```bash
   docker-compose up --build -d
   ```

3. The API and the Database will spin up together. The EF Core Migrations are automatically applied on application startup.
4. Access the API documentation (Swagger) at:
   - `http://localhost:8080/swagger`

5. Access the Hangfire Background Jobs Dashboard at:
   - `http://localhost:8080/hangfire`

## Architecture Overview

The system follows **Clean Architecture** principles, segregated into the following layers:

1. **Domain**: Contains entities, enums, value objects, and domain exceptions. Entities like `Member` and `Contribution` encapsulate business rules.
2. **Application**: Contains the business logic orchestrations via CQRS using **MediatR**. Request validation is handled via **FluentValidation**.
3. **Infrastructure**: Implements data access using **Entity Framework Core** and background job processing using **Hangfire**. `PensionDbContext` lives here.
4. **API**: The presentation layer, containing standard ASP.NET Core RESTful controllers and a Global Exception Handling Middleware that translates Domain/Validation exceptions into cleanly formatted 400 Bad Request HTTP responses.

## Key Technical Decisions

- **CQRS implementation without complex infrastructure**: MediatR simplifies command/query boundaries, improving separation of concerns.
- **Global Exception Middleware**: Removes `try/catch` clutter from the controllers and unifies API error response formats.
- **Dockerized SQL Server**: Ensures the assessment reviewer can test the API without needing local SQL Server installation. Data persistence is guaranteed through Docker volumes.

## Testing

The solution includes an xUnit testing project covering:
- Domain Entity behaviors (e.g., Age Calculation rules).
- Application Layer Validators ensuring business constraints.

To run tests (requires .NET 8 SDK locally):
```bash
dotnet test PensionSystem.Tests
```
