using Domain.Entities;
using Domain.Models.Users;
using Services.Abstractions;

namespace Services.UnAuthorized
{
    public class ConfigServices : IConfigServices
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
        public Task<List<ConfigRequestType>> GetConfigRequestTypeAsync(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<RequestDetails>> GetRequestDetailsAsync(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }
    }
}