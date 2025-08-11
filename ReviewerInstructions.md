Dear reviewer, here are some instructions to help you with the review process:

API is deployed to https://github.com/daniloairdax/calendar-api
On main branch you can find init API code that you provided.
I did all my changes via one feature branch and one PR.
Branch: https://github.com/daniloairdax/calendar-api/tree/feature/danilo/refactor
PR link: https://github.com/daniloairdax/calendar-api/pull/1
In PR summary you may find short description of changes I made as well.

I added some rules as protection to the main branch, to avoid direct commits to it and to ensure that all changes are made through PRs and reviewed by at least one person.
I added also OWNERS file to the root.
Also you may find README.md file with basic summary of the solution and how to maintain in future database migrations. 
Readme link: https://github.com/daniloairdax/calendar-api/pull/1/files#diff-b335630551682c19a781afebcf4d07bf978fb1f8ac04c6bf87428ed5106870f5

I crated startup class with all necessary services registrations and configurations that is called by Program.cs file.
All projects are divided by layers and have their own responsibilities.
I used MediatR for CQRS and FluentValidation for input validation.
I added exception handling via global filter ApiExceptionFilterAttribute. It intercepts all unhandled exceptions and returns a standardized error response.
Api.http is updated and contains all necessary endpoints to test the API.
Also solution uses SwaggerUI for API documentation and testing.
I added authorization via API key which is now just hardcoded in the ApiKeyMiddleware. In real life it should be stored in a secure place like Azure Key Vault or AWS Secrets Manager.
Also in real life I would use JWT for authentication and authorization.
So in order to be authenticated you need to pass the API key in the request header.
If you are testing over SwaggerUI after launching the API project Calendar.Api, you need to authorize using a key "danilotest".

All controllers and endpoints that you provided are in V1 version of the API now. They are still using hardcoded data for Animals and Appointments.
I created a new controller V2 with endpoints that use EF Core and SQLite database.
In V1 I added a new HealthCheckController that returns the health status of the API.
V1 endpoints are marked as opsolete and will be removed in the future. They are providing  message to force user to use V2 endpoints.
I didnt want to break the existing functionality and wanted to keep the old endpoints for backward compatibility.
All fixes and bugs from V1 are applied to V2.
All important classes and methods are documented with XML comments.
Endpoints provided full Swagger documentation with expected request and response models including errors and status codes.
New features are added in V2 appointments controller in endpoints GetVetAppointments and UpdateStatus.
Launch settings is using Developement environment by default.
There are different appsettings files for different environments: Development, Staging, Production.
For now each of them is using calendar.db file in the root of the API project.
Startup class contains many security features like CORS, API key authentication, SwaggerUI, Cookie policy, HSTS, Https redirection, etc.
It helps with protection from common vulnerabilities and attacks.
V2 controllers are using now commands and queries from Calendar.Application project. This keeps controllers thin and focused on handling HTTP requests and responses.
In application layer I used MediatR for CQRS and AutoMapper for mapping between domain models and DTOs.
Queries and commands are validating input data using FluentValidation.
They also can communicate between each other via MediatR.
Fluent validation does simple validations which doesnt require database access.
If database access is required, then it is done via EF Core in the queries and commands over IRepository interfaces.
Database access is done via Calendar.Infrastructure project and Repository pattern.
Database is craated using code-first approach with migrations with SQL lite as the local database.
After launching the API project Calendar.Api, the database file calendar.db is created in the root of the API project.
Data it self is seeded automatically on app startup via DbSeeder class.
Database schema and initial data are generated from C# models and migrations.
Services and CQRS handlers and fluent validations are tested in Calendar.Tests project.
Controllers are not tested, because they are thin and focused on handling HTTP requests and responses.
In real life I would create Integration and E2E tests with real database access when db is hosted in a cloud environment or in docker container on pipeline.
Those tests would cover Repositories and Conntrolers as well.
I used xUnit and NSubstitute for testing.
EmailService use Debug.WriteLine instead of real email sending. 
All nugets that are used are without vulnerabilities and are up to date to versions that suits .NET 8.

I created also simple Nightly build workfklow that runs every night at 2 AM on github actions and its located int .github/workflows/nightly-build.yml.
It does simple build and test of the solution.
Example of one run:
https://github.com/daniloairdax/calendar-api/actions/runs/16867921274

Let me know if you have any questions or need more information.