using System.Net;
using System.Net.Http.Json;
using KanbanAPI.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace KanbanAPI.Tests.Endpoints
{
	public class UserProfile : ApiTestBase
	{
		public UserProfile(WebApplicationFactory<Program> factory)
			: base(factory, "TestDb_UserProfile") { }

		[Fact]
		public async Task GetUserProfile_WithValidToken_ReturnsOkWithUserData()
		{
			// Arrange
			var email = "profile@example.com";
			var password = "ProfileTest123!@#";

			var client = await CreateAuthenticatedClientAsync(email, password);

			// Act
			var response = await client.GetAsync("/api/users/me");

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var userProfile = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
			Assert.NotNull(userProfile);
			Assert.NotEmpty(userProfile.Id);
			Assert.Equal(email, userProfile.Email);
		}

		[Fact]
		public async Task GetUserProfile_WithoutAuth_ReturnsUnauthorized()
		{
			var response = await _client.GetAsync("/api/users/me");

			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		private class UserProfileResponse
		{
			public string Id { get; set; } = string.Empty;
			public string UserName { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
		}
	}
}
