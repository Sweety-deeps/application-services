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


namespace Services.CAM
{
    public class UserService : Services.IOA.UserService
    {

        public UserService(ILogger<UserService> logger,
            AppDbContext appDbContext, ICognitoService cognitoService, IDateTimeHelper dateTimeHelper) : base(logger, appDbContext, cognitoService, dateTimeHelper)
        {
        }

        protected override List<String> GetAccessibleRoles()
        {
            return new List<String> { Role.document_manager.ToString() };
        }

        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }
    }
}
