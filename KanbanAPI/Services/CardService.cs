using KanbanAPI.Data;
using KanbanAPI.Exceptions;
using KanbanAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAPI.Services
{
	public class CardService : ICardService
	{
		private readonly ApplicationDbContext _context;

		public CardService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Card> CreateCardAsync(Guid boardId, string title, string? description, Guid columnId)
		{
			if (string.IsNullOrWhiteSpace(title))
				throw new ArgumentException("Card title cannot be empty.", nameof(title));

			var column = await _context.Columns
				.FirstOrDefaultAsync(c => c.Id == columnId && c.BoardId == boardId);

			if (column is null)
				throw new NotFoundException("Column not found.");

			var nextOrder = await _context.Cards
				.Where(c => c.ColumnId == columnId)
				.Select(c => (int?)c.Order)
				.MaxAsync() ?? -1;

			var card = new Card
			{
				Id = Guid.NewGuid(),
				Title = title,
				Description = description ?? string.Empty,
				ColumnId = columnId,
				Order = nextOrder + 1,
			};

			_context.Cards.Add(card);
			await _context.SaveChangesAsync();

			return card;
		}

		public async Task<Card> UpdateCardAsync(Guid boardId, Guid cardId, string title, string? description, Guid columnId)
		{
			if (string.IsNullOrWhiteSpace(title))
				throw new ArgumentException("Card title cannot be empty.", nameof(title));

			var card = await _context.Cards
				.Include(c => c.Column)
				.FirstOrDefaultAsync(c => c.Id == cardId && c.Column.BoardId == boardId);

			if (card is null)
				throw new NotFoundException("Card not found.");

			var targetColumnExists = await _context.Columns
				.AnyAsync(c => c.Id == columnId && c.BoardId == boardId);

			if (!targetColumnExists)
				throw new NotFoundException("Column not found.");

			card.Title = title;
			card.Description = description ?? string.Empty;
			card.ColumnId = columnId;
			card.ModifiedOn = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return card;
		}

		public async Task DeleteCardAsync(Guid boardId, Guid cardId)
		{
			var card = await _context.Cards
				.Include(c => c.Column)
				.FirstOrDefaultAsync(c => c.Id == cardId && c.Column.BoardId == boardId);

			if (card is null)
				throw new NotFoundException("Card not found.");

			_context.Cards.Remove(card);
			await _context.SaveChangesAsync();
		}
	}
}
