using Domain.Models;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Dayforce;
using Services.Helpers;

namespace Services.IOA
{
    public class GPRIService : Services.IIA.GPRIService
    {
        public GPRIService(AppDbContext appDbContext, IDayforceApiClient dayforceApiClient, IS3Handling s3Handling,
            ILogger<GPRIService> logger, IEnumerable<IDataImportService> dataImportServices, Config config,
            ILoggedInUserRoleService loggedInUserRoleService, IDayforceSftpClient dayforceSftpClient, IDateTimeHelper dateTimeHelper)
            : base(appDbContext, dayforceApiClient, s3Handling, logger, dataImportServices, config, loggedInUserRoleService, dayforceSftpClient, dateTimeHelper)
        {
        }
    }
}
