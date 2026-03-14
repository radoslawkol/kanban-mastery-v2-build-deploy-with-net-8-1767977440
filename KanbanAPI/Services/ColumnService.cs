using KanbanAPI.Data;
using KanbanAPI.Exceptions;
using KanbanAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAPI.Services
{
	public class ColumnService : IColumnService
	{
		private readonly ApplicationDbContext _context;

		public ColumnService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Column> CreateColumnAsync(Guid boardId, string name, int? position)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Column name cannot be empty.", nameof(name));

			var boardExists = await _context.Boards.AnyAsync(b => b.Id == boardId);
			if (!boardExists)
				throw new NotFoundException("Board not found.");

			var columns = await _context.Columns
				.Where(c => c.BoardId == boardId)
				.OrderBy(c => c.Order)
				.ToListAsync();

			var targetPosition = position ?? columns.Count;
			if (targetPosition < 0)
				targetPosition = 0;
			if (targetPosition > columns.Count)
				targetPosition = columns.Count;

			foreach (var existingColumn in columns.Where(c => c.Order >= targetPosition))
			{
				existingColumn.Order++;
			}

			var column = new Column
			{
				Id = Guid.NewGuid(),
				Name = name,
				BoardId = boardId,
				Order = targetPosition,
			};

			_context.Columns.Add(column);
			await _context.SaveChangesAsync();

			return column;
		}

		public async Task<Column> UpdateColumnAsync(Guid boardId, Guid columnId, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Column name cannot be empty.", nameof(name));

			var column = await _context.Columns.FirstOrDefaultAsync(c => c.Id == columnId && c.BoardId == boardId);
			if (column is null)
				throw new NotFoundException("Column not found.");

			column.Name = name;
			column.ModifiedOn = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return column;
		}

		public async Task DeleteColumnAsync(Guid boardId, Guid columnId)
		{
			var column = await _context.Columns
				.Include(c => c.Cards)
				.FirstOrDefaultAsync(c => c.Id == columnId && c.BoardId == boardId);

			if (column is null)
				throw new NotFoundException("Column not found.");

			if (column.Cards.Any())
				throw new InvalidOperationException("Cannot delete column with existing cards.");

			var deletedOrder = column.Order;
			_context.Columns.Remove(column);

			var columnsToShift = await _context.Columns
				.Where(c => c.BoardId == boardId && c.Order > deletedOrder)
				.ToListAsync();

			foreach (var existingColumn in columnsToShift)
			{
				existingColumn.Order--;
			}

			await _context.SaveChangesAsync();
		}
	}
}
