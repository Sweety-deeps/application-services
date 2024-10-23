using Amazon.CognitoIdentityProvider.Model;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;
using System.Data;

namespace Services.IIA
{
    public class PayCalendarService : Services.PowerUser.PayCalendarService
    {
        public PayCalendarService(AppDbContext appDbContext, ILogger<PayCalendarService> logger, IEnumerable<IDataImportService> dataImportService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dataImportService, loggedInUserRoleService, dateTimeHelper)
        {
        }

        public override List<PayCalendarModel> GetPayCalendar(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == paygroupCode).Any())
            {
                return base.GetPayCalendar(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<PayPeriods> GetPayPeriods(LoggedInUser user, int payGroupID)
        {
            if (user.Paygroups.Where(p => p.payGroupId == payGroupID).Any())
            {
                return base.GetPayPeriods(user, payGroupID);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<int> GetPayPeriodsByYear(LoggedInUser user, int payGroupID, int year)
        {
            if (user.Paygroups.Where(p => p.payGroupId == payGroupID).Any())
            {
                return base.GetPayPeriodsByYear(user, payGroupID, year);
            }
            throw new UnauthorizedAccessException();
        }
    }
}