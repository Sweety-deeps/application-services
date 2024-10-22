using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IDashboarServices : IUIPermissions
    {
        void GetRequestDetails(LoggedInUser user, int iRequestType, DateTime dtFrom, DateTime dtTo);
    }
}
