using KanbanAPI.Data;
using KanbanAPI.Exceptions;
using KanbanAPI.Models;
using KanbanAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace KanbanAPI.Tests.Services
{
	public class ColumnServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _context;
		private readonly ColumnService _columnService;
		private readonly string _testUserId = "test-user-id";
		private readonly string _otherUserId = "other-user-id";

		public ColumnServiceTests()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new ApplicationDbContext(options);
			_columnService = new ColumnService(_context);

			_context.Users.Add(new ApplicationUser { Id = _testUserId, UserName = "testuser" });
			_context.Users.Add(new ApplicationUser { Id = _otherUserId, UserName = "otheruser" });
			_context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}

		#region CreateColumnAsync Tests

		[Fact]
		public async Task CreateColumnAsync_WithValidData_ShouldCreateColumn()
		{
			// Arrange
			var board = await CreateTestBoard();
			var columnName = "To Do";

			// Act
			var result = await _columnService.CreateColumnAsync(board.Id, columnName, null);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(columnName, result.Name);
			Assert.Equal(board.Id, result.BoardId);
			Assert.Equal(0, result.Order);
			Assert.NotEqual(Guid.Empty, result.Id);

			var savedColumn = await _context.Columns.FindAsync(result.Id);
			Assert.NotNull(savedColumn);
		}

		[Fact]
		public async Task CreateColumnAsync_WithPosition_ShouldInsertAtCorrectPosition()
		{
			// Arrange
			var board = await CreateTestBoard();
			await _columnService.CreateColumnAsync(board.Id, "Column 1", null);
			await _columnService.CreateColumnAsync(board.Id, "Column 2", null);

			// Act
			var result = await _columnService.CreateColumnAsync(board.Id, "Column 1.5", 1);

			// Assert
			Assert.Equal(1, result.Order);

			var allColumns = await _context.Columns
				.Where(c => c.BoardId == board.Id)
				.OrderBy(c => c.Order)
				.ToListAsync();

			Assert.Equal(3, allColumns.Count);
			Assert.Equal("Column 1", allColumns[0].Name);
			Assert.Equal("Column 1.5", allColumns[1].Name);
			Assert.Equal("Column 2", allColumns[2].Name);
			Assert.Equal(2, allColumns[2].Order);
		}

		[Fact]
		public async Task CreateColumnAsync_WithNegativePosition_ShouldInsertAtBeginning()
		{
			// Arrange
			var board = await CreateTestBoard();
			await _columnService.CreateColumnAsync(board.Id, "Column 1", null);

			// Act
			var result = await _columnService.CreateColumnAsync(board.Id, "First", -5);

			// Assert
			Assert.Equal(0, result.Order);

			var allColumns = await _context.Columns
				.Where(c => c.BoardId == board.Id)
				.OrderBy(c => c.Order)
				.ToListAsync();

			Assert.Equal("First", allColumns[0].Name);
			Assert.Equal("Column 1", allColumns[1].Name);
		}

		[Fact]
		public async Task CreateColumnAsync_WithPositionBeyondCount_ShouldInsertAtEnd()
		{
			// Arrange
			var board = await CreateTestBoard();
			await _columnService.CreateColumnAsync(board.Id, "Column 1", null);

			// Act
			var result = await _columnService.CreateColumnAsync(board.Id, "Last", 100);

			// Assert
			Assert.Equal(1, result.Order);
		}

		[Fact]
		public async Task CreateColumnAsync_WithEmptyName_ShouldThrowArgumentException()
		{
			// Arrange
			var board = await CreateTestBoard();

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentException>(
				() => _columnService.CreateColumnAsync(board.Id, "", null)
			);
		}

		[Fact]
		public async Task CreateColumnAsync_WithWhitespaceName_ShouldThrowArgumentException()
		{
			// Arrange
			var board = await CreateTestBoard();

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentException>(
				() => _columnService.CreateColumnAsync(board.Id, "   ", null)
			);
		}

		[Fact]
		public async Task CreateColumnAsync_WithNonExistentBoard_ShouldThrowNotFoundException()
		{
			// Arrange
			var nonExistentBoardId = Guid.NewGuid();

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(
				() => _columnService.CreateColumnAsync(nonExistentBoardId, "Column", null)
			);
		}

		#endregion

		#region UpdateColumnAsync Tests

		[Fact]
		public async Task UpdateColumnAsync_WithValidData_ShouldUpdateColumn()
		{
			// Arrange
			var board = await CreateTestBoard();
			var column = await _columnService.CreateColumnAsync(board.Id, "Old Name", null);
			var newName = "New Name";

			// Act
			var result = await _columnService.UpdateColumnAsync(board.Id, column.Id, newName);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(newName, result.Name);
			Assert.Equal(column.Id, result.Id);

			var updatedColumn = await _context.Columns.FindAsync(column.Id);
			Assert.Equal(newName, updatedColumn!.Name);
		}

		[Fact]
		public async Task UpdateColumnAsync_ShouldUpdateModifiedOn()
		{
			// Arrange
			var board = await CreateTestBoard();
			var column = await _columnService.CreateColumnAsync(board.Id, "Column", null);
			var originalModifiedOn = column.ModifiedOn;
			await Task.Delay(100);

			// Act
			var result = await _columnService.UpdateColumnAsync(board.Id, column.Id, "Updated");

			// Assert
			Assert.True(result.ModifiedOn > originalModifiedOn);
		}

		[Fact]
		public async Task UpdateColumnAsync_WithEmptyName_ShouldThrowArgumentException()
		{
			// Arrange
			var board = await CreateTestBoard();
			var column = await _columnService.CreateColumnAsync(board.Id, "Column", null);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentException>(
				() => _columnService.UpdateColumnAsync(board.Id, column.Id, "")
			);
		}

		[Fact]
		public async Task UpdateColumnAsync_WithWhitespaceName_ShouldThrowArgumentException()
		{
			// Arrange
			var board = await CreateTestBoard();
			var column = await _columnService.CreateColumnAsync(board.Id, "Column", null);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentException>(
				() => _columnService.UpdateColumnAsync(board.Id, column.Id, "   ")
			);
		}

		[Fact]
		public async Task UpdateColumnAsync_WithNonExistentColumn_ShouldThrowNotFoundException()
		{
			// Arrange
			var board = await CreateTestBoard();
			var nonExistentColumnId = Guid.NewGuid();

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(
				() => _columnService.UpdateColumnAsync(board.Id, nonExistentColumnId, "New Name")
			);
		}

		[Fact]
		public async Task UpdateColumnAsync_WithWrongBoardId_ShouldThrowNotFoundException()
		{
			// Arrange
			var board1 = await CreateTestBoard();
			var board2 = await CreateTestBoard();
			var column = await _columnService.CreateColumnAsync(board1.Id, "Column", null);

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(
				() => _columnService.UpdateColumnAsync(board2.Id, column.Id, "New Name")
			);
		}

		#endregion

		#region DeleteColumnAsync Tests

		[Fact]
		public async Task DeleteColumnAsync_WithEmptyColumn_ShouldDeleteSuccessfully()
		{
			// Arrange
			var board = await CreateTestBoard();
			var column = await _columnService.CreateColumnAsync(board.Id, "Column to Delete", null);

			// Act
			await _columnService.DeleteColumnAsync(board.Id, column.Id);

			// Assert
			var deletedColumn = await _context.Columns.FindAsync(column.Id);
			Assert.Null(deletedColumn);
		}

		[Fact]
		public async Task DeleteColumnAsync_ShouldShiftOrderOfRemainingColumns()
		{
			// Arrange
			var board = await CreateTestBoard();
			var col1 = await _columnService.CreateColumnAsync(board.Id, "Column 1", null);
			var col2 = await _columnService.CreateColumnAsync(board.Id, "Column 2", null);
			var col3 = await _columnService.CreateColumnAsync(board.Id, "Column 3", null);

			// Act
			await _columnService.DeleteColumnAsync(board.Id, col2.Id);

			// Assert
			var remainingColumns = await _context.Columns
				.Where(c => c.BoardId == board.Id)
				.OrderBy(c => c.Order)
				.ToListAsync();

			Assert.Equal(2, remainingColumns.Count);
			Assert.Equal(0, remainingColumns[0].Order);
			Assert.Equal(1, remainingColumns[1].Order);
		}

		[Fact]
		public async Task DeleteColumnAsync_WithCards_ShouldThrowInvalidOperationException()
		{
			// Arrange
			var board = await CreateTestBoard();
			var column = await _columnService.CreateColumnAsync(board.Id, "Column with Card", null);

			var card = new Card
			{
				Id = Guid.NewGuid(),
				Title = "Test Card",
				Description = "Test Description",
				Order = 0,
				ColumnId = column.Id,
				CreatedOn = DateTime.UtcNow,
				ModifiedOn = DateTime.UtcNow
			};

			_context.Cards.Add(card);
			await _context.SaveChangesAsync();

			// Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _columnService.DeleteColumnAsync(board.Id, column.Id)
			);

			var columnStillExists = await _context.Columns.FindAsync(column.Id);
			Assert.NotNull(columnStillExists);
		}

		[Fact]
		public async Task DeleteColumnAsync_WithNonExistentColumn_ShouldThrowNotFoundException()
		{
			// Arrange
			var board = await CreateTestBoard();
			var nonExistentColumnId = Guid.NewGuid();

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(
				() => _columnService.DeleteColumnAsync(board.Id, nonExistentColumnId)
			);
		}

		[Fact]
		public async Task DeleteColumnAsync_WithWrongBoardId_ShouldThrowNotFoundException()
		{
			// Arrange
			var board1 = await CreateTestBoard();
			var board2 = await CreateTestBoard();
			var column = await _columnService.CreateColumnAsync(board1.Id, "Column", null);

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(
				() => _columnService.DeleteColumnAsync(board2.Id, column.Id)
			);
		}

		[Fact]
		public async Task DeleteColumnAsync_FirstColumn_ShouldShiftOthersCorrectly()
		{
			// Arrange
			var board = await CreateTestBoard();
			var col1 = await _columnService.CreateColumnAsync(board.Id, "First", null);
			var col2 = await _columnService.CreateColumnAsync(board.Id, "Second", null);
			var col3 = await _columnService.CreateColumnAsync(board.Id, "Third", null);

			// Act
			await _columnService.DeleteColumnAsync(board.Id, col1.Id);

			// Assert
			var remaining = await _context.Columns
				.Where(c => c.BoardId == board.Id)
				.OrderBy(c => c.Order)
				.ToListAsync();

			Assert.Equal(2, remaining.Count);
			Assert.Equal(0, remaining[0].Order);
			Assert.Equal(1, remaining[1].Order);
		}

		[Fact]
		public async Task DeleteColumnAsync_LastColumn_ShouldNotAffectOthers()
		{
			// Arrange
			var board = await CreateTestBoard();
			var col1 = await _columnService.CreateColumnAsync(board.Id, "First", null);
			var col2 = await _columnService.CreateColumnAsync(board.Id, "Second", null);
			var col3 = await _columnService.CreateColumnAsync(board.Id, "Third", null);

			// Act
			await _columnService.DeleteColumnAsync(board.Id, col3.Id);

			// Assert
			var remaining = await _context.Columns
				.Where(c => c.BoardId == board.Id)
				.OrderBy(c => c.Order)
				.ToListAsync();

			Assert.Equal(2, remaining.Count);
			Assert.Equal(0, remaining[0].Order);
			Assert.Equal(1, remaining[1].Order);
		}

		#endregion

		#region Helper Methods

		private async Task<Board> CreateTestBoard()
		{
			var board = new Board
			{
				Id = Guid.NewGuid(),
				Name = $"Test Board {Guid.NewGuid()}",
				CreatedOn = DateTime.UtcNow,
				ModifiedOn = DateTime.UtcNow
			};

			_context.Boards.Add(board);
			await _context.SaveChangesAsync();

			return board;
		}

		#endregion
	}
}
