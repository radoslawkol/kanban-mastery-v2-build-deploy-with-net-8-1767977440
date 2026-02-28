using System.Net.Http.Headers;
using System.Net.Http.Json;
using KanbanAPI.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KanbanAPI.Tests.Helpers
{
	public abstract class ApiTestBase : IClassFixture<WebApplicationFactory<Program>>
	{
		protected readonly HttpClient _client;
		protected readonly WebApplicationFactory<Program> _factory;

		protected ApiTestBase(WebApplicationFactory<Program> factory, string databaseName)
		{
			_factory = factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureServices(services =>
				{
					var descriptor = services.SingleOrDefault(
						d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

					if (descriptor is not null) services.Remove(descriptor);

					services.AddDbContext<ApplicationDbContext>(options =>
						options.UseInMemoryDatabase(databaseName));
				});
			});

			_client = _factory.CreateClient();
		}

		protected async Task<HttpClient> CreateAuthenticatedClientAsync(string email, string password)
		{
			var registrationData = new { email, password };
			await _client.PostAsJsonAsync("/register", registrationData);

			var loginData = new { email, password };
			var loginResponse = await _client.PostAsJsonAsync("/login", loginData);
			var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

			var client = _factory.CreateClient();
			client.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", loginContent?.AccessToken);

			return client;
		}
	}
}
