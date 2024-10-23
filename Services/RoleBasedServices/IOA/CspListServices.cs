using Domain.Models.Users;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;

namespace Services.IOA
{
    public class CspListServices : Services.IIA.CspListServices
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
