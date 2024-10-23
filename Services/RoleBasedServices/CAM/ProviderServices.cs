using Services.Abstractions;
using Domain.Models;
using Domain.Models.Users;
using Persistence;
using Microsoft.Extensions.Logging;
using Services.Helpers;

namespace Services.CAM
{
    public class ProviderServices : Services.CM.ProviderServices
    {
        public ProviderServices(AppDbContext appDbContext, IDateTimeHelper dateTimeHelper, ILogger logger = null) : base(appDbContext,dateTimeHelper, logger)
        {
        }
    }
}
