using KanbanAPI.Data;
using KanbanAPI.Exceptions;
using KanbanAPI.Models;
using KanbanAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace KanbanAPI.Tests.Services
{
	public class BoardServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _context;
		private readonly BoardService _boardService;
		private readonly string _testUserId = "test-user-id";
		private readonly string _otherUserId = "other-user-id";

		public BoardServiceTests()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new ApplicationDbContext(options);
			_boardService = new BoardService(_context);
			
			_context.Users.Add(new ApplicationUser { Id = _testUserId, UserName = "testuser" });
			_context.Users.Add(new ApplicationUser { Id = _otherUserId, UserName = "otheruser" });
			_context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}

		#region CreateBoardAsync Tests

		[Fact]
		public async Task CreateBoardAsync_ShouldCreateBoard()
		{
			// Arrange
			var boardName = "Test Board";

			// Act
			var result = await _boardService.CreateBoardAsync(boardName, _testUserId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(boardName, result.Name);
			Assert.NotEqual(Guid.Empty, result.Id);

			// Verify board was saved to database
			var savedBoard = await _context.Boards.FindAsync(result.Id);
			Assert.NotNull(savedBoard);
		}

		[Fact]
		public async Task CreateBoardAsync_ShouldAssignCreatorAsOwner()
		{
			// Arrange
			var boardName = "Owner Board";
			var board = await _boardService.CreateBoardAsync(boardName, _testUserId);

			// Act
			var membership = await _context.BoardMembers
				.FirstOrDefaultAsync(bm => bm.BoardId == board.Id && bm.UserId == _testUserId);

			// Assert
			Assert.NotNull(membership);
			Assert.Equal(BoardRole.Owner, membership.Role);
		}

		[Fact]
		public async Task CreateBoardAsync_WithEmptyName_ShouldThrowArgumentException()
		{
			var boardName = "";

			await Assert.ThrowsAsync<ArgumentException>(
				(() => _boardService.CreateBoardAsync(boardName, _testUserId)));
		}

		#endregion

		#region GetByIdAsync Tests

		[Fact]
		public async Task GetByIdAsync_WhenUserIsMember_ShouldReturnBoard()
		{
			// Arrange
			var board = await CreateTestBoardWithMember(_testUserId, BoardRole.Member);

			// Act
			var result = await _boardService.GetByIdAsync(board.Id, _testUserId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(board.Id, result.Id);
			Assert.Equal(board.Name, result.Name);
		}

		[Fact]
		public async Task GetByIdAsync_WhenUserIsNotMember_ShouldThrowUnauthorizedException()
		{
			var board = await CreateTestBoardWithMember(_otherUserId, BoardRole.Owner);

			await Assert.ThrowsAsync<UnauthorizedException>(
				() => _boardService.GetByIdAsync(board.Id, _testUserId)
			);
		}

		[Fact]
		public async Task GetByIdAsync_WhenBoardDoesNotExist_ShouldThrowNotFoundException()
		{
			var nonExistentBoardId = Guid.NewGuid();

			await Assert.ThrowsAsync<NotFoundException>(
				() => _boardService.GetByIdAsync(nonExistentBoardId, _testUserId)
			);
		}

		#endregion

		#region GetUserBoardsAsync Tests

		[Fact]
		public async Task GetUserBoardsAsync_WhenUserIdMatchesCurrentUserId_ShouldReturnBoards()
		{
			// Arrange
			var board1 = await CreateTestBoardWithMember(_testUserId, BoardRole.Owner);
			var board2 = await CreateTestBoardWithMember(_testUserId, BoardRole.Member);
			await CreateTestBoardWithMember(_otherUserId, BoardRole.Owner); // Should not be included

			// Act
			var result = await _boardService.GetUserBoardsAsync(_testUserId, _testUserId);

			// Assert
			var boards = result.ToList();
			Assert.Equal(2, boards.Count);
			Assert.Contains(boards, b => b.Id == board1.Id);
			Assert.Contains(boards, b => b.Id == board2.Id);
		}

		[Fact]
		public async Task GetUserBoardsAsync_WhenUserIdDoesNotMatchCurrentUserId_ShouldThrowUnauthorizedException()
		{
			await CreateTestBoardWithMember(_otherUserId, BoardRole.Owner);

			await Assert.ThrowsAsync<UnauthorizedException>(
				() => _boardService.GetUserBoardsAsync(_otherUserId, _testUserId)
			);
		}

		#endregion

		#region UpdateAsync Tests

		[Fact]
		public async Task UpdateAsync_WhenUserIsOwner_ShouldUpdateBoard()
		{
			// Arrange
			var board = await CreateTestBoardWithMember(_testUserId, BoardRole.Owner);
			var newName = "Updated Board Name";

			// Act
			var result = await _boardService.UpdateAsync(board.Id, newName, _testUserId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(newName, result.Name);

			// Verify database was updated
			var updatedBoard = await _context.Boards.FindAsync(board.Id);
			Assert.Equal(newName, updatedBoard!.Name);
		}

		[Fact]
		public async Task UpdateAsync_WhenUserIsMemberButNotOwner_ShouldThrowUnauthorizedException()
		{
			var board = await CreateTestBoardWithMember(_testUserId, BoardRole.Member);
			var newName = "Updated Board Name";

			await Assert.ThrowsAsync<UnauthorizedException>(
				() => _boardService.UpdateAsync(board.Id, newName, _testUserId)
			);
		}

		[Fact]
		public async Task UpdateAsync_WhenUserIsNotMember_ShouldThrowUnauthorizedException()
		{
			var board = await CreateTestBoardWithMember(_otherUserId, BoardRole.Owner);
			var newName = "Updated Board Name";

			await Assert.ThrowsAsync<UnauthorizedException>(
				() => _boardService.UpdateAsync(board.Id, newName, _testUserId)
			);
		}

		[Fact]
		public async Task UpdateAsync_ShouldUpdateModifiedOnDate()
		{
			// Arrange
			var board = await CreateTestBoardWithMember(_testUserId, BoardRole.Owner);
			var originalModifiedOn = board.ModifiedOn;
			await Task.Delay(100); // Ensure time difference

			// Act
			var result = await _boardService.UpdateAsync(board.Id, "New Name", _testUserId);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.ModifiedOn > originalModifiedOn);
		}

		#endregion

		#region DeleteAsync Tests

		[Fact]
		public async Task DeleteAsync_WhenUserIsOwner_ShouldDeleteBoard()
		{
			// Arrange
			var board = await CreateTestBoardWithMember(_testUserId, BoardRole.Owner);

			// Act
			var result = await _boardService.DeleteAsync(board.Id, _testUserId);

			// Assert
			Assert.True(result);

			// Verify board was deleted from database
			var deletedBoard = await _context.Boards.FindAsync(board.Id);
			Assert.Null(deletedBoard);
		}

		[Fact]
		public async Task DeleteAsync_WhenUserIsMemberButNotOwner_ShouldThrowUnauthorizedException()
		{
			var board = await CreateTestBoardWithMember(_testUserId, BoardRole.Member);

			await Assert.ThrowsAsync<UnauthorizedException>(
				() => _boardService.DeleteAsync(board.Id, _testUserId)
			);
		}

		[Fact]
		public async Task DeleteAsync_WhenUserIsNotMember_ShouldThrowUnauthorizedException()
		{
			var board = await CreateTestBoardWithMember(_otherUserId, BoardRole.Owner);

			await Assert.ThrowsAsync<UnauthorizedException>(
				() => _boardService.DeleteAsync(board.Id, _testUserId)
			);
		}

		#endregion

		#region IsUserBoardMemberAsync Tests

		[Fact]
		public async Task IsUserBoardMemberAsync_WhenUserIsMember_ShouldReturnTrue()
		{
			// Arrange
			var board = await CreateTestBoardWithMember(_testUserId, BoardRole.Member);

			// Act
			var result = await _boardService.IsUserBoardMemberAsync(board.Id, _testUserId);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task IsUserBoardMemberAsync_WhenUserIsNotMember_ShouldReturnFalse()
		{
			// Arrange
			var board = await CreateTestBoardWithMember(_otherUserId, BoardRole.Owner);

			// Act
			var result = await _boardService.IsUserBoardMemberAsync(board.Id, _testUserId);

			// Assert
			Assert.False(result);
		}

		#endregion

		#region IsUserBoardOwnerAsync Tests

		[Fact]
		public async Task IsUserBoardOwnerAsync_WhenUserIsOwner_ShouldReturnTrue()
		{
			// Arrange
			var board = await CreateTestBoardWithMember(_testUserId, BoardRole.Owner);

			// Act
			var result = await _boardService.IsUserBoardOwnerAsync(board.Id, _testUserId);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task IsUserBoardOwnerAsync_WhenUserIsMemberButNotOwner_ShouldReturnFalse()
		{
			// Arrange
			var board = await CreateTestBoardWithMember(_testUserId, BoardRole.Member);

			// Act
			var result = await _boardService.IsUserBoardOwnerAsync(board.Id, _testUserId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task IsUserBoardOwnerAsync_WhenUserIsNotMember_ShouldReturnFalse()
		{
			// Arrange
			var board = await CreateTestBoardWithMember(_otherUserId, BoardRole.Owner);

			// Act
			var result = await _boardService.IsUserBoardOwnerAsync(board.Id, _testUserId);

			// Assert
			Assert.False(result);
		}

		#endregion

		#region GetBoardMembersAsync Tests

		[Fact]
		public async Task GetBoardMembersAsync_WhenUserIsMember_ShouldReturnMembers()
		{
			// Arrange
			var board = await CreateTestBoardWithMember(_testUserId, BoardRole.Owner);
			await AddMemberToBoard(board.Id, _otherUserId, BoardRole.Member);

			// Act
			var result = await _boardService.GetBoardMembersAsync(board.Id, _testUserId);

			// Assert
			var members = result.ToList();
			Assert.Equal(2, members.Count);
			Assert.Contains(members, m => m.UserId == _testUserId);
			Assert.Contains(members, m => m.UserId == _otherUserId);
		}

		[Fact]
		public async Task GetBoardMembersAsync_WhenUserIsNotMember_ShouldThrowUnauthorizedException()
		{
			var board = await CreateTestBoardWithMember(_otherUserId, BoardRole.Owner);

			await Assert.ThrowsAsync<UnauthorizedException>(
				() => _boardService.GetBoardMembersAsync(board.Id, _testUserId)
			);
		}

		#endregion

		#region Helper Methods

		private async Task<Board> CreateTestBoardWithMember(string userId, BoardRole role)
		{
			var board = new Board
			{
				Id = Guid.NewGuid(),
				Name = $"Test Board {Guid.NewGuid()}",
				CreatedOn = DateTime.UtcNow,
				ModifiedOn = DateTime.UtcNow
			};

			_context.Boards.Add(board);

			var boardMember = new BoardMember
			{
				BoardId = board.Id,
				UserId = userId,
				Role = role,
				CreatedOn = DateTime.UtcNow,
				ModifiedOn = DateTime.UtcNow
			};

			_context.BoardMembers.Add(boardMember);
			await _context.SaveChangesAsync();

			return board;
		}

		private async Task AddMemberToBoard(Guid boardId, string userId, BoardRole role)
		{
			var boardMember = new BoardMember
			{
				BoardId = boardId,
				UserId = userId,
				Role = role,
				CreatedOn = DateTime.UtcNow,
				ModifiedOn = DateTime.UtcNow
			};

			_context.BoardMembers.Add(boardMember);
			await _context.SaveChangesAsync();
		}

		#endregion
	}
}
