using KanbanAPI.Data;
using KanbanAPI.Exceptions;
using KanbanAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAPI.Services
{
	public class BoardService : IBoardService
	{
		private readonly ApplicationDbContext _context;

		public BoardService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Board?> GetByIdAsync(Guid id, string currentUserId)
		{
			var board = await _context.Boards
				.Where(b => b.Id == id)
				.Include(b => b.Members)
				.Include(b => b.Columns)
				.FirstOrDefaultAsync();

			if (board is null)
				throw new NotFoundException("Board not found.");

			var isMember = await IsUserBoardMemberAsync(id, currentUserId);
			if (!isMember)
				throw new UnauthorizedException("You are not a member of this board.");

			return board;
		}

		public async Task<IEnumerable<Board>> GetUserBoardsAsync(string userId, string currentUserId)
		{
			if (userId != currentUserId)
				throw new UnauthorizedException("You can only view your own boards.");

			return await _context.BoardMembers
				.Where(bm => bm.UserId == userId)
				.Include(bm => bm.Board)
					.ThenInclude(b => b.Columns)
				.Include(bm => bm.Board)
					.ThenInclude(b => b.Members)
				.Select(bm => bm.Board)
				.ToListAsync();
		}

		public async Task<Board> CreateAsync(string name, string currentUserId)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Board name cannot be empty or whitespace.", nameof(name));

			var board = new Board
			{
				Id = Guid.NewGuid(),
				Name = name,
			};

			_context.Boards.Add(board);

			var boardMember = new BoardMember
			{
				BoardId = board.Id,
				UserId = currentUserId,
				Role = BoardRole.Owner,
			};

			_context.BoardMembers.Add(boardMember);
			await _context.SaveChangesAsync();

			return board;
		}

		public async Task<Board?> UpdateAsync(Guid id, string name, string currentUserId)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Board name cannot be empty or whitespace.", nameof(name));

			var isOwner = await IsUserBoardOwnerAsync(id, currentUserId);
			if (!isOwner)
				throw new UnauthorizedException("Only board owners can update the board.");

			var board = await _context.Boards.FindAsync(id);
			if (board is null)
				throw new NotFoundException("Board not found.");

			board.Name = name;
			board.ModifiedOn = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			return board;
		}

		public async Task<bool> DeleteAsync(Guid id, string currentUserId)
		{
			var isOwner = await IsUserBoardOwnerAsync(id, currentUserId);
			if (!isOwner)
				throw new UnauthorizedException("Only board owners can delete the board.");

			var board = await _context.Boards.FindAsync(id);
			if (board is null)
				throw new NotFoundException("Board not found.");

			_context.Boards.Remove(board);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<bool> IsUserBoardMemberAsync(Guid boardId, string userId)
		{
			return await _context.BoardMembers
				.AnyAsync(bm => bm.BoardId == boardId && bm.UserId == userId);
		}

		public async Task<bool> IsUserBoardOwnerAsync(Guid boardId, string userId)
		{
			return await _context.BoardMembers
				.AnyAsync(bm => bm.BoardId == boardId && bm.UserId == userId && bm.Role == BoardRole.Owner);
		}

		public async Task<IEnumerable<BoardMember>> GetBoardMembersAsync(Guid boardId, string currentUserId)
		{
			var isMember = await IsUserBoardMemberAsync(boardId, currentUserId);
			if (!isMember)
				throw new UnauthorizedException("You are not a member of this board.");

			return await _context.BoardMembers
				.Where(bm => bm.BoardId == boardId)
				.Include(bm => bm.User)
				.ToListAsync();
		}
	}
}
