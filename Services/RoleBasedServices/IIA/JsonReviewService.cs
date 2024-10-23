using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services.IIA
{
    public class JsonReviewService : Services.PowerUser.JsonReviewService
    {
        public JsonReviewService(AppDbContext appDbContext, ILogger<JsonReviewService> logger, IConfigServices configServices, IS3Handling s3Handling,IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, configServices, s3Handling, dateTimeHelper)
        {
        }

        public override async Task<BaseResponseModel<string>> DownloadJsonReview(LoggedInUser user, int requestId)
        {
            var requestDetails = await _appDbContext.requestdetails.Where(t => t.id == requestId).FirstOrDefaultAsync();
            if (user.Paygroups.Any(x => x.payGroupCode == requestDetails.paygroup))
            {
                return await base.DownloadJsonReview(user, requestId);
            }
            throw new UnauthorizedAccessException();
        }

        public override Task<List<JsonReviewModel>> GetJsonReview(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Any(x => x.payGroupCode == paygroupCode))
            {
                return base.GetJsonReview(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
