using System.Security.Claims;
using KanbanAPI.Exceptions;
using KanbanAPI.DTOs;
using KanbanAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace KanbanAPI.Endpoints
{
	public static class ColumnEndpoints
	{
		public static IEndpointRouteBuilder MapColumnEndpoints(this IEndpointRouteBuilder app)
		{
			var columns = app.MapGroup("/api/boards/{boardId}/columns")
				.RequireAuthorization();

			columns.MapPost("/", CreateColumn);
			columns.MapPut("/{columnId}", UpdateColumn);
			columns.MapDelete("/{columnId}", DeleteColumn);

			return app;
		}

		private static async Task<IResult> CreateColumn(
			Guid boardId,
			CreateColumnRequest request,
			HttpContext httpContext,
			IAuthorizationService authorizationService,
			IColumnService columnService)
		{
			try
			{
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var authorizationResult = await authorizationService.AuthorizeAsync(httpContext.User, boardId, "IsBoardMember");
				if (!authorizationResult.Succeeded)
					return Results.Forbid();

				var column = await columnService.CreateColumnAsync(boardId, request.Name, request.Position);
				var response = new CreateColumnResponse(column.Id, column.Name, column.Order, column.BoardId);

				return Results.Created(
					$"/api/boards/{boardId}/columns/{column.Id}",
					response);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(new { message = ex.Message });
			}
			catch (NotFoundException ex)
			{
				return Results.NotFound(new { message = ex.Message });
			}
		}

		private static async Task<IResult> UpdateColumn(
			Guid boardId,
			Guid columnId,
			UpdateColumnRequest request,
			HttpContext httpContext,
			IAuthorizationService authorizationService,
			IColumnService columnService)
		{
			try
			{
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var authorizationResult = await authorizationService.AuthorizeAsync(httpContext.User, boardId, "IsBoardMember");
				if (!authorizationResult.Succeeded)
					return Results.Forbid();

				var column = await columnService.UpdateColumnAsync(boardId, columnId, request.Name);
				var response = new CreateColumnResponse(column.Id, column.Name, column.Order, column.BoardId);

				return Results.Ok(response);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(new { message = ex.Message });
			}
			catch (NotFoundException ex)
			{
				return Results.NotFound(new { message = ex.Message });
			}
		}

		private static async Task<IResult> DeleteColumn(
			Guid boardId,
			Guid columnId,
			HttpContext httpContext,
			IAuthorizationService authorizationService,
			IColumnService columnService)
		{
			try
			{
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var authorizationResult = await authorizationService.AuthorizeAsync(httpContext.User, boardId, "IsBoardMember");
				if (!authorizationResult.Succeeded)
					return Results.Forbid();

				await columnService.DeleteColumnAsync(boardId, columnId);
				return Results.NoContent();
			}
			catch (NotFoundException ex)
			{
				return Results.NotFound(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return Results.BadRequest(new { message = ex.Message });
			}
		}
	}
}
