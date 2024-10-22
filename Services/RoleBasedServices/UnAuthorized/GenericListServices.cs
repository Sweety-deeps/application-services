using Domain.Models;
using Domain.Models.Users;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UnAuthorized
{
    public class GenericListServices : IGenericListServices
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

        public Task<List<GenericListModel>> GetGenericListDetails(LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<DatabaseResponse> AddOrUpdateGenericListAsync(LoggedInUser loggedInUser, GenericListModel data)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<DatabaseResponse> DeleteGenericList(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

    }
}
