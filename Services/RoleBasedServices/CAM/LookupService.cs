using Domain.Models;
using Domain.Models.Users;
using Persistence;
using Services.Abstractions;

namespace Services.CAM
{
    public class LookupService : Services.PowerUser.LookupService
    {
        public LookupService(AppDbContext appDbContext, IEnumerable<IPayGroupServices> paygroupService, IEnumerable<ILegalEntityServices> legalEntityService, IEnumerable<IClientServices> clientService, ILoggedInUserRoleService loggedInUserRoleService) : base(appDbContext, paygroupService, legalEntityService, clientService, loggedInUserRoleService)
        {
        }
    }
}

