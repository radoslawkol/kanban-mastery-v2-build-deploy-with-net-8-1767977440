using System.Net;
using System.Net.Http.Json;
using KanbanAPI.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace KanbanAPI.Tests.Endpoints
{
	public class AuthTests : ApiTestBase
	{
		public AuthTests(WebApplicationFactory<Program> factory)
			: base(factory, "TestDb_Auth") { }

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
	}
}
