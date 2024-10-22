using Domain.Models;
using Domain.Models.Users;
using Services.Abstractions;

namespace Services.UnAuthorized
{
    public class ClientServices : IClientServices
    {
        public bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public bool CanView(LoggedInUser user)
        {
            return false;
        }

        public Task<DatabaseResponse> DeleteClient(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<ClientModel>> GetClient(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<PayGroupModel>> GetPayGroupByClient(LoggedInUser user, int clientid)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<DatabaseResponse> InsertClient(LoggedInUser user, ClientModel clientModel)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<DatabaseResponse> UpdateClient(LoggedInUser user, ClientModel clientModel)
        {
            throw new UnauthorizedAccessException();
        }

        Task<List<PaygroupMinimalModel>> IClientServices.GetPayGroupByClient(LoggedInUser user, int clientid)
        {
            throw new NotImplementedException();
        }
    }
}

