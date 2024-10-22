using Domain.Models.Users;
using Services.Abstractions;

namespace Services
{
    public class LoggedInUserRoleService : ILoggedInUserRoleService
    {
        public LoggedInUserRoleService()
        {
        }

        public T GetServiceForController<T>(LoggedInUser user,  Dictionary<string, T> Services)
        {

            switch (user.Role)
            {
                case Role.CAM:
                    return Services["Services.CAM"];
                case Role.country_manager:
                    return Services["Services.CM"];
                case Role.Interface_oa:
                    return Services["Services.IOA"];
                case Role.interface_ic:
                    return Services["Services.IIA"];
                case Role.poweruser:
                    return Services["Services.PowerUser"];
                case Role.superuser:
                    return Services["Services"];
                default:
                    return Services["Services.UnAuthorized"];
            }
        }
    }
}

