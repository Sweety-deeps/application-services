using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CAM
{
    public class GenericListServices : Services.CM.GenericListServices
    {
        public GenericListServices(AppDbContext appDbContext, ILogger<CM.GenericListServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {
        }
    }
}
