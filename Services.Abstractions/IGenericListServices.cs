using Domain.Models.Users;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IGenericListServices: IUIPermissions
    {
        Task<List<GenericListModel>> GetGenericListDetails(LoggedInUser loggedInUser);
        Task<DatabaseResponse> AddOrUpdateGenericListAsync(LoggedInUser loggedInUser, GenericListModel data);
        Task<DatabaseResponse> DeleteGenericList(LoggedInUser user, int id);
    }
}
