using System.Data.SqlClient;
using DocumentFormat.OpenXml.Office2010.Excel;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services
{
    public class ClientServices : IClientServices
    {
        protected readonly ILogger<ClientServices> _logger;
        protected readonly AppDbContext _appDbContext;
        private readonly IDateTimeHelper _dateTimeHelper;

        public ClientServices(AppDbContext appDbContext, ILogger<ClientServices> logger, IDateTimeHelper dateTimeHelper)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _dateTimeHelper = dateTimeHelper;

        }
        public virtual async Task<List<ClientModel>> GetClient(LoggedInUser user)
        {
            var clientList = new List<ClientModel>();
            try
            {
                var query = (from q in _appDbContext.Set<Client>()

                             join p in _appDbContext.Set<Provider>() on q.providerid equals p.id into jts
                             from pResult in jts.DefaultIfEmpty()

                                 //where pResult.status == "Active"

                                 //orderby pResult.code descending

                             select new ClientModel()
                             {
                                 id = q.id,
                                 code = q.code,
                                 name = q.name,
                                 providercode = pResult != null ? pResult.code : "",
                                 providerid = pResult != null ? pResult.id : 0,
                                 nooflegalentity = 0,
                                 noofpaygroup = 0,
                                 createdby = q.createdby,
                                 createdat = q.createdat,
                                 modifiedby = q.modifiedby,
                                 modifiedat = q.modifiedat,
                                 status = q.status,

                             }).OrderByDescending(a => a.id);

                clientList = await query.ToListAsync();
                if (clientList != null && clientList.Any())
                {
                    foreach (var item in clientList)
                    {
                        item.nooflegalentity = GetNoOfLegalentityForClient(item.id);
                        item.noofpaygroup = GetNoOfPayGroupForClient(item.id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return clientList;
        }

        public virtual async Task<DatabaseResponse> InsertClient(LoggedInUser user, ClientModel clientModel)
        {
            var response = new DatabaseResponse();
            try
            {
                if (clientModel != null)
                {
                    if (clientModel.code != null && clientModel.name != null)
                    {
                        var clientCode = clientModel.code.Trim().ToLower();
                        var recordExists = await _appDbContext.client.AnyAsync(t => t.code.Trim().ToLower() == clientCode);

                        if (recordExists)
                        {
                            response.status = false;
                            response.message = "Code already exist";
                            return response;

                        }

                        var client = new Client
                        {
                            code = clientModel.code,
                            name = clientModel.name,
                            providerid = clientModel.providerid,
                            createdat = _dateTimeHelper.GetDateTimeNow(),
                            status = clientModel.status,
                            modifiedby = clientModel.modifiedby,
                            modifiedat = null,
                            createdby = user.UserName,
                        };

                        await _appDbContext.Set<Client>().AddAsync(client);
                        await _appDbContext.SaveChangesAsync();

                        response.status = true;
                        response.message = "Client is added successfully";
                        return response;
                    }
                    else
                    {
                        response.status = false;
                        response.message = "Missing mandatory fields, Code or Name is empty";
                        return response;

                    }
                }
                else
                {
                    response.status = false;
                    response.message = "Received empty Client details";
                    return response;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                response.status = false;
                response.message = "Something went wrong, please check the logs.";
                return response;
            }
        }

        public virtual async Task<DatabaseResponse> UpdateClient(LoggedInUser user, ClientModel clientModel)
        {
            var response = new DatabaseResponse();

            if (clientModel == null)
            {
                response.status = false;
                response.message = "Received empty Client details";
                return response;
            }


            if (clientModel.id > 0)
            {
                response.status = false;
                response.message = "Something went wront, please check logs";

                var isClientInactive = clientModel.status?.ToLower() == "inactive";
                var strategy = _appDbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _appDbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var clientRecord = _appDbContext.client.Where(t => t.id == clientModel.id);

                        clientRecord.ExecuteUpdate(p => p
                                            .SetProperty(s => s.name, clientModel.name)
                                            .SetProperty(s => s.providerid, clientModel.providerid)
                                            .SetProperty(s => s.modifiedat, _dateTimeHelper.GetDateTimeNow())
                                            .SetProperty(s => s.modifiedby, user.UserName)
                                            .SetProperty(s => s.status, clientModel.status));

                        if (isClientInactive)
                        {
                            _appDbContext.legalentity
                                    .Where(l => l.clientid == clientModel.id)
                                    .ExecuteUpdate(p => p
                                        .SetProperty(s => s.status, clientModel.status));

                            var query = @"update dbo.paygroup as p set status = @clientstatus
                                                from (select l.id from dbo.legalentity l where l.clientid = @clientid) as subquery
                                                where p.legalentityid = subquery.id";

                            var clientStatus = new NpgsqlParameter("@clientstatus", clientModel.status);
                            var clientId = new NpgsqlParameter("@clientid", clientModel.id);

                            _appDbContext.Database.ExecuteSqlRaw(query, new[] { clientStatus, clientId });
                        }

                        await transaction.CommitAsync();
                        response.status = true;
                        response.message = "Records updated successfully";
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();

                        _logger.LogError("{ex}", ex);
                        response.message = "Something went wrong, please check the logs";
                    }
                });

                return response;
            }
            else
            {
                response.status = false;
                response.message = "Unable to update client, request parameter is invalid.";
                return response;
            }
        }

        public virtual async Task<DatabaseResponse> DeleteClient(LoggedInUser user, int id)
        {
            var responseText = "This clientid is associated with a records in";
            DatabaseResponse response = new DatabaseResponse();
            try
            {
                var _client = _appDbContext.client.Where(s => s.id == id);

                var _legalEntity = _appDbContext.legalentity.Where(s => s.clientid == id);
                //var _provider = _appDbContext.provider.Where(s => s.clientid == id);
                //var _provider = "";
                if (_legalEntity.Any())
                {
                    response.status = false;
                    responseText += " Legal Entity";

                }
                else if (_client.Any())
                {
                    _appDbContext.client.RemoveRange(_client);
                    await _appDbContext.SaveChangesAsync();

                    response.status = true;
                    response.message = "Deleted Succesfully";
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            response.status = false;
            response.message = responseText;
            return response;
        }

        protected virtual async Task<List<PaygroupMinimalModel>> GetPaygroupMinimalModels(LoggedInUser user, int clientId)
        {
            return await (from p in _appDbContext.Set<PayGroup>()
                          join l in _appDbContext.Set<LegalEntity>() on p.legalentityid equals l.id
                          where l.clientid == clientId
                          select new PaygroupMinimalModel()
                          {
                              Id = p.id,
                              Code = p.code,
                              Name = p.name
                          })
                             .OrderByDescending(a => a.Id)
                             .ToListAsync();
        }

        public virtual async Task<List<PaygroupMinimalModel>> GetPayGroupByClient(LoggedInUser user, int clientId)
        {
            var paygroupList = new List<PaygroupMinimalModel>();
            try
            {
                paygroupList = await GetPaygroupMinimalModels(user, clientId);

                foreach (var paygroup in paygroupList)
                {
                    var distinctYears = await _appDbContext.Set<PayCalendar>()
                        .Where(pc => pc.paygroupid == paygroup.Id)
                        .Select(pc => pc.year)
                        .Distinct()
                        .ToListAsync();

                    paygroup.Years = distinctYears;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return paygroupList;
        }
        public int GetNoOfLegalentityForClient(int id)
        {
            int nooflegalentity = 0;
            try
            {
                nooflegalentity = _appDbContext.legalentity.Count(t => t.clientid == id);
            }
            catch (Exception ex)
            {
                nooflegalentity = 0;
            }
            
            return nooflegalentity;
        }

        public int GetNoOfPayGroupForClient(int id)
        {
            int noofpaygroup = 0;
            try
            {
                noofpaygroup = (from pg in _appDbContext.Set<PayGroup>()
                                join le in _appDbContext.Set<LegalEntity>() on pg.legalentityid equals le.id
                                where le.clientid == id
                                select pg).Count();
            }
            catch (Exception ex)
            {
                noofpaygroup = 0;
            }
            return noofpaygroup;
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

