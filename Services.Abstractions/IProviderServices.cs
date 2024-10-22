using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IProviderServices : IUIPermissions
    {
        public List<ProviderModel> GetProviderDetails(LoggedInUser user);
        DatabaseResponse InsertProviderDetails(LoggedInUser user, ProviderModel providerModel);
        DatabaseResponse UpdateProviderDetails(LoggedInUser user, ProviderModel providerModel);
        //void DeleteProvider(LoggedInUser user, int id);
        DatabaseResponse DeleteProvider(LoggedInUser user, int id);
    }
}
