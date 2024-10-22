using System.Data.SqlClient;
using System.Security.Claims;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Persistence;
using Services.Abstractions;

namespace Services
{
    public class LoggedInUserService : ILoggedInUserService
    {
        protected readonly ILogger<PayGroupServices> _logger;
        protected readonly AppDbContext _appDbContext;
        private readonly LocalCacheService _cacheService;
        public LoggedInUserService(AppDbContext appDbContext, ILogger<PayGroupServices> logger, LocalCacheService cacheService)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _cacheService = cacheService;
        }

        public virtual LoggedInUser GetLoggedInUser(ClaimsPrincipal loggedInUserClaims)
        {
            var userId = Guid.Parse(loggedInUserClaims.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault()?.Value);
            var email = loggedInUserClaims.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault()?.Value;
            var roleString = loggedInUserClaims.Claims.Where(x => x.Type == "custom:role").FirstOrDefault()?.Value;
            var userName = loggedInUserClaims.Claims.Where(x => x.Type == "preferred_username").FirstOrDefault()?.Value;
            Role role;

            switch (roleString)
            {
                case "CAM":
                    role = Role.CAM;
                    break;
                case "country_manager":
                    role = Role.country_manager;
                    break;
                case "Interface_oa":
                    role = Role.Interface_oa;
                    break;
                case "interface_ic":
                    role = Role.interface_ic;
                    break;
                case "poweruser":
                    role = Role.poweruser;
                    break;
                case "superuser":
                    role = Role.superuser;
                    break;
                default:
                    role = Role.unauthorized;
                    break;
            }

            return new LoggedInUser(userId, email, role, userName, this.GetPayGroupAssignments(userId));
        }

        private IEnumerable<PayGroupCacheModel> GetPayGroupAssignments(Guid userId)
        {
            var cachedList = _cacheService.GetList<PayGroupCacheModel>(userId);

            if (cachedList == null)
            {
                var query = (from P in _appDbContext.paygroup
                             join UP in _appDbContext.Set<UserPaygroupAssignment>() on P.id equals UP.PaygroupId
                             join L in _appDbContext.Set<LegalEntity>() on P.legalentityid equals L.id
                             join C in _appDbContext.Set<Client>() on L.clientid equals C.id
                             where UP.UserId.Equals(userId)
                             select new PayGroupCacheModel
                             {
                                 payGroupId = P.id,
                                 payGroupCode = P.code,
                                 name = P.name,
                                 countryId = P.countryid,
                                 legalEntityId = L.id,
                                 legalentitycode = L.code,
                                 clientId = C.id,
                                 clientCode = C.code,
                                 status = C.status,
                                 payfrequencyid = P.payfrequencyid,
                             }).OrderBy(e => e.payGroupCode);

                // If list not found in cache, fetch it from the data source
                var list = query.ToList<PayGroupCacheModel>();

                // Store list in cache for 5 minutes
                _cacheService.SetList<PayGroupCacheModel>(userId, list, TimeSpan.FromMinutes(5));

                cachedList = list;
            }

            return cachedList;
        }
    }
}

