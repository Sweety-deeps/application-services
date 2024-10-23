using Domain.Entities;
using Domain.Entities.Users;
using Domain.Models;
using Domain.Models.Users;
using Domain.Models.Users.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Persistence;
using Services.Helpers;
using Services.Users;


namespace Services.IIA
{
    public class UserService : Services.CM.UserService
    {

        public UserService(ILogger<UserService> logger,
            AppDbContext appDbContext, ICognitoService cognitoService, IDateTimeHelper dateTimeHelper) : base(logger, appDbContext, cognitoService, dateTimeHelper)
        {
        }
        protected override List<String> GetAccessibleRoles()
        {
            return new List<String> { Role.Interface_oa.ToString(), Role.CAM.ToString(), Role.interface_ic.ToString(), Role.document_manager.ToString() };
        }

        protected override bool CanEditUser(UserResponseModel userInDB, UserModel modifiedUser) {
            return modifiedUser.Email == userInDB.Email &&
                    modifiedUser.FirstName == userInDB.FirstName &&
                    modifiedUser.FullName == userInDB.FullName &&
                    modifiedUser.LastName == userInDB.LastName &&
                    modifiedUser.MiddleName == userInDB.MiddleName &&
                    modifiedUser.Role == userInDB.Role &&
                    modifiedUser.SecondLastName == userInDB.SecondLastName &&
                    modifiedUser.Status == userInDB.Status &&
                    modifiedUser.UserGroup == userInDB.UserGroup &&
                    modifiedUser.UserId == userInDB.UserId;
        }
        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }
    }
}
