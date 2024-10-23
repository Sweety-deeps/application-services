using Domain.Models;
using Domain.Models.Users;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.PowerUser
{
    public class CspListServices : Services.CspListServices
    {
        public CspListServices(AppDbContext appDbContext, ILogger<CspListServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {

        }
        public override async Task<List<CspListModel>> GetCspListDetails(LoggedInUser loggedInUser)
        {
            return await base.GetCspListDetails(loggedInUser);
        }
        public override Task<DatabaseResponse> AddOrUpdateCspListAsync(LoggedInUser loggedInUser, CspListModel data)
        {
            throw new UnauthorizedAccessException();
        }
        public override Task<DatabaseResponse> DeleteCspList(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }
        public override bool CanAdd(LoggedInUser user)
        {
            return false;
        }
        public override bool CanEdit(LoggedInUser user)
        {
            return false;
        }
        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }
    }
}