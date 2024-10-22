using Domain.Models.Users;
using System.Security.Claims;

namespace Services.Abstractions
{
    public interface ILoggedInUserService
    {
        LoggedInUser GetLoggedInUser(ClaimsPrincipal LoggedInUserClaims);
    }
}
