using System.Data.SqlClient;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services.PowerUser
{
    public class ClientServices : Services.ClientServices
    {
        public ClientServices(AppDbContext appDbContext, ILogger<ClientServices> logger,IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {
        }

        public override Task<DatabaseResponse> DeleteClient(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> InsertClient(LoggedInUser user, ClientModel clientModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> UpdateClient(LoggedInUser user, ClientModel clientModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public override bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }
    }
}

