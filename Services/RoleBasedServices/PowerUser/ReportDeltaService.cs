using Microsoft.Extensions.Logging;
using Persistence;
using Amazon.S3;
using Services.Helpers;

namespace Services.PowerUser
{
    public class ReportDeltaService : Services.ReportDeltaService
    {
        public ReportDeltaService(AppDbContext appDbContext, IAmazonS3 s3Client, ILogger<ReportDeltaService> logger, ISelectListHelper selectListHelper, IReportServiceHelper reportServiceHelper, IDateTimeHelper dateTimeHelper) : base(appDbContext, s3Client, logger, selectListHelper, reportServiceHelper, dateTimeHelper)
        {
        }
    }
}
