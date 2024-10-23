using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Domain.Entities.Users
{
    public class User : BaseEntity
    {
        [Column("id")]
        public new Guid Id { get; set; }
        [Column("userid")]
        public string UserId { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password")]
        public string? Password { get; set; }
        [Column("role")]
        public string Role { get; set; }
        [Column("usergroup")]
        public string? UserGroup { get; set; }
        [Column("lastloggedon")]
        public DateTime? LastLoggedOn { get; set; }
        [Column("status")]
        public string Status { get; set; } = "INACTIVE";

        [Column("firstname")]
        public string FirstName { get; set; }
        [Column("lastname")]
        public string LastName { get; set; }
        [Column("middlename")]
        public string? MiddleName { get; set; }
        [Column("secondlastname")]
        public string? SecondLastName { get; set; }
        // Todo: What is the full name logic?
        [Column("fullname")]
        public string? FullName { get; set; }

        [Column("additionaldata", TypeName = "jsonb")]
        public string? AdditionalData { get; set; }
    }

    public static class QueryableExtensions
    {
        public static IEnumerable<User> Filter<User>(this IEnumerable<User> users, Dictionary<string, string> userSearchFilterParams)
        {
            foreach (var prop in userSearchFilterParams.Keys)
            {
                PropertyInfo? propertyInfo = typeof(User).GetProperty(prop);
                string? propValue;
                userSearchFilterParams.TryGetValue(prop, out propValue);

                if(propertyInfo != null && !string.IsNullOrEmpty(propValue)) {
                    users = users.Where(e => propertyInfo.GetValue(e) != null && Convert.ToString(propertyInfo.GetValue(e) as object).Contains(propValue, StringComparison.OrdinalIgnoreCase));
                }
            }
            return users;
        }
    }
}