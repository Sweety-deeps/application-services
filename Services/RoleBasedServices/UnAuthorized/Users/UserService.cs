using Domain.Entities;
using Domain.Entities.Users;
using Domain.Models;
using Domain.Models.Users;
using Domain.Models.Users.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Persistence;
using Services.Users;

namespace Services.UnAuthorized
{
    public class UserService : IUserService
    {
        public Task<UserResponseModel> AddUser(UserModel userModel, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }

        public bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public bool CanView(LoggedInUser user)
        {
            return false;
        }

        public Task<BaseResponseModel<bool>> DeleteUserAsync(Guid id, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<BaseResponseModel<bool>> DeleteUserPayGroupAsync(int id, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<UserPageModel> GetAllUser(int limit, int offset, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<UserResponseModel?> GetUser(Guid id, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }

        public UserPageModel SearchUsers(int limit, int offset, Dictionary<string, string> userSearchFilterParams, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<BaseResponseModel<string?>> SetLastLoggedIn(LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<BaseResponseModel<UserResponseModel>> UpdateUser(string userId, UserModel userModel, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<BaseResponseModel<string?>> UpdateUserStatus(Guid id, string status, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
