namespace KanbanAPI.DTOs
{
	public record CreateBoardRequest(string BoardName);
	public record CreateBoardResponse(Guid Id, string BoardName);
	public record UserBoardResponse(Guid Id, string Name);
	public record UpdateBoardRequest(string Name);
	public record UpdateBoardResponse(Guid Id, string Name);
	public record AddBoardMemberRequest(string UserId);
	public record CreateColumnRequest(string Name, int? Position);
	public record UpdateColumnRequest(string Name);
	public record CreateColumnResponse(Guid Id, string Name, int Order, Guid BoardId);
	public record UpdateColumnResponse(Guid Id, string Name, int Order, Guid BoardId);
	public record CreateCardRequest(string Title, string? Description, Guid ColumnId);
	public record UpdateCardRequest(string Title, string? Description, Guid ColumnId);
	public record CreateCardResponse(Guid Id, string Title, string Description, Guid ColumnId, int Order);
	public record UpdateCardResponse(Guid Id, string Title, string Description, Guid ColumnId, int Order);
	public record UserResponse(string Id, string? UserName, string? Email);
	public record CardResponse(Guid Id, string Title, string Description, int Order, string? AssignedToUserId, UserResponse? AssignedToUser);
	public record ColumnResponse(Guid Id, string Name, int Order, IEnumerable<CardResponse> Cards);
	public record BoardDetailResponse(Guid Id, string Name, IEnumerable<ColumnResponse> Columns);
	public record AssignCardRequest(string UserId);
	public record AssignCardResponse(Guid Id, string Title, string Description, Guid ColumnId, int Order, string? AssignedToUserId, UserResponse? AssignedToUser);
}
