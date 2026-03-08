namespace KanbanAPI.DTOs
{
	public record CreateBoardRequest(string BoardName);
	public record CreateBoardResponse(Guid Id, string BoardName);
	public record AddBoardMemberRequest(string UserId);
	public record CardResponse(Guid Id, string Title, string Description, int Order);
	public record ColumnResponse(Guid Id, string Name, int Order, IEnumerable<CardResponse> Cards);
	public record BoardDetailResponse(Guid Id, string Name, IEnumerable<ColumnResponse> Columns);
}
