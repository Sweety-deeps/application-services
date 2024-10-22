using Domain.Entities;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IConfigServices: IUIPermissions
    {
        Task<List<RequestDetails>> GetRequestDetailsAsync(LoggedInUser user);
        Task<List<ConfigRequestType>> GetConfigRequestTypeAsync(LoggedInUser user);
    }
}
