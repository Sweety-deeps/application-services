using Domain.Entities;
using Domain.Models.Users;
using DomainLayer.Entities;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services.PowerUser
{
    public class EmployeeService : Services.EmployeeService
    {
        public EmployeeService(AppDbContext appDbContext, ILogger<EmployeeService> logger, ISelectListHelper selectListHelper) : base(appDbContext, logger, selectListHelper)
        {
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
