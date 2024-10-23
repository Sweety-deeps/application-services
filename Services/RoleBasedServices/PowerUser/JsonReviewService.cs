using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services.PowerUser
{
    public class JsonReviewService : Services.JsonReviewService
    {
        public JsonReviewService(AppDbContext appDbContext, ILogger<JsonReviewService> logger, IConfigServices configServices, IS3Handling s3Handling,IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, configServices, s3Handling,dateTimeHelper)
        {
        }
    }
}
