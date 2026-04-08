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

		public async Task<Board?> GetByIdAsync(Guid boardId, string currentUserId)
		{
			var isMember = await IsUserBoardMemberAsync(boardId, currentUserId);
			if (!isMember)
				throw new ForbiddenException("You are not a member of this board.");

			var board = await _context.Boards
				.Include(b => b.Columns)
					.ThenInclude(c => c.Cards)
						.ThenInclude(card => card.AssignedToUser)
				.FirstOrDefaultAsync(b => b.Id == boardId);

			return board;
		}

		public async Task<IEnumerable<Board>> GetUserBoardsAsync(string userId, string currentUserId)
		{
			if (userId != currentUserId)
				throw new ForbiddenException("You can only view your own boards.");

			return await _context.BoardMembers
				.Where(bm => bm.UserId == userId)
				.Include(bm => bm.Board)
					.ThenInclude(b => b.Columns)
				.Include(bm => bm.Board)
					.ThenInclude(b => b.Members)
				.Select(bm => bm.Board)
				.ToListAsync();
		}

		public async Task<Board> CreateBoardAsync(string name, string currentUserId)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Board name cannot be empty or whitespace.", nameof(name));

			if (name.Length > 100)
				throw new ArgumentException("Board name cannot be longer than 100 characters.", nameof(name));

			var board = new Board
			{
				Id = Guid.NewGuid(),
				Name = name,
			};

			var boardMember = new BoardMember
			{
				BoardId = board.Id,
				UserId = currentUserId,
				Role = BoardRole.Owner,
			};

			_context.Boards.Add(board);
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
				throw new ForbiddenException("Only board owners can update the board.");

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
				throw new ForbiddenException("Only board owners can delete the board.");

			var board = await _context.Boards
				.Include(b => b.Columns)
					.ThenInclude(c => c.Cards)
				.Include(b => b.Members)
				.FirstOrDefaultAsync(b => b.Id == id);
			
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
				throw new ForbiddenException("You are not a member of this board.");

			return await _context.BoardMembers
				.Where(bm => bm.BoardId == boardId)
				.Include(bm => bm.User)
				.ToListAsync();
		}
		public async Task<bool> AddMemberAsync(Guid boardId, string userIdToAdd, BoardRole role, string currentUserId)
		{
			var isOwner = await IsUserBoardOwnerAsync(boardId, currentUserId);
			if (!isOwner)
				throw new ForbiddenException("Only board owners can add members.");

			var boardExists = await _context.Boards.AnyAsync(b => b.Id == boardId);
			if (!boardExists)
				throw new NotFoundException("Board not found.");

			var userExists = await _context.Users.AnyAsync(u => u.Id == userIdToAdd);
			if (!userExists)
				throw new NotFoundException("User not found.");

			var existingMember = await _context.BoardMembers
				.AnyAsync(bm => bm.BoardId == boardId && bm.UserId == userIdToAdd);

			if (existingMember)
				throw new InvalidOperationException("User is already a member of this board.");

			var boardMember = new BoardMember
			{
				BoardId = boardId,
				UserId = userIdToAdd,
				Role = role,
			};

			_context.BoardMembers.Add(boardMember);
			await _context.SaveChangesAsync();

			return true;
		}
	}
}
