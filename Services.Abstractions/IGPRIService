using Domain.Enums;
using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IGPRIService : IUIPermissions
    {
        Task<StatusTypes> AddGPRI(LoggedInUser user, GPRIModel model);
        Task<List<GPRIModel>> GetGPRI(LoggedInUser user, bool isPayslip, string paygroupCode);
        Task UpdateGPRI(LoggedInUser user, string fileID, string status, string next, string sendGpriResult);
        Task SendGPRI(LoggedInUser user, string fileID, string status);
        void DeleteGPRI(LoggedInUser user, int id);
        Task<string> DownloadGpri(LoggedInUser user, string fileId);
    }
}
