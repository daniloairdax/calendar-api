using Calendar.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.IntegrationTests.Controllers.V2
{
    [ExcludeFromCodeCoverage]
    public class AppointmentControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public AppointmentControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateAppointment_ReturnsOkOrBadRequest()
        {
            // Arrange
            var appointment = new
            {
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
                AnimalId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                VeterinarianId = Guid.NewGuid(),
                Status = 0,
                Notes = "Integration test appointment"
            };
            var content = new StringContent(JsonSerializer.Serialize(appointment), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v2/appointment", content);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAppointment_ReturnsOkOrNotFound()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var requestUri = $"/api/v2/appointment/{appointmentId}";

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetVetAppointments_ReturnsOkOrNotFound()
        {
            // Arrange
            var vetId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(-1).ToString("o");
            var endDate = DateTime.UtcNow.AddDays(1).ToString("o");
            var requestUri = $"/api/v2/appointment/vet/{vetId}?startDate={startDate}&endDate={endDate}";

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsOkOrBadRequest()
        {
            // Arrange
            var updateStatus = new
            {
                AppointmentId = Guid.NewGuid(),
                Status = 1 // e.g. Canceled
            };
            var content = new StringContent(JsonSerializer.Serialize(updateStatus), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync("/api/v2/appointment/status", content);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest);
        }
    }
}
