using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IClientServices: IUIPermissions
    {
        Task<List<ClientModel>> GetClient(LoggedInUser user);
        Task<DatabaseResponse> InsertClient(LoggedInUser user, ClientModel clientModel);
        Task<DatabaseResponse> UpdateClient(LoggedInUser user, ClientModel clientModel);
        Task<DatabaseResponse> DeleteClient(LoggedInUser user, int id);
        Task<List<PaygroupMinimalModel>> GetPayGroupByClient(LoggedInUser user, int clientid);
    }
}
