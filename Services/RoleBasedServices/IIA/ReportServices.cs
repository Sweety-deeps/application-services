using Microsoft.Extensions.Logging;
using Persistence;
using Domain.Entities;
using System.Text.Json;
using Domain.Models;
using Amazon.S3;
using Amazon.S3.Model;
using ClosedXML.Excel;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Npgsql;
using Services.Helpers;
using Domain.Models.Users;

namespace Services.IIA
{
    public class ReportServices : Services.PowerUser.ReportServices
    {
        public ReportServices(AppDbContext appDbContext, IAmazonS3 s3Client, ILogger<ReportServices> logger, ISelectListHelper selectListHelper, IDateTimeHelper dateTimeHelper, IReportServiceHelper reportServiceHelper, Config config)
        : base(appDbContext, s3Client, logger, selectListHelper, dateTimeHelper, reportServiceHelper, config)
        {
        }

        protected override List<CalendarResponseModel> GetCalendarResponseModel(LoggedInUser user, CalendarRequestModel _data)
        {
            var pglist = user.Paygroups.Select(x => x.payGroupId).ToList().Distinct();

            List<CalendarResponseModel> lstCalendarResponseModel = new List<CalendarResponseModel>();
            lstCalendarResponseModel = (from PC in _appDbContext.paycalendar
                                        join PG in _appDbContext.paygroup on PC.paygroupid equals PG.id
                                        where (PC.year == _data.year)
                                        && PG.code == _data.paygroup
                                        && pglist.Contains(PG.id)

                                        select new CalendarResponseModel()
                                        {
                                            Month = PC.months,
                                            PayPeriod = PC.payperiod,
                                            PeriodStartDate = PC.date,
                                            TaskName = PC.taskname
                                        }).Distinct().ToList();
            return lstCalendarResponseModel;
        }

