using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services
{
    public class LegalEntityServices : ILegalEntityServices
    {
        protected readonly ILogger<LegalEntityServices> _logger;
        protected readonly AppDbContext _appDbContext;
        private readonly IDateTimeHelper _dateTimeHelper;

        public LegalEntityServices(AppDbContext appDbContext, ILogger<LegalEntityServices> logger, IDateTimeHelper dateTimeHelper)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _dateTimeHelper = dateTimeHelper;
        }

        // Do we need the following method? What is the difference between this once and get legal entity based details? 
        public virtual async Task<List<LegalEntityModel>> GetLegalEntityDetails(LoggedInUser user)
        {
            var legalentityList = new List<LegalEntityModel>();
            try
            {
                legalentityList = await (from q in _appDbContext.Set<LegalEntity>()

                                join pg in _appDbContext.Set<PayGroup>() on q.id equals pg.legalentityid into jts
                                from pgResult in jts.DefaultIfEmpty()

                                join p in _appDbContext.Set<Client>() on q.clientid equals p.id into jrs
                                from cResult in jrs.DefaultIfEmpty()

                                select new LegalEntityModel()
                                {
                                    id = q.id,
                                    code = q.code,
                                    name = q.name,
                                    clientcode = cResult != null ? cResult.code : "",
                                    clientid = cResult != null ? cResult.id : 0,
                                    paygroupid = pgResult != null ? pgResult.id : 0,
                                    noofpaygroup = 0,
                                    createdby = q.createdby,
                                    createdat = q.createdat,
                                    modifiedby = q.modifiedby,
                                    modifiedat = q.modifiedat,
                                    status = q.status,

                                })
                                .Distinct()
                                .OrderByDescending(e => e.id)
                                .ToListAsync();

                foreach (var item in legalentityList)
                {
                    item.noofpaygroup = GetNoOfPaygroupsForLegalEntity(item.id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return legalentityList;
        }

        public virtual async Task<List<LegalEntityBaseModel>> GetLegalEntityBaseDetails(LoggedInUser user)
        {
            var _legalentity = new List<LegalEntityBaseModel>();
            try
            {
                var query = from l in _appDbContext.Set<LegalEntity>()
                            join c in _appDbContext.Set<Client>() on l.clientid equals c.id
                            select new LegalEntityBaseModel()
                            {
                                id = l.id,
                                code = l.code,
                                name = l.name,
                                clientcode = c.code,
                                clientid = l.clientid,
                                noofpaygroup = (from p in _appDbContext.Set<PayGroup>()
                                                where p.legalentityid == l.id
                                                select p.id).Count(),
                                createdby = l.createdby,
                                createdat = l.createdat,
                                modifiedby = l.modifiedby,
                                modifiedat = l.modifiedat,
                                status = l.status,
                            };
                _legalentity = await query.Distinct().OrderByDescending(e => e.id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _legalentity;
        }

        public virtual async Task<DatabaseResponse> InsertLegalEntity(LoggedInUser user, LegalEntityModel legalEntityModel)
        {
            var response = new DatabaseResponse();
            try
            {
                if (legalEntityModel == null)
                {
                    response.status = false;
                    response.message = "Received empty Client details";
                    return response;
                }

                if (legalEntityModel.code == null || legalEntityModel.name == null)
                {
                    response.status = false;
                    response.message = "Received blank/empty code or name received";
                    return response;
                }

                var recordExists = await _appDbContext.legalentity.AnyAsync(t => t.code.Trim().ToLower() == legalEntityModel.code.Trim().ToLower());

                if (recordExists)
                {
                    response.status = false;
                    response.message = "Code already exist";
                    return response;
                }

                var legalEntity = new LegalEntity
                {
                    code = legalEntityModel.code,
                    name = legalEntityModel.name,
                    clientid = legalEntityModel.clientid,
                    createdat = _dateTimeHelper.GetDateTimeNow(),
                    status = legalEntityModel.status,
                    modifiedby = legalEntityModel.modifiedby,
                    modifiedat = null,
                    createdby = user.UserName
                };

                await _appDbContext.Set<LegalEntity>().AddAsync(legalEntity);
                await _appDbContext.SaveChangesAsync();

                response.status = true;
                response.message = "Added Successfully";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                response.status = false;
                response.message = "Something went wrong, please check the logs.";
                return response;
            }
        }

        public virtual async Task<DatabaseResponse> UpdateLegalEntity(LoggedInUser user, LegalEntityModel legalEntityModel)
        {
            var response = new DatabaseResponse();

            if (legalEntityModel == null)
            {
                response.status = false;
                response.message = "Received empty Provider details";
                return response;
            }

            if (legalEntityModel.id > 0)
            {
                var isLegalEntityInactive = legalEntityModel.status?.ToLower() == "inactive";
                var strategy = _appDbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _appDbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var legalEntityRecord = _appDbContext.legalentity.Where(l => l.id == legalEntityModel.id);

                        legalEntityRecord.ExecuteUpdate(p => p
                                            .SetProperty(s => s.code, legalEntityModel.code)
                                            .SetProperty(s => s.name, legalEntityModel.name)
                                            .SetProperty(s => s.clientid, legalEntityModel.clientid)
                                            .SetProperty(s => s.modifiedby, user.UserName)
                                            .SetProperty(s => s.modifiedat, _dateTimeHelper.GetDateTimeNow())
                                            .SetProperty(s => s.status, legalEntityModel.status));

                        if (isLegalEntityInactive)
                        {
                            _appDbContext.paygroup
                                    .Where(p => p.legalentityid == legalEntityModel.id)
                                    .ExecuteUpdate(p => p
                                        .SetProperty(s => s.status, legalEntityModel.status));
                        }

                        await transaction.CommitAsync();
                        response.status = true;
                        response.message = "Record updated successfully";
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError("{ex}", ex);
                        response.status = false;
                        response.message = "Record update is failed.";

                        await transaction.RollbackAsync();
                    }
                });

                return response;
            }
            else
            {
                response.status = false;
                response.message = "Unable to update legal entity, request parameter is invalid.";
                return response;
            }
        }

        public virtual async Task<DatabaseResponse> DeleteLegalEntity(LoggedInUser user, int id)
        {
            var responseText = "This LegalEntity is associated with ";
            var response = new DatabaseResponse();
            try
            {
                var paygroupExists = await _appDbContext.paygroup.AnyAsync(s => s.legalentityid == id);

                if (paygroupExists)
                {
                    response.status = false;
                    responseText += " Pay Group";
                }

                var legalEntity = _appDbContext.legalentity.Where(s => s.id == id);
                await legalEntity.ExecuteDeleteAsync();

                response.status = true;
                response.message = "Deleted Succesfully";
                return response;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }

            response.status = false;
            response.message = responseText;
            return response;
        }

        public int GetNoOfPaygroupsForLegalEntity(int id)
        {
            int noOfPaygroups = 0;
            try
            {
                noOfPaygroups = _appDbContext.paygroup.Where(t => t.legalentityid == id).Count();
            }
            catch (Exception ex)
            {

            }
            return noOfPaygroups;
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
