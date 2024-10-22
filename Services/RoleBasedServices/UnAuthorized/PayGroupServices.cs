using Domain.Models;
using Domain.Models.Users;
using Services.Abstractions;

namespace Services.UnAuthorized
{
    public class PayGroupServices : IPayGroupServices
    {
        public virtual bool CanView(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public DatabaseResponse AddPayGroup(LoggedInUser user, PayGroupModel paygroupModel)
        {
            throw new UnauthorizedAccessException();
        }

        public DatabaseResponse DeletePayGroup(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<PaygroupMinimalModel>> GetActivePaygroups(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public List<CountryModel> GetCountryDetails(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public List<PayFrequencyModel> GetPayFrequencyDetails(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<PayGroupModel>> GetPayGroupDetails(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public DatabaseResponse UpdatePayGroupDetails(LoggedInUser user, PayGroupModel payGroupModel)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<bool> LauchCollectChangesAction(LoggedInUser loggedInUser, PayGroupModel payGroupModel, String eventFunctionArn)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
