using Domain.Models;
using Domain.Models.Users;
using Services.Abstractions;

namespace Services.UnAuthorized
{
    public class JsonReviewService : IJsonReviewService
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

        public Task<BaseResponseModel<string>> DownloadJsonReview(LoggedInUser user, int requestId)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<JsonReviewModel>> GetJsonReview(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
