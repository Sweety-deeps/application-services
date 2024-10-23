using Persistence;
using Services.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.Data;
using EFCore.BulkExtensions;
using Domain;
using Domain.Models.Users;
using Services.Helpers;

namespace Services.PowerUser
{
    public class PayrollElementServices : Services.PayrollElementServices
    {
        public PayrollElementServices(AppDbContext appDbContext, IEnumerable<IDataImportService> dataImportService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper, ILogger logger = null) : base(appDbContext, dataImportService, loggedInUserRoleService, dateTimeHelper, logger)
        {
        }

        public override void DeletePayrollElements(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse InsertPayrollElements(LoggedInUser user, PayrollElementsModel payrollelementsModel)
        {
            if (user.Paygroups.Where(p => p.payGroupId == payrollelementsModel.paygroupid).Any())
            {
                return base.InsertPayrollElements(user, payrollelementsModel);
            }
            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse UpdatePayrollElements(LoggedInUser user, PayrollElementsModel payrollelementsModel)
        {
            if (user.Paygroups.Where(p => p.payGroupId == payrollelementsModel.paygroupid).Any())
            {
                return base.UpdatePayrollElements(user, payrollelementsModel);
            }
            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> UploadPayElements(LoggedInUser user, PayCalendarUploadModal mdl)
        {
            if (user.Paygroups.Where(p => p.name == mdl.paygroup).Any())
            {
                return base.UploadPayElements(user, mdl);
            }
            throw new UnauthorizedAccessException();
        }

        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }
    }
}
