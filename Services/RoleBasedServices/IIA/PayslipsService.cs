using Amazon.S3;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Dayforce;
using Services.Helpers;

namespace Services.IIA
{
    public class PayslipsService : Services.PowerUser.PayslipsService
    {
        public PayslipsService(AppDbContext appDbContext, IAmazonS3 awsS3Client,
            IDayforceApiClient dayforceApiClient, IDayforceSftpClient dayforceSftpClient,
            ILogger<PayslipsService> logger, Config config, IDateTimeHelper dateTimeHelper,
            ILoggedInUserRoleService loggedInUserRoleService, IEnumerable<IDataImportService> dataImportServiceCollection, IAmazonS3 s3Client)
            : base(appDbContext, awsS3Client, dayforceApiClient, dayforceSftpClient, logger, config, dateTimeHelper, loggedInUserRoleService, dataImportServiceCollection, s3Client)
        {
        }

        public override List<PaySlipsDownloadModel> DownloadPayslipData(LoggedInUser user, string fileId)
        {
            var record = _appDbContext.gpri.Where(g => g.fileid == fileId).FirstOrDefault();
            if (user.Paygroups.Where(x => x.payGroupId == record.paygroupid).Any())
            {
                return base.DownloadPayslipData(user, fileId);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<PaySlipsModel> GetPayslipData(LoggedInUser user, string fileId)
        {
            var record = _appDbContext.gpri.Where(g => g.fileid == fileId).FirstOrDefault();
            if (user.Paygroups.Where(x => x.payGroupId == record.paygroupid).Any())
            {
                return base.GetPayslipData(user, fileId);
            }
            throw new UnauthorizedAccessException();
        }

        public override async Task<string> GetGpriPayslipMetadata(LoggedInUser user, string fileId, int gpriPayslipId)
        {
            var record = _appDbContext.gpri.Where(g => g.fileid == fileId).FirstOrDefault();
            if (record != null && user.Paygroups.Where(x => x.payGroupId == record.paygroupid).Any())
            {
                return await base.GetGpriPayslipMetadata(user, fileId, gpriPayslipId);
            }

            throw new UnauthorizedAccessException();
        }

        public override Task<BaseResponseModel<string>> SendPayslip(LoggedInUser user, string paygroupCode, string fileId)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == paygroupCode).Any())
            {
                return base.SendPayslip(user, paygroupCode, fileId);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
