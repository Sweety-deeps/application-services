using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IPayGroupServices : IUIPermissions
    {
        Task<List<PaygroupMinimalModel>> GetActivePaygroups(LoggedInUser user);
        Task<List<PayGroupModel>> GetPayGroupDetails(LoggedInUser user);
        DatabaseResponse AddPayGroup(LoggedInUser user, PayGroupModel paygroupModel);
        DatabaseResponse UpdatePayGroupDetails(LoggedInUser user, PayGroupModel payGroupModel);
        List<CountryModel> GetCountryDetails(LoggedInUser user);
        List<PayFrequencyModel> GetPayFrequencyDetails(LoggedInUser user);
        DatabaseResponse DeletePayGroup(LoggedInUser user, int id);
        Task<bool> LauchCollectChangesAction(LoggedInUser loggedInUser, PayGroupModel payGroupModel, String eventFunctionArn);
    }
}
