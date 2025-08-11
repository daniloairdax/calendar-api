# Reviewer Instructions

Dear reviewer, here are some instructions to help you with the review process:

- API is deployed to [GitHub](https://github.com/daniloairdax/calendar-api)
- Main branch contains the initial API code.
- All changes are made via one feature branch and one PR:
  - Branch: https://github.com/daniloairdax/calendar-api/tree/feature/danilo/refactor
  - PR: https://github.com/daniloairdax/calendar-api/pull/1
  - PR summary contains a short description of changes.
- Main branch is protected:
  - No direct commits allowed.
  - All changes require PR and review.
- OWNERS file is added to the root.
- README.md provides a summary and database migration instructions ([link](https://github.com/daniloairdax/calendar-api/pull/1/files#diff-b335630551682c19a781afebcf4d07bf978fb1f8ac04c6bf87428ed5106870f5)).
- Startup class registers and configures all necessary services, called by Program.cs.
- Projects are divided by layers with clear responsibilities.
- MediatR is used for CQRS; FluentValidation for input validation.
- Exception handling via global filter `ApiExceptionFilterAttribute` for standardized error responses.
- `Api.http` contains all endpoints for API testing.
- SwaggerUI is used for API documentation and testing.
- Authorization via API key (hardcoded in `ApiKeyMiddleware`). In real life, use a secure store (e.g., Azure Key Vault).
- JWT is recommended for real authentication/authorization.
- To authenticate, pass API key in request header. For SwaggerUI, use key `danilotest`.
- V1 controllers/endpoints use hardcoded data for Animals/Appointments.
- V2 controllers use EF Core and SQLite database.
- V1 includes a HealthCheckController for API health status.
- V1 endpoints are marked obsolete and will be removed; they prompt users to use V2.
- V1 kept for backward compatibility; all fixes/bugs from V1 are applied to V2.
- All important classes/methods have XML documentation.
- Endpoints have full Swagger documentation (requests, responses, errors, status codes).
- V2 AppointmentsController adds new features: `GetVetAppointments`, `UpdateStatus`.
- Launch settings use Development environment by default.
- Multiple appsettings files for different environments (Development, Staging, Production).
- Each environment uses `calendar.db` in API project root.
- Startup class includes security features: CORS, API key authentication, SwaggerUI, Cookie policy, HSTS, HTTPS redirection, etc.
- V2 controllers use commands/queries from Calendar.Application for thin controllers.
- Application layer uses MediatR for CQRS and AutoMapper for model/DTO mapping.
- Queries/commands use FluentValidation for input validation.
- Handlers communicate via MediatR.
- FluentValidation does simple (non-DB) validations; DB access via EF Core in queries/commands over IRepository interfaces.
- Database access via Calendar.Infrastructure and Repository pattern.
- Database is code-first with migrations, using SQLite locally.
- On API launch, `calendar.db` is created in API root.
- Data is seeded automatically on startup via DbSeeder.
- Database schema/initial data generated from C# models/migrations.
- Services, CQRS handlers, and validations are tested in Calendar.Tests.
- Controllers are not tested (thin, only handle HTTP).
- For real projects, add integration/E2E tests with real DB (cloud/docker pipeline).
- Tests should cover repositories and controllers.
- xUnit and NSubstitute are used for testing.
- EmailService uses Debug.WriteLine instead of real email sending.
- All NuGet packages are up-to-date and suitable for .NET 8.
- Nightly build workflow runs at 2 AM on GitHub Actions (`.github/workflows/nightly-build.yml`).
  - Example run: https://github.com/daniloairdax/calendar-api/actions/runs/16867921274

## Issues found in initial API code
- Typo: calender instead of calendar API
- GetAnimal returns OK even if animal not found (null)
- CreateAnimal endpoint does not validate properties other than Name (e.g., OwnerName, OwnerEmail)
- CreateAppointment endpoint returns type Animal (should be Appointment)
- CreateAppointment endpoint does not validate StartTime/EndTime
- In AppointmentData, second Appointment missing StartTime/EndTime (not nullable)
- In AppointmentData, reading Id/OwnerId after First() may fail if list is empty
- Enum AppointmentStatus should be in separate file in Enums folder, not in Appointment.cs
- VeterinarianId in Appointment.cs has no reference; must be specified in AppointmentData (not nullable)
- WeatherForecast endpoint in Api.http does not exist; only api/animal and api/appointment
- Program.cs lacks authorization for production; consider enabling Swagger for non-dev environments
- Endpoints should be async
- Typo in AppointmentStatus enum: Cancelled should be Canceled
- In CreateAnimal, calling GetAnimal after add is not needed; should return created animal (fixed in V2)
- Same issue in CreateAppointment
- GetAnimal endpoint does not handle not found exception
- Not enough input validation in creation endpoints for Animals/Appointments
- CreateAppointment does not validate if AnimalId exists (fixed in V2)

Let me know if you have any questions or need more information.