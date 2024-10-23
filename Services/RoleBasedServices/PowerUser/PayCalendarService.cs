using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;
using System.Data;

namespace Services.PowerUser
{
    public class PayCalendarService : Services.PayCalendarService
    {
        public PayCalendarService(AppDbContext appDbContext, ILogger<PayCalendarService> logger, IEnumerable<IDataImportService> dataImportService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dataImportService, loggedInUserRoleService, dateTimeHelper)
        {
        }

        public override DatabaseResponse AddPayCalendar(LoggedInUser user, PayCalendarModel payCalendarModel)
        {
            if (user.Paygroups.Where(p => p.payGroupId == payCalendarModel.paygroupid).Any())
            {
                return base.AddPayCalendar(user, payCalendarModel);
            }
            throw new UnauthorizedAccessException();
        }

        public override void DeletePayCalendar(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse UpdatePayCalendar(LoggedInUser user, PayCalendarModel payCalendarModel)
        {
            if (user.Paygroups.Where(p => p.payGroupId == payCalendarModel.paygroupid).Any())
            {
                return base.UpdatePayCalendar(user, payCalendarModel);
            }
            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> UploadPayCalender(LoggedInUser user, PayCalendarUploadModal payCalendarUploadModal)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == payCalendarUploadModal.paygroup).Any())
            {
                return base.UploadPayCalender(user, payCalendarUploadModal);
            }
            throw new UnauthorizedAccessException();
        }

        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }
    }
}