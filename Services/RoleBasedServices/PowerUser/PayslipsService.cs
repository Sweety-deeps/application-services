using Amazon.S3;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Dayforce;
using Services.Helpers;
using System.Data;

namespace Services.PowerUser
{
    public class PayslipsService : Services.PayslipsService
    {
        public PayslipsService(AppDbContext appDbContext, IAmazonS3 awsS3Client,
            IDayforceApiClient dayforceApiClient, IDayforceSftpClient dayforceSftpClient,
            ILogger<PayslipsService> logger, Config config, IDateTimeHelper dateTimeHelper,
            ILoggedInUserRoleService loggedInUserRoleService, IEnumerable<IDataImportService> dataImportServiceCollection, IAmazonS3 s3Client)
            : base(appDbContext, awsS3Client, dayforceApiClient, dayforceSftpClient, logger, config, dateTimeHelper, loggedInUserRoleService, dataImportServiceCollection, s3Client)
        {
        }

        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public override Task<DatabaseResponse> UploadPaySlips(LoggedInUser user, PaySlipsUploadModel model)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == model.paygroup).Any() || user.Paygroups.Where(x => x.name == model.paygroup).Any())
            {
                return base.UploadPaySlips(user, model);
            }
            throw new UnauthorizedAccessException();
        }

        public override Task<BaseResponseModel<string>> GetPayslipUploadUrl(LoggedInUser user, string paygroup, string fileName)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == paygroup).Any() || user.Paygroups.Where(x => x.name == paygroup).Any())
            {
                return base.GetPayslipUploadUrl(user, paygroup, fileName);
            }
            throw new UnauthorizedAccessException();
        }
    }
}