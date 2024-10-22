using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface ILookupService
    {
        List<LookupValue> GetLookupFields(LoggedInUser user, int id, String? value);
    }
}
