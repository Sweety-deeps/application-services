using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services.IOA
{
    public class LegalEntityServices : Services.IIA.LegalEntityServices
    {
        public LegalEntityServices(AppDbContext appDbContext, ILogger<LegalEntityServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {
        }
        public override Task<DatabaseResponse> UpdateLegalEntity(LoggedInUser user, LegalEntityModel legalEntityModel)
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

        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }
    }
}
