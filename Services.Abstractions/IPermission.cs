using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IUIPermissions
    {
        bool CanView(LoggedInUser user);
        bool CanEdit(LoggedInUser user);
        bool CanAdd(LoggedInUser user);
        bool CanDelete(LoggedInUser user);
    }
}
