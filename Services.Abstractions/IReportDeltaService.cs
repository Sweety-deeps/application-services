using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReportDeltaService : IUIPermissions
    {
        List<DeltaResponseModel> GetDeltaReport(LoggedInUser user, DeltaRequestModel _data);

        Task<string> GetDeltaReport(LoggedInUser user, List<DeltaResponseModel> results, DeltaRequestModel _mdl, string fileName);

    }
}
