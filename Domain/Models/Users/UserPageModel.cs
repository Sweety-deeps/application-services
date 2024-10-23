using Domain.Entities;

namespace Domain.Models.Users
{
    public class UserPageModel
    {
        public List<UserResponseModel> data { get; set; }
        public Permissions permissions { get; set; }
        public int totalPages { get; set; }
        public int totalRecords { get; set; }
    }
}

