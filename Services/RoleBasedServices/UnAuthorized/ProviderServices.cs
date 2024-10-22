using Services.Abstractions;
using Domain.Models;
using Domain.Models.Users;

namespace Services.UnAuthorized
{
    public class ProviderServices : IProviderServices
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

        public DatabaseResponse DeleteProvider(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public List<ProviderModel> GetProviderDetails(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public DatabaseResponse InsertProviderDetails(LoggedInUser user, ProviderModel providerModel)
        {
            throw new UnauthorizedAccessException();
        }

        public DatabaseResponse UpdateProviderDetails(LoggedInUser user, ProviderModel providerModel)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
