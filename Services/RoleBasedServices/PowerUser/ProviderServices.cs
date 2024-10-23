using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Abstractions;
using Domain.Models;
using Domain.Entities;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using Domain.Models.Users;
using Services.Helpers;

namespace Services.PowerUser
{
    public class ProviderServices : Services.ProviderServices
    {
        public ProviderServices(AppDbContext appDbContext, IDateTimeHelper dateTimeHelper,ILogger logger = null) : base(appDbContext,dateTimeHelper, logger)
        {
        }

        public override DatabaseResponse DeleteProvider(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse InsertProviderDetails(LoggedInUser user, ProviderModel providerModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse UpdateProviderDetails(LoggedInUser user, ProviderModel providerModel)
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
