using KanbanAPI.Data;
using KanbanAPI.Exceptions;
using KanbanAPI.Models;
using KanbanAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace KanbanAPI.Tests.Services
{
	public class CardServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _context;
		private readonly CardService _cardService;

		public CardServiceTests()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			_context = new ApplicationDbContext(options);
			_cardService = new CardService(_context);
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}

		[Fact]
		public async Task CreateCardAsync_WithValidData_ShouldCreateCard()
		{
			var board = await CreateBoardAsync();
			var column = await CreateColumnAsync(board.Id, "Todo", 0);

			var card = await _cardService.CreateCardAsync(board.Id, "Task 1", "Description", column.Id);

			Assert.NotEqual(Guid.Empty, card.Id);
			Assert.Equal("Task 1", card.Title);
			Assert.Equal("Description", card.Description);
			Assert.Equal(column.Id, card.ColumnId);
			Assert.Equal(0, card.Order);
		}

		[Fact]
		public async Task CreateCardAsync_WithUnknownColumn_ShouldThrowNotFoundException()
		{
			var board = await CreateBoardAsync();

			await Assert.ThrowsAsync<NotFoundException>(() =>
				_cardService.CreateCardAsync(board.Id, "Task 1", null, Guid.NewGuid()));
		}

		[Fact]
		public async Task UpdateCardAsync_WithColumnChange_ShouldMoveCard()
		{
			var board = await CreateBoardAsync();
			var todo = await CreateColumnAsync(board.Id, "Todo", 0);
			var done = await CreateColumnAsync(board.Id, "Done", 1);
			var card = await _cardService.CreateCardAsync(board.Id, "Task 1", "Initial", todo.Id);

			var updatedCard = await _cardService.UpdateCardAsync(board.Id, card.Id, "Task 1 Updated", "Moved", done.Id);

			Assert.Equal("Task 1 Updated", updatedCard.Title);
			Assert.Equal("Moved", updatedCard.Description);
			Assert.Equal(done.Id, updatedCard.ColumnId);
		}

		[Fact]
		public async Task UpdateCardAsync_WithColumnFromAnotherBoard_ShouldThrowNotFoundException()
		{
			var board1 = await CreateBoardAsync();
			var board2 = await CreateBoardAsync();
			var board1Column = await CreateColumnAsync(board1.Id, "Todo", 0);
			var board2Column = await CreateColumnAsync(board2.Id, "Done", 0);
			var card = await _cardService.CreateCardAsync(board1.Id, "Task", null, board1Column.Id);

			await Assert.ThrowsAsync<NotFoundException>(() =>
				_cardService.UpdateCardAsync(board1.Id, card.Id, "Task", null, board2Column.Id));
		}

		[Fact]
		public async Task DeleteCardAsync_WithValidData_ShouldDeleteCard()
		{
			var board = await CreateBoardAsync();
			var column = await CreateColumnAsync(board.Id, "Todo", 0);
			var card = await _cardService.CreateCardAsync(board.Id, "Task 1", null, column.Id);

			await _cardService.DeleteCardAsync(board.Id, card.Id);

			var deleted = await _context.Cards.FindAsync(card.Id);
			Assert.Null(deleted);
		}

		[Fact]
		public async Task DeleteCardAsync_WithWrongBoard_ShouldThrowNotFoundException()
		{
			var board1 = await CreateBoardAsync();
			var board2 = await CreateBoardAsync();
			var column = await CreateColumnAsync(board1.Id, "Todo", 0);
			var card = await _cardService.CreateCardAsync(board1.Id, "Task 1", null, column.Id);

			await Assert.ThrowsAsync<NotFoundException>(() =>
				_cardService.DeleteCardAsync(board2.Id, card.Id));
		}

		[Fact]
		public async Task AssignCardAsync_WithValidCardId_ShouldSetAssignedToUserId()
		{
			var board = await CreateBoardAsync();
			var column = await CreateColumnAsync(board.Id, "Todo", 0);
			var card = await _cardService.CreateCardAsync(board.Id, "Task 1", null, column.Id);
			var userId = "user-abc";

			var result = await _cardService.AssignCardAsync(card.Id, userId);

			Assert.NotNull(result);
			Assert.Equal(userId, result.AssignedToUserId);
			Assert.Equal(card.Id, result.Id);
		}

		[Fact]
		public async Task AssignCardAsync_WithUnknownCardId_ShouldReturnNull()
		{
			var result = await _cardService.AssignCardAsync(Guid.NewGuid(), "user-abc");

			Assert.Null(result);
		}

		private async Task<Board> CreateBoardAsync()
		{
			var board = new Board
			{
				Id = Guid.NewGuid(),
				Name = $"Board {Guid.NewGuid()}"
			};

			_context.Boards.Add(board);
			await _context.SaveChangesAsync();
			return board;
		}

		private async Task<Column> CreateColumnAsync(Guid boardId, string name, int order)
		{
			var column = new Column
			{
				Id = Guid.NewGuid(),
				BoardId = boardId,
				Name = name,
				Order = order
			};

			_context.Columns.Add(column);
			await _context.SaveChangesAsync();
			return column;
		}
	}
}
