using KanbanAPI.Authorization.Handlers;
using KanbanAPI.Authorization.Requirements;
using KanbanAPI.Data;
using KanbanAPI.DTOs;
using KanbanAPI.Exceptions;
using KanbanAPI.Models;
using KanbanAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IAuthorizationHandler, IsBoardOwnerHandler>();

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("IsBoardOwner", policy =>
	policy.Requirements.Add(new IsBoardOwnerRequirement()));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBoardService, BoardService>();

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

app.MapPost("/api/boards", async (CreateBoardRequest createBoardRequest, HttpContext httpContext, IBoardService boardService) =>
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
	catch (ForbiddenException ex)
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

app.Run();

public partial class Program { }