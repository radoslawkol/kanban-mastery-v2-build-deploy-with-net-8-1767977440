using KanbanAPI.DTOs;
using KanbanAPI.Exceptions;
using KanbanAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace KanbanAPI.Endpoints
{
	public static class BoardEndpoints
	{
		public static IEndpointRouteBuilder MapBoardEndpoints(this IEndpointRouteBuilder app)
		{
			var boards = app.MapGroup("/api/boards")
				.RequireAuthorization();

			boards.MapGet("/", GetCurrentUserBoards);
			boards.MapPost("/", CreateBoard);
			boards.MapPut("/{boardId}", UpdateBoard);
			boards.MapDelete("/{boardId}", DeleteBoard);

			return app;
		}

		private static async Task<IResult> GetCurrentUserBoards(
			HttpContext httpContext,
			IBoardService boardService)
		{
			try
			{
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var boards = await boardService.GetUserBoardsAsync(userId, userId);
				var response = boards
					.Select(board => new UserBoardResponse(board.Id, board.Name))
					.ToList();

				return TypedResults.Ok(response);
			}
			catch (ForbiddenException ex)
			{
				return Results.Forbid();
			}
		}

		private static async Task<IResult> CreateBoard(
			CreateBoardRequest createBoardRequest,
			HttpContext httpContext,
			IBoardService boardService)
		{
			try
			{
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var board = await boardService.CreateBoardAsync(createBoardRequest.BoardName, userId);

				var createBoardResponse = new CreateBoardResponse(board.Id, board.Name);

				return TypedResults.Created($"/api/boards/{board.Id}", createBoardResponse);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(new { message = ex.Message });
			}
		}

		private static async Task<IResult> UpdateBoard(
			Guid boardId,
			UpdateBoardRequest updateBoardRequest,
			HttpContext httpContext,
			IBoardService boardService,
			IAuthorizationService authorizationService)
		{
			try
			{
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var authorizationResult = await authorizationService.AuthorizeAsync(httpContext.User, boardId, "IsBoardOwner");
				if (!authorizationResult.Succeeded)
					return Results.Forbid();

				var board = await boardService.UpdateAsync(boardId, updateBoardRequest.Name, userId);

				if (board is null)
					return Results.NotFound(new { message = "Board not found" });

				var updateBoardResponse = new UpdateBoardResponse(board.Id, board.Name);
				return TypedResults.Ok(updateBoardResponse);
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

		private static async Task<IResult> DeleteBoard(
			Guid boardId,
			HttpContext httpContext,
			IBoardService boardService,
			IAuthorizationService authorizationService)
		{
			try
			{
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var authorizationResult = await authorizationService.AuthorizeAsync(httpContext.User, boardId, "IsBoardOwner");
				if (!authorizationResult.Succeeded)
					return Results.Forbid();

				var deleted = await boardService.DeleteAsync(boardId, userId);

				if (!deleted)
					return Results.NotFound(new { message = "Board not found" });

				return TypedResults.NoContent();
			}
			catch (NotFoundException ex)
			{
				return Results.NotFound(new { message = ex.Message });
			}
		}
	}
}
