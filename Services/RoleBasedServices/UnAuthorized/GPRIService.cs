using Amazon.CognitoIdentityProvider;
using Domain.Enums;
using Domain.Models;
using Domain.Enums;
using Domain.Models.Users;
using Services.Abstractions;
namespace Services.UnAuthorized
{
    public class GPRIService : IGPRIService
    {
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
        
        public Task<StatusTypes> AddGPRI(LoggedInUser user, GPRIModel model)
        {
            throw new UnauthorizedAccessException();
        }

        public void DeleteGPRI(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<GPRIModel>> GetGPRI(LoggedInUser user, bool isPayslip, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public Task SendGPRI(LoggedInUser user, string fileID, string status)
        {
            throw new UnauthorizedAccessException();
        }

        public Task UpdateGPRI(LoggedInUser user, string fileID, string status, string next, string sendGpriResult)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> DownloadGpri(LoggedInUser user, string fileId)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
