using Microsoft.Extensions.Logging;
using Persistence;
using Domain.Models;
using Amazon.S3;
using Services.Helpers;
using Domain.Models.Users;

namespace Services.IIA
{
    public class ReportDeltaService : Services.PowerUser.ReportDeltaService
    {
        public ReportDeltaService(AppDbContext appDbContext, IAmazonS3 s3Client, ILogger<ReportDeltaService> logger, ISelectListHelper selectListHelper, IReportServiceHelper reportServiceHelper, IDateTimeHelper dateTimeHelper)
        : base(appDbContext, s3Client, logger, selectListHelper, reportServiceHelper, dateTimeHelper)
        {
        }

        public override bool IsAuthorisedtoAccess(LoggedInUser user, DeltaRequestModel _requestModel)
        {
            return user.Paygroups.Any(x => x.payGroupCode == _requestModel.paygroup);
        }
    }
}
