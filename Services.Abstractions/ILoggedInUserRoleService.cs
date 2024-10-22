using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface ILoggedInUserRoleService
    {
        T GetServiceForController<T>(LoggedInUser user, Dictionary<String, T> Services);
    }
}