        public override List<RequestHighLevelDetails> GetRequestHighLevelDetails(LoggedInUser user)
        {
            string sResponse = string.Empty;
            List<RequestHighLevelDetails> res = new List<RequestHighLevelDetails>();
            var pglist = user.Paygroups.Select(x => x.payGroupCode).ToList().Distinct();
            try
            {
                res = _appDbContext.requesthighleveldetails.Where(x => pglist.Contains(x.paygroup)).ToList().OrderByDescending(t => t.requestdetailsid).ToList();
                return res;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return res;
        }

        public override List<RequestLowLevelDetails> GetLowLevelDetails(LoggedInUser user)
        {
            string sResponse = string.Empty;
            var res = new List<RequestLowLevelDetails>();
            try
            {
                res = (from rld in _appDbContext.requestlowleveldetails
                       join rd in _appDbContext.requestdetails on rld.requestdetailsid equals rd.id
                       from pg in user.Paygroups
                       where rd.paygroup == pg.payGroupCode
                       orderby rld.requestdetailsid descending
                       select rld).ToList();
                return res;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return res;
        }

        public override string GetComparisonData(LoggedInUser user, int? iRequestID)
        {
            string sResponse = string.Empty;
            try
            {
                //TODO Arumugam
                var vResponse = (from s in _appDbContext.Set<StagingEmployee>().Where(x => x.requestid == iRequestID)
                                 join t in _appDbContext.Set<TransformedEmployee>().Where(y => y.requestid == iRequestID)
                                 on s.employeeid equals t.employeeid
                                 join rd in _appDbContext.Set<RequestDetails>() on s.requestid equals rd.id
                                 join pg in user.Paygroups on rd.paygroup equals pg.payGroupCode
                                 select new
                                 {
                                     semployeeid = s.employeeid,
                                     temployeeid = t.employeeid,
                                     semployeenumber = s.employeenumber,
                                     temployeenumber = t.employeenumber,
                                     spaygroup = s.paygroup,
                                     tpaygroup = t.paygroup,
                                     shiredate = s.hiredate,
                                     thiredate = t.hiredate,
                                     slastname = s.lastname,
                                     tlastname = t.lastname,
                                     ssecondlastname = s.secondlastname,
                                     tsecondlastname = t.secondlastname,
                                     sfirstname = s.firstname,
                                     tfirstname = t.firstname,
                                     smiddlenames = s.middlenames,
                                     tmiddlenames = t.middlenames,
                                     stitle = s.title,
                                     ttitle = t.title,
                                     sgender = s.gender,
                                     tgender = t.gender,
                                     sdateofbirth = s.dateofbirth,
                                     tdateofbirth = t.dateofbirth,
                                     sbirthcity = s.birthcity,
                                     tbirthcity = t.birthcity,
                                     sbirthcountry = s.birthcountry,
                                     tbirthcountry = t.birthcountry,
                                     sdateofleaving = s.dateofleaving,
                                     tdateofleaving = t.dateofleaving,
                                     sterminationreason = s.terminationreason,
                                     tterminationreason = t.terminationreason,
                                     ssenioritydate = s.senioritydate,
                                     tsenioritydate = t.senioritydate,
                                     smaritalstatus = s.maritalstatus,
                                     tmaritalstatus = t.maritalstatus,
                                     scontacttype = s.contacttype,
                                     sphone = s.phone,
                                     semail = s.email,
                                     sworkemail = s.workemail,
                                     tworkemail = t.workemail,
                                     spersonalemail = s.personalemail,
                                     tpersonalemail = t.personalemail,
                                     sworkphone = s.workphone,
                                     tworkphone = t.workphone,
                                     smobilephone = s.mobilephone,
                                     tmobilephone = t.mobilephone,
                                     shomephone = s.homephone,
                                     thomephone = t.homephone,
                                     screatedby = s.createdby,
                                     tcreatedby = t.createdby,
                                     screatedat = s.createdat,
                                     tcreatedat = t.createdat,
                                     smodifiedby = s.modifiedby,
                                     tmodifiedby = t.modifiedby,
                                     smodifiedat = s.modifiedat,
                                     tmodifiedat = t.modifiedat,
                                     sstatus = s.status,
                                     tstatus = t.status,
                                 }
                       ).ToList();
                sResponse = JsonSerializer.Serialize(vResponse).ToString();

            }
            catch (Exception ex)
            {
                //TODO Need to do error logging
                _logger.Log(LogLevel.Error, ex.ToString());
            }

            return sResponse;

        }
        public override string GetEmployeeDetails(LoggedInUser user, string iEmployeeID)
        {
            string sResponse = string.Empty;
            try
            {
                var vResponse = (from s in _appDbContext.Set<Employee>().Where(x => x.employeeid == iEmployeeID)
                                 join h in user.Paygroups on s.paygroupid equals h.payGroupId
                                 select new
                                 {
                                     semployeeid = s.employeeid,
                                     semployeenumber = s.employeenumber,
                                     paygroup = h.payGroupCode,
                                     shiredate = s.hiredate,
                                     slastname = s.lastname,
                                     ssecondlastname = s.secondlastname,
                                     sfirstname = s.firstname,
                                     smiddlenames = s.middlenames,
                                     stitle = s.title,
                                     sgender = s.gender,
                                     sdateofbirth = s.dateofbirth,
                                     sbirthcity = s.birthcity,
                                     sbirthcountry = s.birthcountry,
                                     sdateofleaving = s.dateofleaving,
                                     sterminationreason = s.terminationreason,
                                     ssenioritydate = s.senioritydate,
                                     smaritalstatus = s.maritalstatus,
                                     sworkemail = s.workemail,
                                     spersonalemail = s.personalemail,
                                     sworkphone = s.workphone,
                                     smobilephone = s.mobilephone,
                                     shomephone = s.homephone,
                                     screatedby = s.createdby,
                                     screatedat = s.createdat,
                                     smodifiedby = s.modifiedby,
                                     smodifiedat = s.modifiedat,
                                     sstatus = s.status,

                                 }
                       ).ToList();
                sResponse = JsonSerializer.Serialize(vResponse).ToString();

            }
            catch (Exception ex)
            {
                //TODO Need to do error logging
                _logger.Log(LogLevel.Error, ex.ToString());
            }

            return sResponse;

        }

        #region Download Reports

        public override async Task<List<PayPeriodRegisterResponseModel>> GetPayPeriodRegistersAsync(LoggedInUser user, PayPeriodRegisterRequestModel _data, int iPayGroupID)
        {
            if (user.Paygroups.Any(upa => upa.payGroupId == iPayGroupID))
            {
                return await base.GetPayPeriodRegistersAsync(user, _data, iPayGroupID);
            }
            throw new UnauthorizedAccessException();
        }

        public override async Task<List<PayPeriodRegisterDetailResponseModel>> GetPayPeriodDetailRegistersAsync(LoggedInUser user, PayPeriodRegisterRequestModel _data, int iPayGroupID)
        {
            if (user.Paygroups.Any(upa => upa.payGroupId == iPayGroupID))
            {
                return await base.GetPayPeriodDetailRegistersAsync(user, _data, iPayGroupID);
            }
            throw new UnauthorizedAccessException();
        }

        public override DateTime GetPayPeriodDate(LoggedInUser user, string strPayGroupCode, string strTaskName, int iPayPeriod)
        {
            if (user.Paygroups.Any(upa => upa.payGroupCode == strPayGroupCode))
            {
                return base.GetPayPeriodDate(user, strPayGroupCode, strTaskName, iPayPeriod);
            }

            throw new UnauthorizedAccessException();
        }

        public override List<HrDatawarehouseResponseModel> GetHrDatawarehouse(LoggedInUser user, HrDatawarehouseRequestModel _data)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == _data.paygroup).Any())
            {
                return base.GetHrDatawarehouse(user, _data);
            }
            throw new UnauthorizedAccessException();
        }


