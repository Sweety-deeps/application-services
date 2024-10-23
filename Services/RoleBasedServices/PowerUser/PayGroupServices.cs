using Domain.Models;
using Domain.Models.Users;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;

namespace Services.PowerUser
{
    public class PayGroupServices : Services.PayGroupServices
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
            if (user.Paygroups.Any(upa => upa.payGroupId == payGroupModel.id))
            {
                return base.UpdatePayGroupDetails(user, payGroupModel);
            }

            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse DeletePayGroup(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task<bool> LauchCollectChangesAction(LoggedInUser loggedInUser, PayGroupModel payGroupModel, String eventFunctionArn)
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
