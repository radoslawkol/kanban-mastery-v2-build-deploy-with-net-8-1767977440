using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
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

		[Fact]
		public async Task Login_WithValidCredentials_ReturnsOkWithToken()
		{
			// Arrange
			var email = "login@example.com";
			var password = "LoginTest123!@#";

			var registrationData = new { email, password };
			var registerRes = await _client.PostAsJsonAsync("/register", registrationData);

			var loginData = new { email, password };

			// Act
			var response = await _client.PostAsJsonAsync("/login", loginData);

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
			Assert.NotNull(content);
			Assert.NotNull(content.AccessToken);
		}

		[Fact]
		public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
		{
			// Arrange
			var email = "valid@example.com";
			var password = "ValidPassword123!@#";

			var registrationData = new { email, password };
			await _client.PostAsJsonAsync("/register", registrationData);

			var loginData = new { email, password = "WrongPassword123!@#" };

			// Act
			var response = await _client.PostAsJsonAsync("/login", loginData);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task GetUserProfile_WithValidToken_ReturnsOkWithUserData()
		{
			// Arrange
			var email = "profile@example.com";
			var password = "ProfileTest123!@#";

			var registrationData = new { email, password };
			var registerResponse = await _client.PostAsJsonAsync("/register", registrationData);

			var loginData = new { email, password };
			var loginResponse = await _client.PostAsJsonAsync("/login", loginData);

			var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

			Assert.NotNull(loginContent?.AccessToken);

			var client = _factory.CreateClient();
			client.DefaultRequestHeaders.Authorization = 
				new AuthenticationHeaderValue("Bearer", loginContent?.AccessToken);

			// Act
			var response = await client.GetAsync("/api/users/me");

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var userProfile = await response.Content.ReadFromJsonAsync<UserProfile>();
			Assert.NotNull(userProfile);
			Assert.NotEmpty(userProfile.Id);
			Assert.Equal(email, userProfile.Email);
		}

		[Fact]
		public async Task GetUserProfile_WithoutToken_ReturnsUnauthorized()
		{
			// Act
			var response = await _client.GetAsync("/api/users/me");

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		private class UserProfile
		{
			public string Id { get; set; } = string.Empty;
			public string UserName { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
		}

		private class LoginResponse
		{
			public string AccessToken { get; set; } = string.Empty;
			public string TokenType { get; set; } = string.Empty;
			public int ExpiresIn { get; set; }
		}
	}
}
