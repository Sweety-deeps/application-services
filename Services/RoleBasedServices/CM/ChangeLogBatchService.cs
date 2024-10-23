using Amazon.EventBridge;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;

namespace Services.CM
{
    public class ChangeLogBatchService : Services.IOA.ChangeLogBatchService
    {

        public ChangeLogBatchService(AppDbContext appDbContext, ILogger<ChangeLogBatchService> logger, IServiceProvider serviceProvider, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, serviceProvider, dateTimeHelper)
        {
        }
    }
}

