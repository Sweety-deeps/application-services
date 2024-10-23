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

namespace Services.CM
{
    public class ClientServices : Services.IOA.ClientServices
    {
        public ClientServices(AppDbContext appDbContext, ILogger<ClientServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {
        }
    }
}

