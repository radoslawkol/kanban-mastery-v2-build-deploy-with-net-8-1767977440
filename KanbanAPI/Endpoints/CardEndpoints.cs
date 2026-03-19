using System.Security.Claims;
using KanbanAPI.DTOs;
using KanbanAPI.Exceptions;
using KanbanAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace KanbanAPI.Endpoints
{
	public static class CardEndpoints
	{
		public static IEndpointRouteBuilder MapCardEndpoints(this IEndpointRouteBuilder app)
		{
			var cards = app.MapGroup("/api/boards/{boardId}/cards")
				.RequireAuthorization();

			cards.MapPost("/", CreateCard);
			cards.MapPut("/{cardId}", UpdateCard);
			cards.MapDelete("/{cardId}", DeleteCard);
			cards.MapPut("/{cardId}/assign", AssignCard);

			return app;
		}

		private static async Task<IResult> CreateCard(
			Guid boardId,
			CreateCardRequest request,
			HttpContext httpContext,
			IAuthorizationService authorizationService,
			ICardService cardService)
		{
			try
			{
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var authorizationResult = await authorizationService.AuthorizeAsync(httpContext.User, boardId, "IsBoardMember");
				if (!authorizationResult.Succeeded)
					return Results.Forbid();

				var card = await cardService.CreateCardAsync(boardId, request.Title, request.Description, request.ColumnId);
				var response = new CreateCardResponse(card.Id, card.Title, card.Description, card.ColumnId, card.Order);

				return Results.Created($"/api/boards/{boardId}/cards/{card.Id}", response);
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

		private static async Task<IResult> UpdateCard(
			Guid boardId,
			Guid cardId,
			UpdateCardRequest request,
			HttpContext httpContext,
			IAuthorizationService authorizationService,
			ICardService cardService)
		{
			try
			{
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var authorizationResult = await authorizationService.AuthorizeAsync(httpContext.User, boardId, "IsBoardMember");
				if (!authorizationResult.Succeeded)
					return Results.Forbid();

				var card = await cardService.UpdateCardAsync(boardId, cardId, request.Title, request.Description, request.ColumnId);
				var response = new UpdateCardResponse(card.Id, card.Title, card.Description, card.ColumnId, card.Order);

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

		private static async Task<IResult> DeleteCard(
			Guid boardId,
			Guid cardId,
			HttpContext httpContext,
			IAuthorizationService authorizationService,
			ICardService cardService)
		{
			try
			{
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var authorizationResult = await authorizationService.AuthorizeAsync(httpContext.User, boardId, "IsBoardMember");
				if (!authorizationResult.Succeeded)
					return Results.Forbid();

				await cardService.DeleteCardAsync(boardId, cardId);
				return Results.NoContent();
			}
			catch (NotFoundException ex)
			{
				return Results.NotFound(new { message = ex.Message });
			}
		}

		private static async Task<IResult> AssignCard(
			Guid boardId,
			Guid cardId,
			AssignCardRequest dto,
			HttpContext httpContext,
			IAuthorizationService authorizationService,
			ICardService cardService,
			IBoardService boardService)
		{
			var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Results.Unauthorized();

			var authorizationResult = await authorizationService.AuthorizeAsync(httpContext.User, boardId, "IsBoardMember");
			if (!authorizationResult.Succeeded)
				return Results.Forbid();

			var isMember = await boardService.IsUserBoardMemberAsync(boardId, dto.UserId);
			if (!isMember)
				return Results.BadRequest(new { message = "User is not a board member" });

			var card = await cardService.AssignCardAsync(cardId, dto.UserId);
			if (card is null)
				return Results.NotFound();

			var response = new AssignCardResponse(card.Id, card.Title, card.Description, card.ColumnId, card.Order, card.AssignedToUserId);
			return Results.Ok(response);
		}
	}
}
