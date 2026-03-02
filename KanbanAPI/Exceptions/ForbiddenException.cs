namespace KanbanAPI.Exceptions
{
	public class ForbiddenException : Exception
	{
		public ForbiddenException() : base("User does not have permission to perform this action.")
		{
		}

		public ForbiddenException(string message) : base(message)
		{
		}

		public ForbiddenException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
