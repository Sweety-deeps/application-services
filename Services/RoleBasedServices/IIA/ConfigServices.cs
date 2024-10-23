using Domain.Entities;
using Domain.Models.Users;
using Services.Abstractions;

namespace Services.IIA
{
    public class ConfigServices : IConfigServices
    {
        public bool CanAdd(LoggedInUser user)
        {
            throw new NotImplementedException();
        }

        public bool CanDelete(LoggedInUser user)
        {
            throw new NotImplementedException();
        }

        public bool CanEdit(LoggedInUser user)
        {
            throw new NotImplementedException();
        }

        public bool CanView(LoggedInUser user)
        {
            throw new NotImplementedException();
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