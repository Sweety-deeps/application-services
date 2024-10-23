using Domain.Entities;
using Domain.Entities.Users;
using Domain.Models;
using Domain.Models.Users;
using Domain.Models.Users.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Persistence;
using Services.Helpers;
using Services.Users;

namespace Services.PowerUser
{
    public class UserService : Services.UserService
    {
        public UserService(ILogger<UserService> logger,
            AppDbContext appDbContext, ICognitoService cognitoService, IDateTimeHelper dateTimeHelper) : base(logger, appDbContext, cognitoService, dateTimeHelper)
        {
        }

        public override UserPageModel SearchUsers(int limit, int offset, Dictionary<string, string> userSearchFilterParams, LoggedInUser loggedInUser)
        {
            var loggedInUserPaygroups = loggedInUser.Paygroups.Select(x => x.payGroupId).Distinct().ToList();
            var accessibleUsers = _appDbContext.userpaygroupassignment
                                  .Where(x => loggedInUserPaygroups.Contains(x.PaygroupId))
                                  .Select(x => x.UserId)
                                  .Distinct()
                                  .ToList();

            List<UserResponseModel> allFilteredUsers = _appDbContext.users
                                                        .Where(x => accessibleUsers.Contains(x.Id))
                                                        .AsEnumerable()
                                                        .Filter(userSearchFilterParams)
                                                        .OrderBy(e => e.Email)
                                                        .Select(e => e.ToUserResponseModel())
                                                        .ToList();

            int totalRecords = allFilteredUsers.Count;
            int totalPages = totalRecords / limit + (totalRecords % limit > 0 ? 1 : 0);

            return new UserPageModel()
            {
                data = allFilteredUsers.Skip(offset).Take(limit).ToList(),
                totalPages = totalPages,
                totalRecords = totalRecords
            };
        }

        public override async Task<UserPageModel> GetAllUser(int limit, int offset, LoggedInUser loggedInUser)
        {
            var loggedInUserPaygroups = loggedInUser.Paygroups.Select(x => x.payGroupId).Distinct().ToList();
            var allUsers = _appDbContext.userpaygroupassignment
                                  .Where(x => loggedInUserPaygroups.Contains(x.PaygroupId))
                                  .Select(x => x.UserId)
                                  .Distinct();

            List<UserResponseModel> userList = await (
                                                      from U in _appDbContext.users
                                                      where allUsers.Contains(U.Id)
                                                      select U
                                                     )
                                                     .OrderBy(e => e.Email)
                                                     .Select(e => e.ToUserResponseModel())
                                                     .ToListAsync();

            int totalRecords = userList.Count;
            int totalPages = totalRecords / limit + (totalRecords % limit > 0 ? 1 : 0);

            return new UserPageModel()
            {
                data = userList.Skip(offset).Take(limit).ToList(),
                totalPages = totalPages,
                totalRecords = totalRecords
            };
        }

        public override async Task<UserResponseModel?> GetUser(Guid id, LoggedInUser loggedInUser)
        {
            var loggedInUserPaygroups = loggedInUser.Paygroups.Select(x => x.payGroupId).Distinct().ToList();

            var result = await _appDbContext.users.Where(e => e.Id.Equals(id)).FirstOrDefaultAsync();
            var assignedPaygroups = from userpaygroupassignment in _appDbContext.userpaygroupassignment
                                    join client in _appDbContext.client on userpaygroupassignment.ClientId equals client.id
                                    join legalentity in _appDbContext.legalentity on userpaygroupassignment.LegalEntityId equals legalentity.id
                                    join paygroup in _appDbContext.paygroup on userpaygroupassignment.PaygroupId equals paygroup.id
                                    where userpaygroupassignment.UserId.Equals(id) && loggedInUserPaygroups.Contains(userpaygroupassignment.PaygroupId)
                                    select new PayGroupAssignmentModel
                                    {
                                        Id = userpaygroupassignment.Id,
                                        PayGroupCode = paygroup.code,
                                        ClientName = client.name,
                                        LegalEntityName = legalentity.name,
                                        ClientId = client.id,
                                        LegalEntityId = legalentity.id,
                                        PaygroupId = paygroup.id,
                                        Status = userpaygroupassignment.Status,
                                    };

            if (result != null)
            {
                var pasygroupAssignmentResult = await assignedPaygroups.ToListAsync();
                return result.ToUserResponseModel(pasygroupAssignmentResult);
            }

            // Todo: just reconfirm once if this needs to be null or thrown exception
            return null;
        }

        public override Task<UserResponseModel> AddUser(UserModel userModel, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task<BaseResponseModel<bool>> DeleteUserAsync(Guid id, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }

        public override async Task<BaseResponseModel<UserResponseModel>> UpdateUser(string userId, UserModel userModel, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task<BaseResponseModel<string?>> UpdateUserStatus(Guid id, string status, LoggedInUser loggedInUser)
        {
            throw new UnauthorizedAccessException();
        }

        protected virtual List<String> GetAccessibleRoles()
        {
            return new List<String>
            {
                Role.document_manager.ToString(),
                Role.CAM.ToString(),
                Role.country_manager.ToString(),
                Role.Interface_oa.ToString(),
                Role.interface_ic.ToString(),
                Role.poweruser.ToString()
            };
        }

        public override bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public override bool CanEdit(LoggedInUser user)
        {
            return false;
        }
    }
}