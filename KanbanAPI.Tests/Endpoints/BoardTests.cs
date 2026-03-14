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

		[Fact]
		public async Task AddMember_ByOwner_ReturnsOkAndAddsMember()
		{
			// Arrange
			var ownerEmail = "owner@example.com";
			var ownerClient = await CreateAuthenticatedClientAsync(ownerEmail, "Test@123");
			
			// Create a board as owner
			var createBoardRequest = new CreateBoardRequest("Test Board");
			var createResponse = await ownerClient.PostAsJsonAsync("/api/boards/", createBoardRequest);
			var board = await createResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			// Register a new user to add as member
			var memberEmail = "member@example.com";
			await _client.PostAsJsonAsync("/register", new { email = memberEmail, password = "Test@123" });

			// Get the member's user ID from database
			using var db = CreateDbContext();
			var memberUser = await db.Users.SingleOrDefaultAsync(u => u.Email == memberEmail);
			Assert.NotNull(memberUser);

			var addMemberRequest = new AddBoardMemberRequest(memberUser.Id);

			// Act
			var response = await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/members", addMemberRequest);

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			// Verify member was added to database
			var memberRecord = await db.BoardMembers
				.SingleOrDefaultAsync(bm => bm.BoardId == board.Id && bm.UserId == memberUser.Id);
			Assert.NotNull(memberRecord);
			Assert.Equal(BoardRole.Member, memberRecord.Role);
		}

		[Fact]
		public async Task AddMember_ByNonOwner_ReturnsForbidden()
		{
			// Arrange
			var ownerEmail = "owner2@example.com";
			var ownerClient = await CreateAuthenticatedClientAsync(ownerEmail, "Test@123");
			
			// Create a board as owner
			var createBoardRequest = new CreateBoardRequest("Test Board 2");
			var createResponse = await ownerClient.PostAsJsonAsync("/api/boards/", createBoardRequest);
			var board = await createResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			// Register two other users: one to add as member, one as non-owner
			var memberEmail = "member2@example.com";
			var nonOwnerEmail = "nonowner@example.com";
			await _client.PostAsJsonAsync("/register", new { email = memberEmail, password = "Test@123" });
			await _client.PostAsJsonAsync("/register", new { email = nonOwnerEmail, password = "Test@123" });

			// Get user IDs from database
			using var db = CreateDbContext();
			var memberUser = await db.Users.SingleOrDefaultAsync(u => u.Email == memberEmail);
			var nonOwnerUser = await db.Users.SingleOrDefaultAsync(u => u.Email == nonOwnerEmail);
			Assert.NotNull(memberUser);
			Assert.NotNull(nonOwnerUser);

			// Add non-owner as a regular member to the board
			var addNonOwnerRequest = new AddBoardMemberRequest(nonOwnerUser.Id);
			await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/members", addNonOwnerRequest);

			// Create authenticated client for non-owner
			var nonOwnerClient = await CreateAuthenticatedClientAsync(nonOwnerEmail, "Test@123");

			var addMemberRequest = new AddBoardMemberRequest(memberUser.Id);

			// Act
			var response = await nonOwnerClient.PostAsJsonAsync($"/api/boards/{board.Id}/members", addMemberRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task GetBoard_ByMember_ReturnsBoardWithColumnsAndCards()
		{
			// Arrange
			var ownerEmail = "boardowner4@example.com";
			var ownerClient = await CreateAuthenticatedClientAsync(ownerEmail, "Test@123");

			var createBoardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Member Board"));
			var createdBoard = await createBoardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(createdBoard);

			var memberEmail = "boardmember4@example.com";
			await _client.PostAsJsonAsync("/register", new { email = memberEmail, password = "Test@123" });

			using var db = CreateDbContext();
			var memberUser = await db.Users.SingleOrDefaultAsync(u => u.Email == memberEmail);
			Assert.NotNull(memberUser);

			await ownerClient.PostAsJsonAsync($"/api/boards/{createdBoard.Id}/members", new AddBoardMemberRequest(memberUser.Id));

			var memberClient = await CreateAuthenticatedClientAsync(memberEmail, "Test@123");

			// Act
			var response = await memberClient.GetAsync($"/api/boards/{createdBoard.Id}");

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var boardDetail = await response.Content.ReadFromJsonAsync<BoardDetailResponse>();
			Assert.NotNull(boardDetail);
			Assert.Equal(createdBoard.Id, boardDetail.Id);
			Assert.Equal("Member Board", boardDetail.Name);
			Assert.NotNull(boardDetail.Columns);
		}

		[Fact]
		public async Task GetBoard_ByNonMember_ReturnsForbidden()
		{
			// Arrange
			var ownerEmail = "boardowner5@example.com";
			var ownerClient = await CreateAuthenticatedClientAsync(ownerEmail, "Test@123");

			var createBoardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Private Board"));
			var createdBoard = await createBoardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(createdBoard);

			var nonMemberEmail = "nonmember5@example.com";
			var nonMemberClient = await CreateAuthenticatedClientAsync(nonMemberEmail, "Test@123");

			// Act
			var response = await nonMemberClient.GetAsync($"/api/boards/{createdBoard.Id}");

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task CreateColumn_ByMember_ReturnsCreated()
		{
			// Arrange
			var ownerClient = await CreateAuthenticatedClientAsync("owner-columns-1@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Columns Board"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var memberEmail = "member-columns-1@example.com";
			await _client.PostAsJsonAsync("/register", new { email = memberEmail, password = "Test@123" });

			using var db = CreateDbContext();
			var member = await db.Users.SingleOrDefaultAsync(u => u.Email == memberEmail);
			Assert.NotNull(member);

			await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/members", new AddBoardMemberRequest(member.Id));
			var memberClient = await CreateAuthenticatedClientAsync(memberEmail, "Test@123");

			// Act
			var response = await memberClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));

			// Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			var createdColumn = await response.Content.ReadFromJsonAsync<ColumnResponse>();
			Assert.NotNull(createdColumn);
			Assert.Equal("Todo", createdColumn.Name);
		}

		[Fact]
		public async Task CreateColumn_ByNonMember_ReturnsForbidden()
		{
			// Arrange
			var ownerClient = await CreateAuthenticatedClientAsync("owner-columns-2@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Private Columns Board"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var nonMemberClient = await CreateAuthenticatedClientAsync("nonmember-columns-2@example.com", "Test@123");

			// Act
			var response = await nonMemberClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task UpdateColumn_ByMember_ReturnsOk()
		{
			// Arrange
			var ownerClient = await CreateAuthenticatedClientAsync("owner-update-col-1@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Update Columns Board"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var memberEmail = "member-update-col-1@example.com";
			await _client.PostAsJsonAsync("/register", new { email = memberEmail, password = "Test@123" });

			using var db = CreateDbContext();
			var member = await db.Users.SingleOrDefaultAsync(u => u.Email == memberEmail);
			Assert.NotNull(member);

			await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/members", new AddBoardMemberRequest(member.Id));
			var memberClient = await CreateAuthenticatedClientAsync(memberEmail, "Test@123");

			// Create a column first
			var columnResponse = await memberClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));
			var column = await columnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column);

			// Act
			var response = await memberClient.PutAsJsonAsync($"/api/boards/{board.Id}/columns/{column.Id}", new UpdateColumnRequest("Updated Todo"));

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var updatedColumn = await response.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(updatedColumn);
			Assert.Equal("Updated Todo", updatedColumn.Name);
		}

		[Fact]
		public async Task UpdateColumn_ByNonMember_ReturnsForbidden()
		{
			// Arrange
			var ownerClient = await CreateAuthenticatedClientAsync("owner-update-col-2@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Update Columns Board 2"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			using var db = CreateDbContext();
			var column = new Column
			{
				Id = Guid.NewGuid(),
				BoardId = board.Id,
				Name = "Todo",
				Order = 0
			};
			db.Columns.Add(column);
			await db.SaveChangesAsync();

			var nonMemberClient = await CreateAuthenticatedClientAsync("nonmember-update-col-2@example.com", "Test@123");

			// Act
			var response = await nonMemberClient.PutAsJsonAsync($"/api/boards/{board.Id}/columns/{column.Id}", new UpdateColumnRequest("Updated"));

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task DeleteColumn_ByMember_ReturnsNoContent()
		{
			// Arrange
			var ownerClient = await CreateAuthenticatedClientAsync("owner-delete-col-1@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Delete Columns Board"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var memberEmail = "member-delete-col-1@example.com";
			await _client.PostAsJsonAsync("/register", new { email = memberEmail, password = "Test@123" });

			using var db = CreateDbContext();
			var member = await db.Users.SingleOrDefaultAsync(u => u.Email == memberEmail);
			Assert.NotNull(member);

			await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/members", new AddBoardMemberRequest(member.Id));
			var memberClient = await CreateAuthenticatedClientAsync(memberEmail, "Test@123");

			// Create a column first
			var columnResponse = await memberClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));
			var column = await columnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column);

			// Act
			var response = await memberClient.DeleteAsync($"/api/boards/{board.Id}/columns/{column.Id}");

			// Assert
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			// Verify column was deleted
			var deletedColumn = await db.Columns.FindAsync(column.Id);
			Assert.Null(deletedColumn);
		}

		[Fact]
		public async Task DeleteColumn_ByNonMember_ReturnsForbidden()
		{
			// Arrange
			var ownerClient = await CreateAuthenticatedClientAsync("owner-delete-col-2@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Delete Columns Board 2"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			using var db = CreateDbContext();
			var column = new Column
			{
				Id = Guid.NewGuid(),
				BoardId = board.Id,
				Name = "Todo",
				Order = 0
			};
			db.Columns.Add(column);
			await db.SaveChangesAsync();

			var nonMemberClient = await CreateAuthenticatedClientAsync("nonmember-delete-col-2@example.com", "Test@123");

			// Act
			var response = await nonMemberClient.DeleteAsync($"/api/boards/{board.Id}/columns/{column.Id}");

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task DeleteColumn_WithExistingCards_ReturnsBadRequest()
		{
			// Arrange
			var ownerClient = await CreateAuthenticatedClientAsync("owner-columns-3@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Cards Board"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			using var db = CreateDbContext();
			var column = new Column
			{
				Id = Guid.NewGuid(),
				BoardId = board.Id,
				Name = "In Progress",
				Order = 0
			};
			db.Columns.Add(column);
			db.Cards.Add(new Card
			{
				Id = Guid.NewGuid(),
				ColumnId = column.Id,
				Title = "Task",
				Description = "Task Description",
				Order = 0
			});
			await db.SaveChangesAsync();

			// Act
			var response = await ownerClient.DeleteAsync($"/api/boards/{board.Id}/columns/{column.Id}");

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
	}
}