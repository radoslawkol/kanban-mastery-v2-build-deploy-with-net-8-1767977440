using KanbanAPI.Authorization.Handlers;
using KanbanAPI.Authorization.Requirements;
using KanbanAPI.Data;
using KanbanAPI.DTOs;
using KanbanAPI.Endpoints;
using KanbanAPI.Exceptions;
using KanbanAPI.Models;
using KanbanAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

const string ClientCorsPolicy = "_clientCorsPolicy";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IAuthorizationHandler, IsBoardOwnerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, IsBoardMemberHandler>();

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("IsBoardOwner", policy =>
	policy.Requirements.Add(new IsBoardOwnerRequirement()));
	options.AddPolicy("IsBoardMember", policy =>
	policy.Requirements.Add(new IsBoardMemberRequirement()));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<IColumnService, ColumnService>();
builder.Services.AddScoped<ICardService, CardService>();

builder.Services.AddCors(options =>
{
	options.AddPolicy(ClientCorsPolicy, policy =>
		policy
			.WithOrigins(
				"http://localhost:5173",
				"https://localhost:5173",
				"http://127.0.0.1:5173",
				"https://127.0.0.1:5173"
			)
			.AllowAnyHeader()
			.AllowAnyMethod());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(ClientCorsPolicy);

app.MapIdentityApi<ApplicationUser>();

app.MapGet("/api/users/me", async (ClaimsPrincipal user, IUserService userService) =>
{
	var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
        return Results.Unauthorized();

    var appUser = await userService.GetUserProfileAsync(userId);

    if (appUser is null)
        return Results.Unauthorized();

	return TypedResults.Ok(new { appUser.Id, appUser.UserName, appUser.Email });
}).RequireAuthorization();

app.MapPost("/api/boards/{boardId}/members", async (
	Guid boardId, AddBoardMemberRequest addBoardMemberRequest, HttpContext httpContext, IBoardService boardService, IAuthorizationService authorizationService) =>
{
	try
	{
		var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(userId))
			return Results.Unauthorized();

		var authorizationResult = await authorizationService.AuthorizeAsync(httpContext.User, boardId, "IsBoardOwner");
		if (!authorizationResult.Succeeded)
			return Results.Forbid();

		await boardService.AddMemberAsync(boardId, addBoardMemberRequest.UserId, BoardRole.Member, userId);

		return Results.Ok(new { message = "Member added successfully" });
	}
	catch (ForbiddenException)
	{
		return Results.Forbid();
	}
	catch (NotFoundException ex)
	{
		return Results.NotFound(new { message = ex.Message });
	}
	catch (InvalidOperationException ex)
	{
		return Results.BadRequest(new { message = ex.Message });
	}
	catch (ArgumentException ex)
	{
		return Results.BadRequest(new { message = ex.Message });
	}
}).RequireAuthorization();

app.MapGet("/api/boards/{boardId}",
	async (Guid boardId, HttpContext httpContext, IBoardService boardService, IAuthorizationService authorizationService) =>
{
	try
	{
		var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(userId))
			return Results.Unauthorized();

		var authorizationResult = await authorizationService.AuthorizeAsync(httpContext.User, boardId, "IsBoardMember");
		if (!authorizationResult.Succeeded)
			return Results.Forbid();

		var board = await boardService.GetByIdAsync(boardId, userId);

		if (board is null)
			return Results.NotFound(new { message = "Board not found" });

		var response = new BoardDetailResponse(
			board.Id,
			board.Name,
			board.Columns
				.OrderBy(c => c.Order)
				.Select(c => new ColumnResponse(
					c.Id,
					c.Name,
					c.Order,
					c.Cards
						.OrderBy(card => card.Order)
						.Select(card => new CardResponse(
							card.Id, 
							card.Title, 
							card.Description, 
							card.Order, 
							card.AssignedToUserId,
							card.AssignedToUser != null 
								? new UserResponse(card.AssignedToUser.Id, card.AssignedToUser.UserName, card.AssignedToUser.Email)
								: null)
						).ToList())
				).ToList());

		return TypedResults.Ok(response);
	}
	catch (ForbiddenException ex)
	{
		return Results.Forbid();
	}
}).RequireAuthorization();

app.MapBoardEndpoints();
app.MapColumnEndpoints();
app.MapCardEndpoints();
app.Run();

public partial class Program { }public partial class Program { }