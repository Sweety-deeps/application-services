using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;

namespace Services.PowerUser
{
    public class ConfigServices : Services.ConfigServices
    {
        public ConfigServices(AppDbContext appDbContext, ILogger<ConfigServices> logger) : base(appDbContext, logger)
        {
        }
    }
}