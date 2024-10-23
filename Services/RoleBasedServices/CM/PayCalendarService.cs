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

namespace Services.CM
{
    public class PayCalendarService : Services.IOA.PayCalendarService
    {
        public PayCalendarService(AppDbContext appDbContext, ILogger<PayCalendarService> logger, IEnumerable<IDataImportService> dataImportService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dataImportService, loggedInUserRoleService, dateTimeHelper)
        {
        }

        public override DatabaseResponse AddPayCalendar(LoggedInUser user, PayCalendarModel payCalendarModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override void DeletePayCalendar(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse UpdatePayCalendar(LoggedInUser user, PayCalendarModel payCalendarModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> UploadPayCalender(LoggedInUser user, PayCalendarUploadModal mdl)
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
    }
}