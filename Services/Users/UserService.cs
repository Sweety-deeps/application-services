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

namespace Services
{
    public class UserService : IUserService
    {
        protected readonly ILogger<UserService> _logger;
        protected readonly AppDbContext _appDbContext;
        protected readonly ICognitoService _cognitoService;
        private readonly IDateTimeHelper _dateTimeHelper;
        public UserService(ILogger<UserService> logger,
            AppDbContext appDbContext, ICognitoService cognitoService,IDateTimeHelper dateTimeHelper)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _cognitoService = cognitoService;
            _dateTimeHelper = dateTimeHelper;
        }

        public virtual async Task<UserResponseModel> AddUser(UserModel userModel, LoggedInUser loggedInUser)
        {
            try
            {
                // Todo: validate usermodel before proceeding
                // Todo: User access validation
                // if (userModel.Id > 0)
                // {
                //     throw new InvalidDataException("Entity Id already exists");
                // }

                var user = new User
                {
                    UserId = userModel.UserId,
                    Email = userModel.Email,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    SecondLastName = userModel.SecondLastName,
                    MiddleName = userModel.MiddleName,
                    FullName = userModel.FullName,
                    Role = userModel.Role,
                    UserGroup = userModel.UserGroup,
                    ModifiedBy = string.IsNullOrEmpty(loggedInUser.UserName) ? "admin" : loggedInUser.UserName,
                };

                var cognitoUserData = await _cognitoService.CreateUser(user);
                user.Id = Guid.Parse(cognitoUserData.Attributes.Where(x=>x.Name.ToUpper() == "SUB").FirstOrDefault().Value);
                user.AdditionalData = JsonConvert.SerializeObject(cognitoUserData);
                user.Status = cognitoUserData.Enabled ? "ACTIVE" : "INACTIVE";

                var result = await _appDbContext.AddAsync(user);
                _ = await _appDbContext.SaveChangesAsync();

                var createdUserId = result.Entity.Id;
                userModel.PaygroupsAssigned.ForEach(p => _appDbContext.AddAsync(
                    new UserPaygroupAssignment {
                        ClientId = Convert.ToInt32(p.ClientId),
                        LegalEntityId = Convert.ToInt32(p.LegalEntityId),
                        PaygroupId = Convert.ToInt32(p.PaygroupId),
                        UserId = createdUserId,
                        Status = user.Status,
                        ModifiedBy = string.IsNullOrEmpty(loggedInUser.UserName) ? "admin" : loggedInUser.UserName,
                    })
                );

                _ = await _appDbContext.SaveChangesAsync();
                return result.Entity.ToUserResponseModel();
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                throw new InvalidOperationException(ex.Message);
            }
        }

