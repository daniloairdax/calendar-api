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
    public class AnimalControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public AnimalControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateAnimal_ReturnsOkOrBadRequest()
        {
            // Arrange
            var animal = new
            {
                Name = "Test Animal",
                Species = "Dog",
                Breed = "Test Breed",
                OwnerName = "Test Owner",
                OwnerEmail = "owner@example.com"
            };
            var content = new StringContent(JsonSerializer.Serialize(animal), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v2/animal", content);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAnimal_ReturnsOkOrNotFound()
        {
            // Arrange
            var animalId = Guid.NewGuid();
            var requestUri = $"/api/v2/animal/{animalId}";

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteAnimal_ReturnsOkOrNotFound()
        {
            // Arrange
            var animalId = Guid.NewGuid();
            var requestUri = $"/api/v2/animal/{animalId}";

            // Act
            var response = await _client.DeleteAsync(requestUri);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }
    }
}