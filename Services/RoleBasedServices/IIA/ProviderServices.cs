using Services.Abstractions;
using Domain.Models;
using Domain.Models.Users;
using Persistence;
using Microsoft.Extensions.Logging;
using Services.Helpers;

namespace Services.IIA
{
    public class ProviderServices : Services.PowerUser.ProviderServices
    {
        public ProviderServices(AppDbContext appDbContext, IDateTimeHelper dateTimeHelper, ILogger logger = null) : base(appDbContext,dateTimeHelper, logger)
        {
        }

        public override bool CanView(LoggedInUser user)
        {
            return false;
        }
    }
}
