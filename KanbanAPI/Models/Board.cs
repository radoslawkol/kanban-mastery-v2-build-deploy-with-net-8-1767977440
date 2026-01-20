namespace KanbanAPI.Models
{
	public class Board
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public ICollection<Column> Columns { get; set; } = new List<Column>();
		public ICollection<BoardMember> Members { get; set; } = new List<BoardMember>();
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
		public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
	}
}
