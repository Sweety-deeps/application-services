using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;

namespace Services.CM
{
    public class EmployeeService : Services.IOA.EmployeeService
    {
        public EmployeeService(AppDbContext appDbContext, ILogger<EmployeeService> logger, ISelectListHelper selectListHelper) : base(appDbContext, logger, selectListHelper)
        {
        }
    }
}
