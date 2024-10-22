using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IJsonReviewService : IUIPermissions
    {
        Task<List<JsonReviewModel>> GetJsonReview(LoggedInUser user, string paygroupCode);

        Task<BaseResponseModel<string>> DownloadJsonReview(LoggedInUser user, int requestId);
        
    }
}
