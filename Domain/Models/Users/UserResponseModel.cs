using Domain.Entities;

namespace Domain.Models.Users
{
    public class UserResponseModel
	{
        

        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? UserGroup { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? SecondLastName { get; set; }
        public string? FullName { get; set; }

        public string Status { get; set; }
        public List<PayGroupAssignmentModel> PaygroupsAssigned { get; set; }
        public Permissions permissions { get; set; }
        public DateTime? LastLoggedOn { get; set; }
    }
}

