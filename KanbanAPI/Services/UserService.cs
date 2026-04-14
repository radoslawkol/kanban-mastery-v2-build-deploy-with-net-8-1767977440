using KanbanAPI.Data;
using KanbanAPI.Exceptions;
using KanbanAPI.Models;
using Microsoft.EntityFrameworkCore;

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

		public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				throw new ArgumentException("Email cannot be empty or whitespace.", nameof(email));
			}

			var normalizedEmail = email.Trim().ToUpperInvariant();
			return await _context.Users.FirstOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail);
		}
	}
}