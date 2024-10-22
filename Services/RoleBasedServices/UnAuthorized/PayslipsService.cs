using Domain.Models;
using Domain.Models.Users;
using Services.Abstractions;
using System.IO.Compression;

namespace Services.UnAuthorized
{
    public class PayslipsService : IPayslipsService
    {
        public virtual bool CanView(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public List<PaySlipsDownloadModel> DownloadPayslipData(LoggedInUser user, string fileID)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> DownloadPayslips(LoggedInUser user, List<PaySlipsDownloadModel> results, string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public List<PaySlipsModel> GetPayslipData(LoggedInUser user, string fileID)
        {
            throw new UnauthorizedAccessException();
        }

        public async Task<string> GetGpriPayslipMetadata(LoggedInUser user, string fileId, int gpriPayslipId)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<BaseResponseModel<string>> SendPayslip(LoggedInUser user, string paygroupCode, string fileId)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<DatabaseResponse> UploadPaySlips(LoggedInUser user, PaySlipsUploadModel model)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<DatabaseResponse> UploadPaySlipsFromS3(LoggedInUser user, PaySlipsUploadFromS3Model mdl)
        {
            throw new UnauthorizedAccessException();
        }
        
        public Task<BaseResponseModel<string>> GetPayslipUploadUrl(LoggedInUser user, string paygroup, string fileName)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
