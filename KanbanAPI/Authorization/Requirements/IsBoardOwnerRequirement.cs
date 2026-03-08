using Microsoft.AspNetCore.Authorization;

namespace KanbanAPI.Authorization.Requirements
{
	public class IsBoardOwnerRequirement : IAuthorizationRequirement
	{
	}
}
