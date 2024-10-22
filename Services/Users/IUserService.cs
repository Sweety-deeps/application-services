using Domain.Models;
using Domain.Models.Users;
using Services.Abstractions;

namespace Services.Users
{
    public interface IUserService : IUIPermissions
    {
        Task<UserResponseModel> AddUser(UserModel userModel, LoggedInUser loggedInUser);
        Task<BaseResponseModel<UserResponseModel>> UpdateUser(String userId, UserModel userModel, LoggedInUser loggedInUser);
        Task<BaseResponseModel<string?>> UpdateUserStatus(Guid id, string status, LoggedInUser loggedInUser);
        Task<UserResponseModel?> GetUser(Guid id, LoggedInUser loggedInUser);
        Task<UserPageModel> GetAllUser(int limit, int offset, LoggedInUser loggedInUser);
        UserPageModel SearchUsers(int limit, int offset, Dictionary<string, string> userSearchFilterParams, LoggedInUser loggedInUser);
        Task<BaseResponseModel<bool>> DeleteUserAsync(Guid id, LoggedInUser loggedInUser);
        Task<BaseResponseModel<string?>> SetLastLoggedIn(LoggedInUser loggedInUser);
        Task<BaseResponseModel<bool>> DeleteUserPayGroupAsync(int id, LoggedInUser loggedInUser);
    }
}
