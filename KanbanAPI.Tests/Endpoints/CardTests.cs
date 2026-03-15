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
			var board = await boardResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
			Assert.NotNull(board);

			var memberEmail = "member-create-card-1@example.com";
			await _client.PostAsJsonAsync("/register", new { email = memberEmail, password = "Test@123" });

			using var db = CreateDbContext();
			var member = await db.Users.SingleOrDefaultAsync(u => u.Email == memberEmail);
			Assert.NotNull(member);

			await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/members", new AddBoardMemberRequest(member.Id));
			var memberClient = await CreateAuthenticatedClientAsync(memberEmail, "Test@123");

			var columnResponse = await memberClient.PostAsJsonAsync($"/api/boards/{board.Id}/columns/", new CreateColumnRequest("Todo", null));
			var column = await columnResponse.Content.ReadFromJsonAsync<CreateColumnResponse>();
			Assert.NotNull(column);

			var response = await memberClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", "Description", column.Id));

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

			using var db = CreateDbContext();
			var member = await db.Users.SingleOrDefaultAsync(u => u.Email == memberEmail);
			Assert.NotNull(member);

			await ownerClient.PostAsJsonAsync($"/api/boards/{board.Id}/members", new AddBoardMemberRequest(member.Id));

			var createCardResponse = await memberClient.PostAsJsonAsync(
				$"/api/boards/{board.Id}/cards/",
				new CreateCardRequest("Task", "Description", column.Id));
			var createdCard = await createCardResponse.Content.ReadFromJsonAsync<CreateCardResponse>();
			Assert.NotNull(createdCard);

			// Act
			var updateCardRequest = new UpdateCardRequest("Task Updated", "Description Updated", column.Id);
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
				new UpdateCardRequest("Task Updated", "Moved", toColumn.Id));

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
				new UpdateCardRequest("Task Updated", "Moved", column.Id));

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
	}
}
