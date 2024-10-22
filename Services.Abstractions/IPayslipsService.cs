using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IPayslipsService : IUIPermissions
    {
        Task<DatabaseResponse> UploadPaySlips(LoggedInUser user, PaySlipsUploadModel model);
        List<PaySlipsModel> GetPayslipData(LoggedInUser user, string fileID);
        Task<string> DownloadPayslips(LoggedInUser user, List<PaySlipsDownloadModel> results, string fileName);
        Task<string> GetGpriPayslipMetadata(LoggedInUser user, string fileId, int gpriPayslipId);
        List<PaySlipsDownloadModel> DownloadPayslipData(LoggedInUser user, string fileID);
        Task<BaseResponseModel<string>> SendPayslip(LoggedInUser user, string paygroupCode, string fileId);
        Task<BaseResponseModel<string>> GetPayslipUploadUrl(LoggedInUser user, string paygroup, string fileName); 
    }
}
