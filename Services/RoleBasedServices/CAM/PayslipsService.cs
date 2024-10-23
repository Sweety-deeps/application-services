using Amazon.S3;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Dayforce;
using Services.Helpers;

namespace Services.CAM
{
    public class PayslipsService : Services.CM.PayslipsService
    {
        public PayslipsService(AppDbContext appDbContext, IAmazonS3 awsS3Client,
            IDayforceApiClient dayforceApiClient, IDayforceSftpClient dayforceSftpClient,
            ILogger<PayslipsService> logger, Config config, IDateTimeHelper dateTimeHelper,
            ILoggedInUserRoleService loggedInUserRoleService, IEnumerable<IDataImportService> dataImportServiceCollection, IAmazonS3 s3Client)
            : base(appDbContext, awsS3Client, dayforceApiClient, dayforceSftpClient, logger, config, dateTimeHelper, loggedInUserRoleService, dataImportServiceCollection, s3Client)
        {
        }
    }
}
