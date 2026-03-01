using KanbanAPI.DTOs;
using KanbanAPI.Models;
using KanbanAPI.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace KanbanAPI.Tests.Endpoints
{
	public class BoardTests : ApiTestBase
	{
		public BoardTests(WebApplicationFactory<Program> factory)
			: base(factory, "TestDb_Board") { }

		[Fact]
		public async Task CreateBoard_WithValidData_ReturnsCreatedWithBoardData()
		{
			// Arrange
			var client = await CreateAuthenticatedClientAsync("test@example.com", "Test@123");
			var request = new CreateBoardRequest("Test Board");

			// Act
			var response = await client.PostAsJsonAsync("/api/boards/", request);

			// Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);

			var board = await response.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);
			Assert.Equal("Test Board", board.BoardName);
			Assert.NotEqual(Guid.Empty, board.Id);
		}

		[Fact]
		public async Task CreateBoard_WithValidData_CreatesBoardMemberWithOwnerRole()
		{
			// Arrange
			var email = "owner@example.com";
			var client = await CreateAuthenticatedClientAsync(email, "Test@123");
			var request = new CreateBoardRequest("Owner Board");

			// Act
			var response = await client.PostAsJsonAsync("/api/boards/", request);

			Assert.Equal(HttpStatusCode.Created, response.StatusCode);

			var board = await response.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			// Assert
			using var db = CreateDbContext();

			var boardRecord = await db.Boards.FindAsync(board.Id);
			Assert.NotNull(boardRecord);
			Assert.Equal("Owner Board", boardRecord.Name);

			var userRecord = await db.Users.SingleOrDefaultAsync(u => u.Email == email);
			Assert.NotNull(userRecord);

			var memberRecord = await db.BoardMembers
				.SingleOrDefaultAsync(bm => bm.BoardId == board.Id && bm.UserId == userRecord.Id);
			Assert.NotNull(memberRecord);
			Assert.Equal(BoardRole.Owner, memberRecord.Role);
		}

		[Fact]
		public async Task CreateBoard_WithoutAuth_ReturnsUnauthorized()
		{
			// Arrange
			var request = new CreateBoardRequest("Unauthorized Board");

			// Act
			var response = await _client.PostAsJsonAsync("/api/boards/", request);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task CreateBoard_WithEmptyBoardName_ReturnsBadRequest()
		{
			// Arrange
			var client = await CreateAuthenticatedClientAsync("empty@example.com", "Test@123");
			var request = new CreateBoardRequest("");

			// Act
			var response = await client.PostAsJsonAsync("/api/boards/", request);

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task CreateBoard_WithBoardNameExceedingMaxLength_ReturnsBadRequest()
		{
			// Arrange
			var client = await CreateAuthenticatedClientAsync("toolong@example.com", "Test@123");
			var request = new CreateBoardRequest(new string('A', 101));
			// Act
			var response = await client.PostAsJsonAsync("/api/boards/", request);

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
	}
}