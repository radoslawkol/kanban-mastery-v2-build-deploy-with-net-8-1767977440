namespace KanbanAPI.Models
{
	public class BoardMember
	{
		public Guid BoardId { get; set; }
		public Board Board { get; set; } = null!;
		public string UserId { get; set; } = null!;
		public ApplicationUser User { get; set; } = null!;
		public BoardRole Role { get; set; }
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
		public DateTime ModifiedOn {  get; set; } = DateTime.UtcNow;
	}
}
