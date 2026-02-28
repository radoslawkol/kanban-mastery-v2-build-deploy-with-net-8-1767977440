using KanbanAPI.Data;
using KanbanAPI.Exceptions;
using KanbanAPI.Models;
using KanbanAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace KanbanAPI.Tests.Services
{
	public class UserServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _context;
		private readonly UserService _userService;
		private readonly string _testUserId = "test-user-id";

		public UserServiceTests()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new ApplicationDbContext(options);
			_userService = new UserService(_context);

			_context.Users.Add(new ApplicationUser
			{
				Id = _testUserId,
				UserName = "testuser",
				Email = "testuser@example.com"
			});
			_context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}

		[Fact]
		public async Task GetUserProfileAsync_WhenUserExists_ShouldReturnUser()
		{
			// Act
			var result = await _userService.GetUserProfileAsync(_testUserId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(_testUserId, result.Id);
			Assert.Equal("testuser", result.UserName);
			Assert.Equal("testuser@example.com", result.Email);
		}

		[Fact]
		public async Task GetUserProfileAsync_WhenUserDoesNotExist_ShouldThrowNotFoundException()
		{
			var nonExistentUserId = "non-existent-id";

			await Assert.ThrowsAsync<NotFoundException>(
				() => _userService.GetUserProfileAsync(nonExistentUserId)
			);
		}

		[Fact]
		public async Task GetUserProfileAsync_WhenUserExists_ShouldReturnCorrectUser()
		{
			// Arrange
			var otherUserId = "other-user-id";
			_context.Users.Add(new ApplicationUser
			{
				Id = otherUserId,
				UserName = "otheruser",
				Email = "other@example.com"
			});
			await _context.SaveChangesAsync();

			// Act
			var result = await _userService.GetUserProfileAsync(otherUserId);

			// Assert 
			Assert.NotNull(result);
			Assert.Equal(otherUserId, result.Id);
			Assert.Equal("otheruser", result.UserName);
		}
	}
}
