using KanbanAPI.Models;

namespace KanbanAPI.Services
{
	public interface IBoardService
	{
		Task<Board?> GetByIdAsync(Guid id, string currentUserId);
		Task<IEnumerable<Board>> GetUserBoardsAsync(string userId, string currentUserId);
		Task<Board> CreateAsync(string name, string currentUserId);
		Task<Board?> UpdateAsync(Guid id, string name, string currentUserId);
		Task<bool> DeleteAsync(Guid id, string currentUserId);
		Task<bool> AddMemberAsync(Guid boardId, string userIdToAdd, BoardRole role, string currentUserId);
		Task<bool> RemoveMemberAsync(Guid boardId, string userIdToRemove, string currentUserId);
		Task<bool> UpdateMemberRoleAsync(Guid boardId, string userIdToUpdate, BoardRole newRole, string currentUserId);
		Task<bool> IsUserBoardMemberAsync(Guid boardId, string userId);
		Task<bool> IsUserBoardOwnerAsync(Guid boardId, string userId);
		Task<IEnumerable<BoardMember>> GetBoardMembersAsync(Guid boardId, string currentUserId);
	}
}
