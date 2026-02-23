namespace KanbanAPI.Tests.Helpers
{
	public class LoginResponse
	{
		public string AccessToken { get; set; } = string.Empty;
		public string TokenType { get; set; } = string.Empty;
		public int ExpiresIn { get; set; }
	}
}
