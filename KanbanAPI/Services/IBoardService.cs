using KanbanAPI.Models;

namespace KanbanAPI.Services
{
	public interface IBoardService
	{
		Task<Board?> GetByIdAsync(Guid id, string currentUserId);
		Task<IEnumerable<Board>> GetUserBoardsAsync(string userId, string currentUserId);
		Task<Board> CreateBoardAsync(string name, string currentUserId);
		Task<Board?> UpdateAsync(Guid id, string name, string currentUserId);
		Task<bool> DeleteAsync(Guid id, string currentUserId);
		Task<bool> IsUserBoardMemberAsync(Guid boardId, string userId);
		Task<bool> IsUserBoardOwnerAsync(Guid boardId, string userId);
		Task<IEnumerable<BoardMember>> GetBoardMembersAsync(Guid boardId, string currentUserId);
	}
}
