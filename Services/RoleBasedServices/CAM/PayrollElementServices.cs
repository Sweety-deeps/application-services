using Persistence;
using Services.Abstractions;
using Microsoft.Extensions.Logging;
using Domain.Models;
using Domain.Models.Users;
using Services.Helpers;

namespace Services.CAM
{
    public class PayrollElementServices : Services.CM.PayrollElementServices
    {
        public PayrollElementServices(AppDbContext appDbContext, IEnumerable<IDataImportService> dataImportService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper, ILogger logger = null) : base(appDbContext, dataImportService, loggedInUserRoleService,dateTimeHelper, logger)
        {
        }
    }
}
