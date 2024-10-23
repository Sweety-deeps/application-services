using Persistence;
using Services.Abstractions;
using Microsoft.Extensions.Logging;
using Domain.Models;
using Domain.Models.Users;
using Services.Helpers;

namespace Services.IIA
{
    public class PayrollElementServices : Services.PowerUser.PayrollElementServices
    {
        public PayrollElementServices(AppDbContext appDbContext, IEnumerable<IDataImportService> dataImportService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper, ILogger logger = null) : base(appDbContext, dataImportService, loggedInUserRoleService,dateTimeHelper, logger)
        {
        }

        public override List<PayrollElementsModel> GetPayrollElements(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == paygroupCode).Any())
            {
                return base.GetPayrollElements(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
