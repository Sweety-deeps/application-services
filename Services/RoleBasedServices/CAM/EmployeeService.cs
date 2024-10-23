using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;

namespace Services.CAM
{
    public class EmployeeService : Services.CM.EmployeeService
    {
        public EmployeeService(AppDbContext appDbContext, ILogger<EmployeeService> logger, ISelectListHelper selectListHelper) : base(appDbContext, logger, selectListHelper)
        {
        }
    }
}
