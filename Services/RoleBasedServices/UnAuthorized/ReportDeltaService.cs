using Services.Abstractions;
using Domain.Models;
using Domain.Models.Users;

namespace Services.UnAuthorized
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportDeltaService : IReportDeltaService
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

        public virtual List<DeltaResponseModel> GetDeltaReport(LoggedInUser user, DeltaRequestModel _data)
        {
            throw new UnauthorizedAccessException();
        }

        public virtual async Task<string> GetDeltaReport(LoggedInUser user, List<DeltaResponseModel> results, DeltaRequestModel _mdl, string fileName)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
