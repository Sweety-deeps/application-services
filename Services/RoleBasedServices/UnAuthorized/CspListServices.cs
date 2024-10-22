using Domain.Models.Users;
using Domain.Models;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UnAuthorized
{
    public class CspListServices : ICspListServices
    {
        public virtual bool CanView(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public Task<List<CspListModel>> GetCspListDetails(LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<DatabaseResponse> AddOrUpdateCspListAsync(LoggedInUser loggedInUser, CspListModel data)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<DatabaseResponse> DeleteCspList(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

    }
}
