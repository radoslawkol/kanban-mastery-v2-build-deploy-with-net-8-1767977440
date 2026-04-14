using KanbanAPI.Models;

namespace KanbanAPI.Services
{
	public interface IUserService
	{
		Task<ApplicationUser?> GetUserProfileAsync(string userId);
		Task<ApplicationUser?> GetUserByEmailAsync(string email);
	}
}
