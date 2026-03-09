using KanbanAPI.Data;
using KanbanAPI.Exceptions;
using KanbanAPI.Models;

namespace KanbanAPI.Services
{
	public class UserService : IUserService
	{
		private readonly ApplicationDbContext _context;

		public UserService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ApplicationUser?> GetUserProfileAsync(string userId)
		{
			var appUser = await _context.Users.FindAsync(userId);

			if (appUser is null)
			{
				throw new NotFoundException();
			}

			return appUser;
		}
	}
}