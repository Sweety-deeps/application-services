using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services.CM
{
    public class LegalEntityServices : Services.IOA.LegalEntityServices
    {
        public LegalEntityServices(AppDbContext appDbContext, ILogger<LegalEntityServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {
        }
    }
}
