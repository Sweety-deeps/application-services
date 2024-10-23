using Amazon.EventBridge;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;

namespace Services.IOA
{
    public class PayGroupServices : Services.IIA.PayGroupServices
    {
        public PayGroupServices(AppDbContext appDbContext, ILogger<PayGroupServices> logger, IDateTimeHelper dateTimeHelper, IEncrytionHelper encrytionHelper) : base(appDbContext, logger, dateTimeHelper, encrytionHelper)
        {
        }

        public override async Task<List<PaygroupMinimalModel>> GetActivePaygroups(LoggedInUser user)
        {
            return await base.GetActivePaygroups(user);
        }

        public override async Task<List<PayGroupModel>> GetPayGroupDetails(LoggedInUser user)
        {
            return await base.GetPayGroupDetails(user);
        }

        public override DatabaseResponse AddPayGroup(LoggedInUser user, PayGroupModel paygroupModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse UpdatePayGroupDetails(LoggedInUser user, PayGroupModel payGroupModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse DeletePayGroup(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public override bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public override bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }

    }
}
