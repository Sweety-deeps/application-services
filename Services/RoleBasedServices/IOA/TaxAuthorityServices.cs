using Domain.Models;
using Domain.Models.Users;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;

namespace Services.IOA
{
    public class TaxAuthorityServices : Services.IIA.TaxAuthorityServices
    {

        public TaxAuthorityServices(AppDbContext appDbContext, ILogger<TaxAuthorityServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {
        }
    }
}