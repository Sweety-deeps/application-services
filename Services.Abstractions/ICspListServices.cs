using Domain.Models;
using Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface ICspListServices: IUIPermissions
    {
        Task<List<CspListModel>> GetCspListDetails(LoggedInUser loggedInUser);
        Task<DatabaseResponse> AddOrUpdateCspListAsync(LoggedInUser loggedInUser, CspListModel data);
        Task<DatabaseResponse> DeleteCspList(LoggedInUser loggedInUser, int id);
    }
}
