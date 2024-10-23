using Domain.Enums;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Dayforce;
using Services.Helpers;

namespace Services.PowerUser
{
    public class GPRIService : Services.GPRIService
    {
        public GPRIService(AppDbContext appDbContext, IDayforceApiClient dayforceApiClient, IS3Handling s3Handling,
            ILogger<GPRIService> logger, IEnumerable<IDataImportService> dataImportServices, Config config,
            ILoggedInUserRoleService loggedInUserRoleService, IDayforceSftpClient dayforceSftpClient, IDateTimeHelper dateTimeHelper)
            : base(appDbContext, dayforceApiClient, s3Handling, logger, dataImportServices, config, loggedInUserRoleService, dayforceSftpClient, dateTimeHelper)
        {
        }

        public override Task<StatusTypes> AddGPRI(LoggedInUser user, GPRIModel model)
        {
            if (user.Paygroups.Where(x => x.payGroupId == model.paygroupid).Any())
            {
                return base.AddGPRI(user, model);
            }
            throw new UnauthorizedAccessException();
        }

        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public override void DeleteGPRI(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task UpdateGPRI(LoggedInUser user, string fileId, string status, string next, string sendGpriResult)
        {
            var record = _appDbContext.gpri.Where(g => g.fileid == fileId).FirstOrDefault();
            if (user.Paygroups.Where(x => x.payGroupId == record.paygroupid).Any())
            {
                return base.UpdateGPRI(user, fileId, status, next, sendGpriResult);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
