using Persistence;
using Services.Abstractions;
using Microsoft.Extensions.Logging;
using Domain.Models;
using Domain.Models.Users;
using Services.Helpers;

namespace Services.CM
{
    public class PayrollElementServices : Services.IOA.PayrollElementServices
    {
        public PayrollElementServices(AppDbContext appDbContext, IEnumerable<IDataImportService> dataImportService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper, ILogger logger = null) : base(appDbContext, dataImportService, loggedInUserRoleService,dateTimeHelper, logger)
        {
        }

        public override DatabaseResponse InsertPayrollElements(LoggedInUser user, PayrollElementsModel payrollelementsModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse UpdatePayrollElements(LoggedInUser user, PayrollElementsModel payrollelementsModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> UploadPayElements(LoggedInUser user, PayCalendarUploadModal mdl)
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
