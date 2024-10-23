using Services.Abstractions;
using Domain.Models;
using Domain.Models.Users;
using Persistence;
using Microsoft.Extensions.Logging;
using Services.Helpers;

namespace Services.IOA
{
    public class ProviderServices : Services.IIA.ProviderServices
    {
        public ProviderServices(AppDbContext appDbContext, IDateTimeHelper dateTimeHelper, ILogger logger = null) : base(appDbContext,dateTimeHelper, logger)
        {
        }
    }
}
