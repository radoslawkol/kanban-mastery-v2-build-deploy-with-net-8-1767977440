using Microsoft.AspNetCore.Identity;

namespace KanbanAPI.Models
{
	public class ApplicationUser : IdentityUser
	{
		public ICollection<BoardMember> Boards { get; set; } = new List<BoardMember>();
	}
}
