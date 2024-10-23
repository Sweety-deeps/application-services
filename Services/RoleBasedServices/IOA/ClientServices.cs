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

namespace Services.IOA
{
    public class ClientServices : Services.IIA.ClientServices
    {
        public ClientServices(AppDbContext appDbContext, ILogger<ClientServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {
        }

        public override Task<DatabaseResponse> UpdateClient(LoggedInUser user, ClientModel clientModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override bool CanEdit(LoggedInUser user)
        {
            return false;
        }
    }
}

