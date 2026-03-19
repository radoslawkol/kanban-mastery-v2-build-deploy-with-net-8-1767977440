using KanbanAPI.Models;

namespace KanbanAPI.Services
{
	public interface ICardService
	{
		Task<Card> CreateCardAsync(Guid boardId, string title, string? description, Guid columnId);
		Task<Card> UpdateCardAsync(Guid boardId, Guid cardId, string title, string? description, Guid columnId);
		Task DeleteCardAsync(Guid boardId, Guid cardId);
		Task<Card?> AssignCardAsync(Guid cardId, string userId);
	}
}
