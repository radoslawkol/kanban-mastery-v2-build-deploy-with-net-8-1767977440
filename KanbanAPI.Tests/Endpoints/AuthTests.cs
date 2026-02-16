using System.Net;
using System.Net.Http.Json;
using KanbanAPI.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KanbanAPI.Tests.Endpoints
{
    public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public AuthTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOk()
        {
            // Arrange
            var registrationData = new
            {
                email = "test@example.com",
                password = "Test123!@#"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/register", registrationData);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
		public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
		{
			// Arrange
			var email = "duplicate@example.com";
			var password = "Test123!@#";

			var registrationData = new { email, password };

			await _client.PostAsJsonAsync("/register", registrationData);

			// Act
			var response = await _client.PostAsJsonAsync("/register", registrationData);

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
    }
}