        public override CSAndCFDatawarehouseResponseModel GetCSPFDatawarehouse(LoggedInUser user, CSPFDatawarehouseRequestModel _data)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == _data.paygroup).Any())
            {
                return base.GetCSPFDatawarehouse(user, _data);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<PaydDatawarehouseResponseModel> GetPAYDDatawarehouse(LoggedInUser user, PaydDatawarehouseRequestModel _data)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == _data.paygroup).Any())
            {
                return base.GetPAYDDatawarehouse(user, _data);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<PayElementResponseModel> GetPayElementReport(LoggedInUser user, PayElementRequestModel _data)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == _data.paygroup).Any())
            {
                return base.GetPayElementReport(user, _data);
            }
            throw new UnauthorizedAccessException();
        }
        public override List<SystemUserResponseModel> GetSystemUserReport(LoggedInUser user, SystemUserRequestModel _data)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == _data.paygroup).Any())
            {
                return base.GetSystemUserReport(user, _data);
            }
            throw new UnauthorizedAccessException();
        }
        public override Task<ConfigurationResponseModel> GetConfigurationData(LoggedInUser user, ConfigurationRequestModel model)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == model.paygroup).Any())
            {
                return base.GetConfigurationData(user, model);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<TransactionResponseModel> GetTransactionByPayGroupReport(LoggedInUser user, TransactionRequestModel _data)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == _data.paygroup).Any())
            {
                return base.GetTransactionByPayGroupReport(user, _data);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<TransactionCountryResponseModel> GetTransactionByCountryReport(LoggedInUser user, TransactionCountryRequestModel _data)
        {

            var pglist = user.Paygroups.Select(x => x.payGroupCode).ToList().Distinct();
            return base.GetTransactionByCountryReport(user, _data).Where(x => pglist.Contains(x.PayGroup)).ToList();
        }

        public override List<VarianceResponseModel> GetVarianceReport(LoggedInUser user, VarianceRequestModel _data)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == _data.paygroup).Any())
            {
                return base.GetVarianceReport(user, _data);
            }
            throw new UnauthorizedAccessException();
        }
        public override List<PayPeriodWithPrevioudCutOff> GetPayPeriodCutoff(LoggedInUser user, int paygroupid, int year, int? payperiod)
        {
            if (user.Paygroups.Where(x => x.payGroupId == paygroupid).Any())
            {
                return base.GetPayPeriodCutoff(user, paygroupid, year, payperiod);
            }
            throw new UnauthorizedAccessException();
        }

        public override VariancePayPeriodDetails GetPreviousPayPeriod(LoggedInUser user, int iPayGroupID, int iPayPeriodYear)
        {
            if (user.Paygroups.Where(x => x.payGroupId == iPayGroupID).Any())
            {
                return base.GetPreviousPayPeriod(user, iPayGroupID, iPayPeriodYear);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<ErrorLogResponseModel> GetErrorLogReport(LoggedInUser user, ErrorLogRequestModel _data)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == _data.paygroup).Any())
            {
                return base.GetErrorLogReport(user, _data);
            }
            throw new UnauthorizedAccessException();
        }

        public override string GetPayFrequencyForPayGroup(LoggedInUser user, string PayGroupCode)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == PayGroupCode).Any())
            {
                return base.GetPayFrequencyForPayGroup(user, PayGroupCode);
            }
            throw new UnauthorizedAccessException();
        }


        public override Task<string> GetPeriodChangeFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == requestModel.PaygroupCode).Any())
            {
                return base.GetPeriodChangeFileAsync(user, requestModel, sheetName);
            }
            throw new UnauthorizedAccessException();
        }

        public override Task<string> GetCenamPeriodChangeFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == requestModel.PaygroupCode).Any())
            {
                return base.GetCenamPeriodChangeFileAsync(user, requestModel, sheetName);
            }
            throw new UnauthorizedAccessException();
        }

        public override Task<string> GetHybridChageFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == requestModel.PaygroupCode).Any())
            {
                return base.GetHybridChageFileAsync(user, requestModel, sheetName);
            }
            throw new UnauthorizedAccessException();
        }

        #endregion Download Reports

        #region S3 download

        #endregion S3

        public override string GetCountryByClientCode(LoggedInUser user, string paygroup)
        {
            if (user.Paygroups.Where(x => x.clientCode == paygroup).Any())
            {
                return base.GetCountryByClientCode(user, paygroup);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<int> GetPayPeriodsByCountryCode(LoggedInUser user, int? iCountryID)
        {
            try
            {
                var pglist = user.Paygroups.Select(x => x.payGroupId).ToList().Distinct();
                var _payPeriods = (from C in _appDbContext.Set<Client>()
                                   join LE in _appDbContext.Set<LegalEntity>() on C.id equals LE.clientid
                                   join pg in _appDbContext.Set<PayGroup>() on LE.id equals pg.legalentityid
                                   join pc in _appDbContext.Set<PayCalendar>() on pg.id equals pc.paygroupid
                                   where pg.countryid == iCountryID && pglist.Contains(pg.id)
                                   select pc.payperiod).Distinct().ToList();
                return _payPeriods;

            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public override List<int> GetPayPeriodsByYear(LoggedInUser user, int payGroupID, int year)
        {
            if (user.Paygroups.Where(x => x.payGroupId == payGroupID).Any())
            {
                return base.GetPayPeriodsByYear(user, payGroupID, year);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<int> GetPayPeriodsByPayGroupCode(LoggedInUser user, string strPayGroupCode)
        {
            if (user.Paygroups.Where(x => x.payGroupCode == strPayGroupCode).Any())
            {
                return base.GetPayPeriodsByPayGroupCode(user, strPayGroupCode);
            }
            throw new UnauthorizedAccessException();
        }

        public override int GetPayGroupIDByCode(LoggedInUser user, string strCode)
        {
            var response = (from p in user.Paygroups
                            where p.payGroupCode == strCode
                            select p.payGroupId).FirstOrDefault();

            return response;
        }

        public override PeriodChangeFileRequestModel SetStartandEndDateBasedonPayperiod(LoggedInUser user, PeriodChangeFileRequestModel _req)
        {
            if (user.Paygroups.Where(x => x.payGroupId == _req.paygroupid).Any())
            {
                return base.SetStartandEndDateBasedonPayperiod(user, _req);
            }
            throw new UnauthorizedAccessException();
        }

        public override PayCalendar GetPayCalendarByPayPeriod(LoggedInUser user, int payperiod, int paygroupid, int year)
        {
            if (user.Paygroups.Where(x => x.payGroupId == paygroupid).Any())
            {
                return base.GetPayCalendarByPayPeriod(user, payperiod, paygroupid, year);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
