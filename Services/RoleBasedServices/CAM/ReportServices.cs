using Microsoft.Extensions.Logging;
using Persistence;
using Amazon.S3;
using Services.Helpers;
using Domain.Models;

namespace Services.CAM
{
    public class ReportServices : Services.CM.ReportServices
    {
        public ReportServices(AppDbContext appDbContext, IAmazonS3 s3Client, ILogger<ReportServices> logger, ISelectListHelper selectListHelper, IDateTimeHelper dateTimeHelper, IReportServiceHelper reportServiceHelper, Config config)
        : base(appDbContext, s3Client, logger, selectListHelper, dateTimeHelper, reportServiceHelper, config)
        {
        }
    }
}
