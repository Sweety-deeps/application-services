using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CM
{
    public class CspListServices : Services.IOA.CspListServices 
    {
        public CspListServices(AppDbContext appDbContext, ILogger<CspListServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {

        }
    }
}
