namespace Domain.Models.Users
{
    public class LoggedInUser
	{
		public LoggedInUser(Guid userid, string email, Role role, String userName,IEnumerable<PayGroupCacheModel> assignedPaygroups)
		{
			this.Email = email;
			this.UserId = userid;
			this.Role = role;
			this.Paygroups = assignedPaygroups;
			this.UserName = userName;
		}

		public string Email { get;  }
		public Guid UserId { get;  }
		public string UserName { get;  }
		public Role Role { get;  }
		public IEnumerable<PayGroupCacheModel> Paygroups{ get; }
	}
}

