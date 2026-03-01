using KanbanAPI.Data;
using KanbanAPI.DTOs;
using KanbanAPI.Models;
using KanbanAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

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
	var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

	if (string.IsNullOrEmpty(userId))
		return Results.Unauthorized();

	var board = await boardService.CreateBoardAsync(createBoardRequest.BoardName, userId);

	return TypedResults.Ok(board);
}).RequireAuthorization();

app.Run();

public partial class Program { }