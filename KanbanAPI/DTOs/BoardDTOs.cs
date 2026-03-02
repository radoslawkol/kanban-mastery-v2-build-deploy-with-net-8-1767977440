namespace KanbanAPI.DTOs
{
	public record CreateBoardRequest(string BoardName);
	public record CreateBoardResponse(Guid Id, string BoardName);
	public record AddBoardMemberRequest(string UserId);
}
