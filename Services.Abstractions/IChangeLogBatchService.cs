using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IChangeLogBatchService : IUIPermissions
    {
        Task<List<ChangeLogBatchResponseModel>> GetAll();
        Task<bool> LauchChangeLogBatchProcess(LoggedInUser user, String eventFunctionArn);
    }
}
