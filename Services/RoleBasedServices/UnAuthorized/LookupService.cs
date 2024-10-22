using Domain.Models;
using Domain.Models.Users;
using Services.Abstractions;

namespace Services.UnAuthorized
{
    public class LookupService : ILookupService
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
        
        public List<LookupValue> GetLookupFields(LoggedInUser user, int id, string? value)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
