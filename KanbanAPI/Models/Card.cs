namespace KanbanAPI.Models
{
	public class Card
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public int Order { get; set; }
		public Guid ColumnId { get; set; }
		public Column Column { get; set; } = null!;
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
		public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
	}
}
