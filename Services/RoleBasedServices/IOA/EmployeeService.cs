using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;

namespace Services.IOA
{
    public class EmployeeService : Services.IIA.EmployeeService
    {
        public EmployeeService(AppDbContext appDbContext, ILogger<EmployeeService> logger, ISelectListHelper selectListHelper) : base(appDbContext, logger, selectListHelper)
        {
        }
    }
}