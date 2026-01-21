namespace KanbanAPI.Models
{
	public class Column
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Order { get; set; }
		public Guid BoardId { get; set; }
		public Board Board { get; set; } = null!;
		public ICollection<Card> Cards { get; set; } = new List<Card>();
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
		public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
	}
}
