using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services.IIA
{
    public class LegalEntityServices : Services.LegalEntityServices
    {
        public LegalEntityServices(AppDbContext appDbContext, ILogger<LegalEntityServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {
        }

        public async override Task<List<LegalEntityBaseModel>> GetLegalEntityBaseDetails(LoggedInUser user)
        {
            var _legalentity = new List<LegalEntityBaseModel>();
            try
            {
                var query = from P in _appDbContext.Set<PayGroup>()
                            join UP in _appDbContext.Set<UserPaygroupAssignment>() on P.id equals UP.PaygroupId
                            join l in _appDbContext.Set<LegalEntity>() on P.legalentityid equals l.id
                            join c in _appDbContext.Set<Client>() on l.clientid equals c.id
                            where UP.UserId.Equals(user.UserId)
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

        public override async Task<List<LegalEntityModel>> GetLegalEntityDetails(LoggedInUser user)
        {
            var legalentityList = new List<LegalEntityModel>();
            try
            {
                legalentityList = await (from q in _appDbContext.Set<LegalEntity>()
                                         join pg in (from P in _appDbContext.Set<PayGroup>()
                                                     join UP in _appDbContext.Set<UserPaygroupAssignment>() on P.id equals UP.PaygroupId
                                                     where UP.UserId.Equals(user.UserId)
                                                     select P
                                                    ) on q.id equals pg.legalentityid
                                         into jts
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

        public override Task<DatabaseResponse> InsertLegalEntity(LoggedInUser user, LegalEntityModel legalEntityModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> UpdateLegalEntity(LoggedInUser user, LegalEntityModel legalEntityModel)
        {
            if (user.Paygroups.Any(upa => upa.legalEntityId == legalEntityModel.id))
            {
                return base.UpdateLegalEntity(user, legalEntityModel);
            }

            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> DeleteLegalEntity(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public override bool CanEdit(LoggedInUser user)
        {
            return true;
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
