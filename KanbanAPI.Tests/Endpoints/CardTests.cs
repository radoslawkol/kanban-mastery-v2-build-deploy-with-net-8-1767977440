using KanbanAPI.DTOs;
using KanbanAPI.Models;
using KanbanAPI.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace KanbanAPI.Tests.Endpoints
{
	public class CardTests : ApiTestBase
	{
		public CardTests(WebApplicationFactory<Program> factory)
			: base(factory, "TestDb_Cards")
		{
		}

		[Fact]
		public async Task CreateCard_ByMember_ReturnsCreated()
		{
			var ownerClient = await CreateAuthenticatedClientAsync("owner-create-card-1@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Cards Board"));
			Assert.NotNull(boardResponse);
			Assert.True(boardResponse.IsSuccessStatusCode, $"Board creation failed: {boardResponse.StatusCode} - {await boardResponse.Content.ReadAsStringAsync()}");
			
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var memberEmail = "member-create-card-1@example.com";
			await _client.PostAsJsonAsync("/register", new { email = memberEmail, password = "Test@123" });

			var addMemberResponse = await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/members", new AddBoardMemberRequest(memberEmail));
			Assert.True(addMemberResponse.IsSuccessStatusCode, $"Add member failed: {addMemberResponse.StatusCode} - {await addMemberResponse.Content.ReadAsStringAsync()}");
			
			var memberClient = await CreateAuthenticatedClientAsync(memberEmail, "Test@123");

			var columnResponse = await memberClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));
			Assert.True(columnResponse.IsSuccessStatusCode, $"Column creation failed: {columnResponse.StatusCode} - {await columnResponse.Content.ReadAsStringAsync()}");
			
			var column = await columnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column);

			var response = await memberClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", "Description", column.Id));
			Assert.True(response.IsSuccessStatusCode, $"Card creation failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");

			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			var createdCard = await response.Content.ReadFromJsonAsync<CreateCardResponse>();
			Assert.NotNull(createdCard);
			Assert.Equal("Task", createdCard.Title);
			Assert.Equal(column.Id, createdCard.ColumnId);
		}

		[Fact]
		public async Task UpdateCard_ByMember_UpdateCorrectlyTitleDescription()
		{
			// Arrange
			var ownerClient = await CreateAuthenticatedClientAsync("owner-update-card-2@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Test Board 123"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var columnResponse = await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));
			var column = await columnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column);

			var memberEmail = "member-update-card-2@example.com";
			var memberClient = await CreateAuthenticatedClientAsync(memberEmail, "Test@123");

			await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/members", new AddBoardMemberRequest(memberEmail));

			var createCardResponse = await memberClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", "Description", column.Id));
			var createdCard = await createCardResponse.Content.ReadFromJsonAsync<CreateCardResponse>();
			Assert.NotNull(createdCard);

			// Act
			var updateCardRequest = new UpdateCardRequest("Task Updated", "Description Updated", column.Id, 0);
			var response = await memberClient.PutAsJsonAsync(
				$"/api/boards/{board.Id}/cards/{createdCard.Id}",
				updateCardRequest);

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var updatedCard = await response.Content.ReadFromJsonAsync<UpdateCardResponse>();
			Assert.NotNull(updatedCard);
			Assert.Equal(updateCardRequest.Title, updatedCard.Title);
			Assert.Equal(updateCardRequest.Description, updatedCard.Description);
			Assert.Equal(updateCardRequest.ColumnId, updatedCard.ColumnId);
		}

		[Fact]
		public async Task UpdateCard_ByMember_CanMoveBetweenColumns()
		{
			var ownerClient = await CreateAuthenticatedClientAsync("owner-update-card-1@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Move Card Board"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var fromColumnResponse = await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("In Progress", null));
			var toColumnResponse = await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Done", null));
			var fromColumn = await fromColumnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			var toColumn = await toColumnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(fromColumn);
			Assert.NotNull(toColumn);

			var createCardResponse = await ownerClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", "Description", fromColumn.Id));
			var createdCard = await createCardResponse.Content.ReadFromJsonAsync<CreateCardResponse>();
			Assert.NotNull(createdCard);

			var response = await ownerClient.PutAsJsonAsync(
				$"/api/boards/{board.Id}/cards/{createdCard.Id}",
				new UpdateCardRequest("Task Updated", "Moved", toColumn.Id, 0));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var updatedCard = await response.Content.ReadFromJsonAsync<UpdateCardResponse>();
			Assert.NotNull(updatedCard);
			Assert.Equal("Task Updated", updatedCard.Title);
			Assert.Equal(toColumn.Id, updatedCard.ColumnId);
		}

		[Fact]
		public async Task DeleteCard_ByMember_ReturnsNoContent()
		{
			var ownerClient = await CreateAuthenticatedClientAsync("owner-delete-card-1@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Delete Card Board"));
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

			var createCardResponse = await ownerClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", null, column.Id));
			var createdCard = await createCardResponse.Content.ReadFromJsonAsync<CreateCardResponse>();
			Assert.NotNull(createdCard);

			var response = await ownerClient.DeleteAsync($"/api/boards/{board.Id}/cards/{createdCard.Id}");

			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			db.ChangeTracker.Clear();
			var deletedCard = await db.Cards.FindAsync(createdCard.Id);
			Assert.Null(deletedCard);
		}

		[Fact]
		public async Task CreateCard_ByNonMember_ReturnsForbidden()
		{
			var ownerClient = await CreateAuthenticatedClientAsync("owner-create-card-2@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Private Cards Board"));
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

			var nonMemberClient = await CreateAuthenticatedClientAsync("nonmember-create-card-2@example.com", "Test@123");
			var response = await nonMemberClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", null, column.Id));

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task UpdateCard_ByNonMember_ReturnsForbidden()
		{
			var ownerClient = await CreateAuthenticatedClientAsync("owner-update-card-forbidden@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Forbidden Update Board"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var columnResponse = await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));
			var column = await columnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column);

			var createCardResponse = await ownerClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", null, column.Id));
			var createdCard = await createCardResponse.Content.ReadFromJsonAsync<CreateCardResponse>();
			Assert.NotNull(createdCard);

			var nonMemberClient = await CreateAuthenticatedClientAsync("nonmember-update-card@example.com", "Test@123");
			var response = await nonMemberClient.PutAsJsonAsync(
				$"/api/boards/{board.Id}/cards/{createdCard.Id}",
				new UpdateCardRequest("Task Updated", "Moved", column.Id, 0));

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task DeleteCard_ByNonMember_ReturnsForbidden()
		{
			var ownerClient = await CreateAuthenticatedClientAsync("owner-delete-card-forbidden@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Forbidden Delete Board"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var columnResponse = await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));
			var column = await columnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column);

			var createCardResponse = await ownerClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", null, column.Id));
			var createdCard = await createCardResponse.Content.ReadFromJsonAsync<CreateCardResponse>();
			Assert.NotNull(createdCard);

			var nonMemberClient = await CreateAuthenticatedClientAsync("nonmember-delete-card@example.com", "Test@123");
			var response = await nonMemberClient.DeleteAsync($"/api/boards/{board.Id}/cards/{createdCard.Id}");

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task AssignCard_ToValidBoardMember_ReturnsOkWithAssignedUserId()
		{
			var ownerClient = await CreateAuthenticatedClientAsync("owner-assign-card-1@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Assign Card Board"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var memberEmail = "member-assign-card-1@example.com";
			await _client.PostAsJsonAsync("/register", new { email = memberEmail, password = "Test@123" });
			await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/members", new AddBoardMemberRequest(memberEmail));
			var memberClient = await CreateAuthenticatedClientAsync(memberEmail, "Test@123");

			using var db = CreateDbContext();
			var member = await db.Users.SingleOrDefaultAsync(u => u.Email == memberEmail);
			Assert.NotNull(member);

			var columnResponse = await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));
			var column = await columnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column);

			var createCardResponse = await ownerClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", "Description", column.Id));
			var createdCard = await createCardResponse.Content.ReadFromJsonAsync<CreateCardResponse>();
			Assert.NotNull(createdCard);

			var response = await memberClient.PutAsJsonAsync(
				$"/api/boards/{board.Id}/cards/{createdCard.Id}/assign",
				new AssignCardRequest(member.Id));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var assignedCard = await response.Content.ReadFromJsonAsync<AssignCardResponse>();
			Assert.NotNull(assignedCard);
			Assert.Equal(member.Id, assignedCard.AssignedToUserId);
			Assert.Equal(createdCard.Id, assignedCard.Id);
		}

		[Fact]
		public async Task AssignCard_ToNonBoardMember_ReturnsBadRequest()
		{
			var ownerClient = await CreateAuthenticatedClientAsync("owner-assign-card-2@example.com", "Test@123");
			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Assign Card Board 2"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var columnResponse = await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));
			var column = await columnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column);

			var createCardResponse = await ownerClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", "Description", column.Id));
			var createdCard = await createCardResponse.Content.ReadFromJsonAsync<CreateCardResponse>();
			Assert.NotNull(createdCard);

			var nonMemberEmail = "nonmember-assign-card-2@example.com";
			await _client.PostAsJsonAsync("/register", new { email = nonMemberEmail, password = "Test@123" });

			using var db = CreateDbContext();
			var nonMember = await db.Users.SingleOrDefaultAsync(u => u.Email == nonMemberEmail);
			Assert.NotNull(nonMember);

			var response = await ownerClient.PutAsJsonAsync(
				$"/api/boards/{board.Id}/cards/{createdCard.Id}/assign",
				new AssignCardRequest(nonMember.Id));

			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task CreateCard_WithoutAuth_ReturnsUnauthorized()
		{
			// Arrange
			var boardId = Guid.NewGuid();
			var columnId = Guid.NewGuid();

			// Act
			var response = await _client.PostAsJsonAsync(
				$"/api/boards/{boardId}/cards/",
				new CreateCardRequest("Task", "Description", columnId));

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task UpdateCard_WithoutAuth_ReturnsUnauthorized()
		{
			// Arrange
			var boardId = Guid.NewGuid();
			var cardId = Guid.NewGuid();

			// Act
			var response = await _client.PutAsJsonAsync(
				$"/api/boards/{boardId}/cards/{cardId}",
				new UpdateCardRequest("Task Updated", "Description", Guid.NewGuid(), 0));

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task DeleteCard_WithoutAuth_ReturnsUnauthorized()
		{
			// Arrange
			var boardId = Guid.NewGuid();
			var cardId = Guid.NewGuid();

			// Act
			var response = await _client.DeleteAsync($"/api/boards/{boardId}/cards/{cardId}");

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task AssignCard_WithoutAuth_ReturnsUnauthorized()
		{
			// Arrange
			var boardId = Guid.NewGuid();
			var cardId = Guid.NewGuid();
			var userId = "some-user-id";

			// Act
			var response = await _client.PutAsJsonAsync(
				$"/api/boards/{boardId}/cards/{cardId}/assign",
				new AssignCardRequest(userId));

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task AssignCard_ByNonMember_ReturnsForbidden()
		{
			// Arrange
			var ownerClient = await CreateAuthenticatedClientAsync("owner-assign-forbidden@example.com", "Test@123");
			var nonMemberClient = await CreateAuthenticatedClientAsync("nonmember-assign@example.com", "Test@123");

			var boardResponse = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Assign Board"));
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var columnResponse = await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));
			var column = await columnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column);

			var createCardResponse = await ownerClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", "Description", column.Id));
			var createdCard = await createCardResponse.Content.ReadFromJsonAsync<CreateCardResponse>();
			Assert.NotNull(createdCard);

			var memberEmail = "member-assign-forbidden@example.com";
			await _client.PostAsJsonAsync("/register", new { email = memberEmail, password = "Test@123" });

			using var db = CreateDbContext();
			var member = await db.Users.SingleOrDefaultAsync(u => u.Email == memberEmail);
			Assert.NotNull(member);

			// Act
			var response = await nonMemberClient.PutAsJsonAsync(
				$"/api/boards/{board.Id}/cards/{createdCard.Id}/assign",
				new AssignCardRequest(member.Id));

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task CreateCard_ToColumnFromDifferentBoard_ReturnsBadRequest()
		{
			// Arrange
			var ownerClient = await CreateAuthenticatedClientAsync("owner-cross-board@example.com", "Test@123");
			
			var board1Response = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Board 1"));
			var board1 = await board1Response.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board1);

			var board2Response = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Board 2"));
			var board2 = await board2Response.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board2);

			// Create column in board 2
			var columnResponse = await ownerClient.PostAsJsonAsync($"/api/boards/{board2.Id}/columns/", new CreateColumnRequest("Todo", null));
			var column = await columnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column);

			// Act - Try to create card in board 1 but with column from board 2
			var response = await ownerClient.PostAsJsonAsync(
				$"/api/boards/{board1.Id}/cards/",
				new CreateCardRequest("Task", "Description", column.Id));

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task UpdateCard_ToColumnFromDifferentBoard_ReturnsBadRequest()
		{
			// Arrange
			var ownerClient = await CreateAuthenticatedClientAsync("owner-cross-board-update@example.com", "Test@123");
			
			var board1Response = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Board 1 Update"));
			var board1 = await board1Response.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board1);

			var board2Response = await ownerClient.PostAsJsonAsync("/api/boards/", new CreateBoardRequest("Board 2 Update"));
			var board2 = await board2Response.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board2);

			// Create column in both boards
			var column1Response = await ownerClient.PostAsJsonAsync($"/api/boards/{board1.Id}/columns/", new CreateColumnRequest("Todo", null));
			var column1 = await column1Response.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column1);

			var column2Response = await ownerClient.PostAsJsonAsync($"/api/boards/{board2.Id}/columns/", new CreateColumnRequest("Done", null));
			var column2 = await column2Response.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column2);

			// Create card in board 1
			var createCardResponse = await ownerClient.PostAsJsonAsync(
				$"/api/boards/{board1.Id}/cards/",
				new CreateCardRequest("Task", "Description", column1.Id));
			var createdCard = await createCardResponse.Content.ReadFromJsonAsync<CreateCardResponse>();
			Assert.NotNull(createdCard);

			// Act - Try to move card to column from board 2
			var response = await ownerClient.PutAsJsonAsync(
				$"/api/boards/{board1.Id}/cards/{createdCard.Id}",
				new UpdateCardRequest("Task Updated", "Description", column2.Id, 0));

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
	}
}
