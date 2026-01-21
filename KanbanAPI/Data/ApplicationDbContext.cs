using KanbanAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KanbanAPI.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
			: base(options) 
		{ 
		}

		public DbSet<Board> Boards {  get; set; }
		public DbSet<Column> Columns { get; set; }
		public DbSet<Card> Cards { get; set; }
		public DbSet<BoardMember> BoardMembers { get; set; }
	}
}
