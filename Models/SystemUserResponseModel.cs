using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SystemUserResponseModel
    {
        public string? Clientname { get; set; }
        public string? Paygroup { get; set; }
        public string Userid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserGroup { get; set; }
        public string UserRole { get; set; }
        public string UserStatus { get; set; }
        public DateTime? UserLastLoginDate { get; set; }

    }
}
