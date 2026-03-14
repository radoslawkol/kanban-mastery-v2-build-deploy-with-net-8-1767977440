using KanbanAPI.Models;

namespace KanbanAPI.Services
{
	public interface IColumnService
	{
		Task<Column> CreateColumnAsync(Guid boardId, string name, int? position);
		Task<Column> UpdateColumnAsync(Guid boardId, Guid columnId, string name);
		Task DeleteColumnAsync(Guid boardId, Guid columnId);
	}
}
