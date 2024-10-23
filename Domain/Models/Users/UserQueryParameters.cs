namespace Domain.Models.Users
{
    public class UserQueryParameters
	{
		public UserQueryParameters()
		{
		}

		public int Limit { get; set; } = 20;
		public int Offset { get; set; } = 0;
	}
}
