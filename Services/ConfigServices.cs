using Domain.Entities;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;

namespace Services
{
    public class ConfigServices : IConfigServices
    {
        protected readonly AppDbContext _appDbContext;
        protected readonly ILogger<ConfigServices> _logger;

        public ConfigServices(AppDbContext appDbContext, ILogger<ConfigServices> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }
        public virtual async Task<List<RequestDetails>> GetRequestDetailsAsync(LoggedInUser user)
        {
            List<RequestDetails> res = new List<RequestDetails>();
            try
            {
                res = await _appDbContext.requestdetails.OrderByDescending(t => t.id).ToListAsync();
                return res;

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
                return null;
            }
        }

        public virtual async Task<List<ConfigRequestType>> GetConfigRequestTypeAsync(LoggedInUser user)
        {
            List<ConfigRequestType> res = new List<ConfigRequestType>();
            try
            {
                res = await _appDbContext.configrequesttype.Where(t => t.status == "Active").ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
                return null;
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