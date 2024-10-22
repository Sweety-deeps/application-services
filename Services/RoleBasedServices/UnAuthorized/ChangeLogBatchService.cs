using Domain.Models;
using Domain.Models.Users;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;

namespace Services.UnAuthorized
{
    public class ChangeLogBatchService : IChangeLogBatchService
    {

        public ChangeLogBatchService(AppDbContext appDbContext, ILogger<ChangeLogBatchService> logger)
        {
        }

        public bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public bool CanView(LoggedInUser user)
        {
            return false;
        }

        public Task<List<ChangeLogBatchResponseModel>> GetAll()
        {
            throw new UnauthorizedAccessException();
        }

        public Task<bool> LauchChangeLogBatchProcess(LoggedInUser user, String eventFunctionArn)
        {
            throw new UnauthorizedAccessException();
        }
    }
}

