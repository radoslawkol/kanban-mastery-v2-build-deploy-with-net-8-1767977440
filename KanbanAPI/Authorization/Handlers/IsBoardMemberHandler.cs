using System.Security.Claims;
using KanbanAPI.Authorization.Requirements;
using KanbanAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace KanbanAPI.Authorization.Handlers
{
	public class IsBoardMemberHandler : AuthorizationHandler<IsBoardMemberRequirement, Guid>
	{
		private readonly ApplicationDbContext _db;

		public IsBoardMemberHandler(ApplicationDbContext db)
		{
			_db = db;
		}

		protected override async Task HandleRequirementAsync(
			AuthorizationHandlerContext context,
			IsBoardMemberRequirement requirement,
			Guid boardId)
		{
			var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

			var isMember = await _db.BoardMembers
				.AnyAsync(m => m.BoardId == boardId && m.UserId == userId);

			if (isMember)
				context.Succeed(requirement);
		}
	}
}
