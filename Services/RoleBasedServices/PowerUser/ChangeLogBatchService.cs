using Amazon.EventBridge;
using Amazon.Lambda.Core;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;

namespace Services.PowerUser
{
    public class ChangeLogBatchService : Services.ChangeLogBatchService
    {

        public ChangeLogBatchService(AppDbContext appDbContext, ILogger<ChangeLogBatchService> logger, IServiceProvider serviceProvider, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, serviceProvider, dateTimeHelper)
        {
        }

        public override bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public override bool CanView(LoggedInUser user)
        {
            return false;
        }

        public override Task<List<ChangeLogBatchResponseModel>> GetAll()
        {
            throw new UnauthorizedAccessException();
        }
    }
}

