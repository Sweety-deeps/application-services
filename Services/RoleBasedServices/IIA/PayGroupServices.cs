using Domain.Entities;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;

namespace Services.IIA
{
    public class PayGroupServices : Services.PowerUser.PayGroupServices
    {
        public PayGroupServices(AppDbContext appDbContext, ILogger<PayGroupServices> logger, IDateTimeHelper dateTimeHelper, IEncrytionHelper encrytionHelper) : base(appDbContext, logger, dateTimeHelper, encrytionHelper)
        {
        }

        public override async Task<List<PaygroupMinimalModel>> GetActivePaygroups(LoggedInUser user)
        {
            var query = (from P in _appDbContext.paygroup
                         join UP in _appDbContext.Set<UserPaygroupAssignment>() on P.id equals UP.PaygroupId
                         where UP.UserId.Equals(user.UserId) && P.status != null && P.status.ToLower() == "active" && UP.Status.ToLower() == "active"
                         select new PaygroupMinimalModel { Id = P.id, Code = P.code, Name = P.name }).OrderBy(e => e.Code);

            return await query.ToListAsync();
        }

        public override async Task<List<PayGroupModel>> GetPayGroupDetails(LoggedInUser user)
        {
            var lstPayGroupModel = new List<PayGroupModel>();
            try
            {
                lstPayGroupModel = await (from P in _appDbContext.Set<PayGroup>()
                                          join UP in _appDbContext.Set<UserPaygroupAssignment>() on P.id equals UP.PaygroupId
                                          where UP.UserId.Equals(user.UserId)
                                          join L in _appDbContext.Set<LegalEntity>() on P.legalentityid equals L.id into jls
                                          from LResult in jls.DefaultIfEmpty()
                                          join PF in _appDbContext.Set<PayFrequency>() on P.payfrequencyid equals PF.id into jps
                                          from PFResult in jps.DefaultIfEmpty()
                                          join C in _appDbContext.Set<Country>() on P.countryid equals C.id into jcs
                                          from CResult in jcs.DefaultIfEmpty()
                                          select new PayGroupModel()
                                          {
                                              id = P.id,
                                              name = P.name,
                                              code = P.code,
                                              payfrequencyid = PFResult != null ? PFResult.id : 0,
                                              payfrequencycode = PFResult != null ? PFResult.code : "",
                                              legalentityid = LResult != null ? LResult.id : 0,
                                              legalentitycode = LResult != null ? LResult.code : "",
                                              countryid = CResult != null ? CResult.id : 0,
                                              countrycode = CResult != null ? CResult.code : "",
                                              emailto = P.emailto,
                                              emailcc = P.emailcc,
                                              emailsubject = P.emailsubject,
                                              createdat = P.createdat,
                                              createdby = P.createdby,
                                              modifiedat = P.modifiedat,
                                              modifiedby = P.modifiedby,
                                              status = P.status.Trim(),
                                              outboundformat = P.outboundformat,
                                              gpri_sftp_folder = P.GpriSftpFolder,
                                              payslip_sftp_folder = P.PayslipSftpFolder,
                                              collectchanges = P.collectchanges ? "Yes": "No"

                                          })
                                    .OrderByDescending(a => a.id).ToListAsync();

                return lstPayGroupModel;

            }
            catch (Exception ex)
            {
                //TODO Need to do error logging
                return lstPayGroupModel;
            }
        }

        public override DatabaseResponse AddPayGroup(LoggedInUser user, PayGroupModel paygroupModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse DeletePayGroup(LoggedInUser user, int id)
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
