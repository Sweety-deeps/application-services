using Domain.Models;
using Domain.Models.Users;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Dayforce;
using Services.Helpers;

namespace Services.IIA
{
    public class GPRIService : Services.PowerUser.GPRIService
    {
        public GPRIService(AppDbContext appDbContext, IDayforceApiClient dayforceApiClient, IS3Handling s3Handling,
            ILogger<GPRIService> logger, IEnumerable<IDataImportService> dataImportServices, Config config,
            ILoggedInUserRoleService loggedInUserRoleService, IDayforceSftpClient dayforceSftpClient, IDateTimeHelper dateTimeHelper)
        : base(appDbContext, dayforceApiClient, s3Handling, logger, dataImportServices, config, loggedInUserRoleService, dayforceSftpClient, dateTimeHelper)
        {
        }

        public override Task<List<GPRIModel>> GetGPRI(LoggedInUser user, bool isPayslip, string paygroupCode)
        {
            if (user.Paygroups.Any(upa => upa.payGroupCode == paygroupCode))
            {
                return base.GetGPRI(user, isPayslip, paygroupCode);
            }

            throw new UnauthorizedAccessException();
        }

        public override Task SendGPRI(LoggedInUser user, string fileId, string status)
        {
            var record = _appDbContext.gpri.Where(g => g.fileid == fileId).FirstOrDefault();
            if (user.Paygroups.Where(x => x.payGroupId == record.paygroupid).Any())
            {
                return base.SendGPRI(user, fileId, status);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