        public virtual async Task<BaseResponseModel<bool>> DeleteUserAsync(Guid id, LoggedInUser loggedInUser)
        {
            var isSuccess = false;

            try
            {
                var user = await _appDbContext.users.Select(e => new User()
                {
                    Id = e.Id,
                    AdditionalData = e.AdditionalData
                }).Where(e => e.Id.Equals(id)).FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new InvalidOperationException($"User id {id} is not found in the database.");
                }

                if (!string.IsNullOrEmpty(user.AdditionalData))
                {
                    var awsUsername = JsonConvert.DeserializeObject<AwsUser>(user.AdditionalData);

                    await _cognitoService.DeleteUser(awsUsername.Username);
                }

                _appDbContext.Attach(user);
                _appDbContext.Remove(user);

                _ = await _appDbContext.userpaygroupassignment.Where(p => p.UserId == user.Id)
                    .ExecuteDeleteAsync();
    
                var result = await _appDbContext.SaveChangesAsync();

                isSuccess = result == 1;
                return new BaseResponseModel<bool>()
                {
                    Data = isSuccess,
                    Status = isSuccess,
                    Message = isSuccess
                                ? $"Record {id} is succcessfully deleted."
                                : $"Deletion failed for record id: {id}",
                    Errors = new List<string>(),
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Exception occurred while deleting the record {id}, Exception {ex}", id, ex);
                return new BaseResponseModel<bool>()
                {
                    Data = isSuccess,
                    Status = isSuccess,
                    Message = $"Deletion failed for record id: {id}",
                    Errors = new List<string>()
                    {
                        ex.Message
                    },
                };
            }
        }
        public virtual async Task<BaseResponseModel<bool>> DeleteUserPayGroupAsync(int id, LoggedInUser loggedInUser)
        {
            try
            {
                var result = await _appDbContext.userpaygroupassignment
                .Where(assignment => assignment.Id == id)
                .ExecuteDeleteAsync();

                if (result > 0)
                {
                    await _appDbContext.SaveChangesAsync();
                    return new BaseResponseModel<bool>
                    {
                        Data = true,
                        Status = true,
                        Message = "Pay Group assignment has been deleted.",
                        Errors = new List<string>()
                    };
                }
                else
                {
                    return new BaseResponseModel<bool>
                    {
                        Data = false,
                        Status = false,
                        Message = "Pay Group assignment that you are trying to delete no longer exists.",
                        Errors = new List<string>()
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<bool>
                {
                    Data = false,
                    Status = false,
                    Message = "Failed to delete Pay Group assignment.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public virtual async Task<UserPageModel> GetAllUser(int limit, int offset, LoggedInUser loggedInUser)
        {
            var allUsers = _appDbContext.users;
            int totalRecords = allUsers.Count();
            int totalPages = totalRecords / limit + (totalRecords % limit > 0 ? 1 : 0);
            List<UserResponseModel> userList = await allUsers.OrderBy(e => e.Email)
                .Skip(offset).Take(limit)
                .Select(e => e.ToUserResponseModel())
                .ToListAsync();

            return new UserPageModel(){
                data = userList,
                totalPages = totalPages,
                totalRecords = totalRecords
            };
        }

        public virtual UserPageModel SearchUsers(int limit, int offset, Dictionary<string, string> userSearchFilterParams, LoggedInUser loggedInUser)
        {
            List<UserResponseModel> allFilteredUsers = _appDbContext.users
                .AsEnumerable()
                .Filter(userSearchFilterParams)
                .OrderBy(e => e.Email)
                .Select(e => e.ToUserResponseModel())
                .ToList();

            int totalRecords = allFilteredUsers.Count();
            int totalPages = totalRecords / limit + (totalRecords % limit > 0 ? 1 : 0);

            return new UserPageModel(){
                data = allFilteredUsers.Skip(offset).Take(limit).ToList(),
                totalPages = totalPages,
                totalRecords = totalRecords
            };
        }

        public virtual async Task<UserResponseModel?> GetUser(Guid id, LoggedInUser loggedInUser)
        {
            // Todo: User access validation
            _logger.LogDebug("Get user called by {currentUser} to retrieve record for {userId}", loggedInUser.UserId, id);

            var result = await _appDbContext.users.Where(e => e.Id.Equals(id)).FirstOrDefaultAsync();
            var assignedPaygroups = from userpaygroupassignment in _appDbContext.userpaygroupassignment
                            join client in _appDbContext.client
                            on userpaygroupassignment.ClientId equals client.id
                            join legalentity in _appDbContext.legalentity
                            on userpaygroupassignment.LegalEntityId equals legalentity.id
                            join paygroup in _appDbContext.paygroup
                            on userpaygroupassignment.PaygroupId equals paygroup.id
                            where userpaygroupassignment.UserId.Equals(id)
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

        public virtual async Task<BaseResponseModel<UserResponseModel>> UpdateUser(string userId, UserModel userModel, LoggedInUser loggedInUser)
        {
            try
            {
                // Todo: validate usermodel before proceeding
                // Todo: User access validation
                // if (userModel.Id <= 0)
                // {
                //     throw new InvalidDataException("Entity Id does not exist in the request data");
                // }

                var modifiedBy = string.IsNullOrEmpty(loggedInUser.UserName) ? "admin" : loggedInUser.UserName;
                var record = _appDbContext.users.Where(e => e.Id.Equals(userModel.Id));

                var updateResult = await record.ExecuteUpdateAsync(b =>
                                        b.SetProperty(u => u.Email, userModel.Email)
                                         .SetProperty(u => u.UserId, userModel.UserId)
                                         .SetProperty(u => u.FirstName, userModel.FirstName)
                                         .SetProperty(u => u.LastName, userModel.LastName)
                                         .SetProperty(u => u.SecondLastName, userModel.SecondLastName)
                                         .SetProperty(u => u.MiddleName, userModel.MiddleName)
                                         .SetProperty(u => u.FullName, userModel.FullName)
                                         .SetProperty(u => u.Role, userModel.Role)
                                         .SetProperty(u => u.UserGroup, userModel.UserGroup)
                                         .SetProperty(u => u.ModifiedBy, modifiedBy)
                                         .SetProperty(u => u.Status, userModel.Status)
                                        );
                var output = await record.FirstOrDefaultAsync();

                await _cognitoService.UpdateUser(userId, output);

                if (userModel.PaygroupsAssigned.Count == 0)
                {
                    await _appDbContext.userpaygroupassignment.Where(p => p.UserId.Equals(userModel.Id))
                    .ForEachAsync(p => {
                        if (p != null)
                            _appDbContext.userpaygroupassignment.Remove(p);
                    });
                }
                else
                {
                    for (int i = 0; i < userModel.PaygroupsAssigned.Count; i++)
                    {
                        var paygroupAssigned = userModel.PaygroupsAssigned[i];

                        if (paygroupAssigned == null) continue;

                        if (paygroupAssigned.Id == null)
                        {
                            var assignment = new UserPaygroupAssignment
                            {
                                ClientId = Convert.ToInt32(paygroupAssigned.ClientId),
                                LegalEntityId = Convert.ToInt32(paygroupAssigned.LegalEntityId),
                                PaygroupId = Convert.ToInt32(paygroupAssigned.PaygroupId),
                                UserId = userModel.Id,
                                Status = paygroupAssigned.Status,
                                ModifiedBy = modifiedBy,
                            };
                            await _appDbContext.AddAsync(assignment);
                        }
                        else
                        {
                            var existingPaygroup = _appDbContext.userpaygroupassignment.Where(p => p.Id == paygroupAssigned.Id);

                            await existingPaygroup.ExecuteUpdateAsync(p =>
                                p.SetProperty(p => p.ClientId, Convert.ToInt32(paygroupAssigned.ClientId))
                                 .SetProperty(p => p.LegalEntityId, Convert.ToInt32(paygroupAssigned.LegalEntityId))
                                 .SetProperty(p => p.PaygroupId, Convert.ToInt32(paygroupAssigned.PaygroupId))
                                 .SetProperty(p => p.Status, paygroupAssigned.Status)
                                 .SetProperty(p => p.ModifiedBy, modifiedBy)
                            );
                        }
                    }
                }
                
                _ = await _appDbContext.SaveChangesAsync();

                return new BaseResponseModel<UserResponseModel>()
                {
                    Status = updateResult > 0,
                    Data = output.ToUserResponseModel(),
                    Message = "Record is successfully updated.",
                    Errors = new List<string>(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);

                return new BaseResponseModel<UserResponseModel>()
                {
                    Status = false,
                    Message = "Something went wront, please check the errors",
                    Errors = new List<string>()
                    {
                        ex.Message
                    },
                };
            }
        }

        public virtual async Task<BaseResponseModel<string?>> UpdateUserStatus(Guid id, string status, LoggedInUser loggedInUser)
        {
            try
            {
                var result = await _appDbContext.users
                .Where(u => u.Id.Equals(id))
                .ExecuteUpdateAsync(b => b.SetProperty(u => u.Status, status));
                if (result != 1)
                {
                    throw new InvalidOperationException($"No of rows were updated is not one, it was {result}");
                }

                return new BaseResponseModel<string?>()
                {
                    Status = true,
                    Message = "Record is updated sucessfully",
                    Data = null,
                    Errors = new List<string>(),
                };
            }
            catch (InvalidOperationException ex)
            {
                return new BaseResponseModel<string?>()
                {
                    Status = false,
                    Message = "Something went wrong, please check errors.",
                    Data = null,
                    Errors = new List<string>()
                    {
                        ex.Message,
                    },
                };
            }
        }

        public virtual async Task<BaseResponseModel<string?>> SetLastLoggedIn(LoggedInUser loggedInUser)
        {
            try
            {
                var result = await _appDbContext.users
                .Where(u => u.Id == loggedInUser.UserId)
                .ExecuteUpdateAsync(b => b.SetProperty(u => u.LastLoggedOn, _dateTimeHelper.GetDateTimeNow()));
                if (result != 1)
                {
                    throw new InvalidOperationException($"No of rows were updated is not one, it was {result}");
                }

                return new BaseResponseModel<string?>()
                {
                    Status = true,
                    Message = "Record is updated sucessfully",
                    Data = null,
                    Errors = new List<string>(),
                };
            }
            catch (InvalidOperationException ex)
            {
                return new BaseResponseModel<string?>()
                {
                    Status = false,
                    Message = "Something went wrong, please check errors.",
                    Data = null,
                    Errors = new List<string>()
                    {
                        ex.Message,
                    },
                };
            }
        }

        public virtual bool CanView(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return true;
        }
    }
}
