using System;
namespace Domain.Models.Users
{
	public class AwsCognito
	{
		public AwsCognito()
		{
		}

		public string Region { get; set; }
		public string UserPoolId { get;set; }
		public string AppClientId { get; set; }
		public string UserPoolClientId { get; set; }
		public string AppClientSecret { get; set; }
		public bool DefaultPassword { get; set; } = false;
	}
}

