using System.Data.SqlClient;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services.IIA
{
    public class ClientServices : Services.ClientServices
    {
        public ClientServices(AppDbContext appDbContext, ILogger<ClientServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {
        }

        public override Task<DatabaseResponse> DeleteClient(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public override async Task<List<ClientModel>> GetClient(LoggedInUser user)
        {
            var clientList = new List<ClientModel>();
            try
            {
                List<int> clientsByPGAssignment = user.Paygroups.Select(x => x.clientId).Distinct().ToList();

                var query = (from q in _appDbContext.Set<Client>()
                             join p in _appDbContext.Set<Provider>() on q.providerid equals p.id into jts
                             from pResult in jts.DefaultIfEmpty()
                             where clientsByPGAssignment.Contains(q.id)
                             select new ClientModel()
                             {
                                 id = q.id,
                                 code = q.code,
                                 name = q.name,
                                 providercode = pResult != null ? pResult.code : "",
                                 providerid = pResult != null ? pResult.id : 0,
                                 createdby = q.createdby,
                                 createdat = q.createdat,
                                 modifiedby = q.modifiedby,
                                 modifiedat = q.modifiedat,
                                 status = q.status,

                             }).OrderByDescending(a => a.id);

                clientList = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return clientList;
        }

        protected override async Task<List<PaygroupMinimalModel>> GetPaygroupMinimalModels(LoggedInUser user, int clientId)
        {
            try
            {

                var paygroupMinimalmodel = user.Paygroups.Where(x => x.clientId == clientId).Select(pg => new PaygroupMinimalModel()
                {
                    Id = pg.payGroupId,
                    Code = pg.payGroupCode,
                    Name = pg.name
                }).ToList();

                return paygroupMinimalmodel;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return new List<PaygroupMinimalModel>(); // Return an empty list or handle the exception as needed
            }
        }

        public override Task<List<PaygroupMinimalModel>> GetPayGroupByClient(LoggedInUser user, int clientid)
        {
            List<int> clientsByPGAssignment = user.Paygroups.Select(x => x.clientId).Distinct().ToList();
            if (clientsByPGAssignment.Contains(clientid))
            {
                return base.GetPayGroupByClient(user, clientid);
            }

            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> InsertClient(LoggedInUser user, ClientModel clientModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> UpdateClient(LoggedInUser user, ClientModel clientModel)
        {
            List<int> clientsByPGAssignment = user.Paygroups.Select(x => x.clientId).Distinct().ToList();
            if (clientsByPGAssignment.Contains(clientModel.id))
            {
                return base.UpdateClient(user, clientModel);
            }

            throw new UnauthorizedAccessException();

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

