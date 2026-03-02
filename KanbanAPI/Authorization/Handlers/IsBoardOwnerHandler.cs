using System.Security.Claims;
using KanbanAPI.Authorization.Requirements;
using KanbanAPI.Data;
using KanbanAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace KanbanAPI.Authorization.Handlers
{
	public class IsBoardOwnerHandler : AuthorizationHandler<IsBoardOwnerRequirement, Guid>
	{
		private readonly ApplicationDbContext _db;

		public IsBoardOwnerHandler(ApplicationDbContext db)
		{
			_db = db;
		}

		protected override async Task HandleRequirementAsync(
			AuthorizationHandlerContext context,
			IsBoardOwnerRequirement requirement,
			Guid boardId)
		{
			var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
			
			if (string.IsNullOrEmpty(userId))
				return;

			var isOwner = await _db.BoardMembers
				.AnyAsync(m => m.BoardId == boardId && m.UserId == userId && m.Role == BoardRole.Owner);

			if (isOwner)
				context.Succeed(requirement);
		}
	}
}
