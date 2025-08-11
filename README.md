# Calendar API Solution

This solution is an appointment system for a veterinary clinic, built with ASP.NET Core (.NET 8) and Entity Framework Core using a code-first approach with SQLite as the local database.

## Projects
- **Calendar.Api**: ASP.NET Core Web API for managing appointments, animals, and health checks.
- **Calendar.Application**: Application layer containing business logic, CQRS handlers, and interfaces.
- **Calendar.Infrastructure**: Data access, persistence, and service implementations (e.g., email, repositories).
- **Calendar.Domain**: Domain models and enums (excluded from code coverage).
- **Calendar.Tests**: Unit tests for application logic (excluded from code coverage).

## Technologies
- .NET 8
- ASP.NET Core
- Entity Framework Core (SQLite)
- MediatR (CQRS)
- AutoMapper
- FluentValidation
- xUnit, NSubstitute (testing)
- Swagger/OpenAPI

## Database
- **SQLite** is used as the local database.
- The database file (`calendar.db`) is created in the API project root when the app is run.
- **Code-first**: Database schema and initial data are generated from C# models and migrations.
- **Data seeding**: Initial sample data for Animals and Appointments is automatically inserted on app startup.

## How to Run
1. **Restore NuGet packages**
2. **Run the API project** (`Calendar.Api`) to create and seed the database
3. **View the database**:
   - Use a SQLite GUI tool (recommended: [DB Browser for SQLite](https://sqlitebrowser.org/))
   - Or use the [SQLite/SQL Server Compact Toolbox](https://marketplace.visualstudio.com/items?itemName=ErikEJ.SQLServerCompactSQLiteToolbox) extension in Visual Studio:
     - Go to `View > Other Windows > SQLite/SQL Server Compact Toolbox`
     - Add and open `calendar.db` to browse tables and data
   - Or use the SQLite CLI (`sqlite3 calendar.db`)

## EF Core Migrations
- To update the database schema, use the following commands:
  ```sh
  dotnet ef migrations add <MigrationName> --project Calendar.Infrastructure --startup-project Calendar.Api
  dotnet ef database update --project Calendar.Infrastructure --startup-project Calendar.Api
  ```
- Make sure the EF Core CLI tool is installed:
  ```sh
  dotnet tool install --global dotnet-ef
  ```

## Requirements
- .NET 8 SDK
- Visual Studio 2022+ (or VS Code)
- [DB Browser for SQLite](https://sqlitebrowser.org/) or Visual Studio extension for SQLite
- EF Core CLI (`dotnet-ef`)

## Notes
- The solution uses MediatR for CQRS and FluentValidation for input validation.
- Exception handling is centralized via a global filter.
- All data access is via EF Core and the code-first approach.

## Improvements & Scalability
- Move seeding logic to migrations for production
- Add authentication/authorization for secure endpoints
- Use repository pattern for more complex data access
- Add more tests and API documentation

## API Documentation
- Swagger UI is available at `/swagger` when running the API.

---

**For reviewers:**
- Run the API to generate and seed the database
- Use the recommended tools to inspect the database and verify seeded data
- All code and configuration is in the repository; no external dependencies required