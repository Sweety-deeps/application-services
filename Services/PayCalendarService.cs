using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;
using System.Data;

namespace Services
{
    public class PayCalendarService : IPayCalendarService
    {
        protected readonly ILogger<PayCalendarService> _logger;
        protected readonly AppDbContext _appDbContext;
        protected readonly Dictionary<string, IDataImportService> _dataImportService;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public List<string> PayCalendarHeaders = new List<string> {
            "PayGroupXrefCode",
            "Task ID",
            "Task Name",
            "Local Task Name",
            "Date",
            "Cut-Off Hour",
            "Year",
            "PayPeriod",
            "Month",
            "Frequency",
        };
        public List<string> IncomingPayCalendarHeaders = new List<string>();
        public string RowErrorMsg = "";
        public List<string> Frequency = new List<string> { "Monthly", "Bi-Weekly", "Fornightly", "Weekly" };
        public List<string> Months = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        public PayCalendarService(AppDbContext appDbContext, ILogger<PayCalendarService> logger, IEnumerable<IDataImportService> dataImportService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _dataImportService = dataImportService.ToDictionary(service => service.GetType().Namespace);
            _loggedInUserRoleService = loggedInUserRoleService;
            _dateTimeHelper = dateTimeHelper;
        }

        public virtual DatabaseResponse AddPayCalendar(LoggedInUser user, PayCalendarModel payCalendarModel)
        {
            DatabaseResponse response = new DatabaseResponse();
            try
            {
                if (payCalendarModel != null)
                {
                    PayCalendarModel model = new PayCalendarModel();
                    if (payCalendarModel.paygroupid != null)
                    {
                        List<PayCalendarModel> lstPayCalendarModel = new List<PayCalendarModel>();

                        ////lstPayCalendarModel = GetPayCalendar();
                        var res = _appDbContext.paycalendar.Where(t => t.taskid == payCalendarModel.taskid && t.paygroupid == payCalendarModel.paygroupid && t.payperiod == payCalendarModel.payperiod && t.date == payCalendarModel.date).ToList();

                        if (res.Count > 0)
                        {
                            response.status = false;
                            response.message = "Pay Calendar already exists";
                            return response;
                        }
                        else
                        {
                            var payClender = new PayCalendar
                            {
                                paygroupid = payCalendarModel.paygroupid,
                                payperiod = payCalendarModel.payperiod,
                                months = payCalendarModel.months,
                                date = payCalendarModel.date,
                                year = payCalendarModel.year,
                                cutoffhours = payCalendarModel.cutoffhours,
                                taskid = payCalendarModel.taskid,
                                taskname = payCalendarModel.taskname,
                                frequency = payCalendarModel.frequency,
                                tasknamelocal = payCalendarModel.tasknamelocal,
                                createdby = user.UserName,
                                createdat = _dateTimeHelper.GetDateTimeNow(),
                                modifiedby = null,
                                modifiedat = null,
                                status = "Active",
                            };

                            _appDbContext.Set<PayCalendar>().Add(payClender);
                            _appDbContext.SaveChanges();
                            response.status = true;
                            response.message = "Added Successfully";
                            return response;
                        }
                    }
                    else
                    {
                        response.status = false;
                        response.message = "Received blank/empty code or name received";
                        return response;

                    }


                }
                else
                {
                    response.status = false;
                    response.message = "Received empty Pay Calendar details";
                    return response;
                }

            }
            catch (Exception ex) 
            { 
                response.status = false;
                response.message = ex.Message;
                _logger.LogError("{ex}", ex);
                return response;
            }
        }
        public virtual DatabaseResponse UpdatePayCalendar(LoggedInUser user, PayCalendarModel payCalendarModel)
        {
            DatabaseResponse response = new DatabaseResponse();

            if (payCalendarModel != null)
            {
                if (payCalendarModel.id > 0)
                {
                    var dbResponse = _appDbContext.paycalendar.Where(t => t.taskid == payCalendarModel.taskid && t.paygroupid == payCalendarModel.paygroupid && t.payperiod == payCalendarModel.payperiod && t.date == payCalendarModel.date).AsNoTracking().ToList();

                    if (dbResponse.Count > 0)
                    {
                        if (dbResponse[0]?.id != payCalendarModel.id)
                        {
                            response.status = false;
                            response.message = "Pay Calendar already exists";
                            return response;
                        }
                        else
                        {
                            var payClender = new PayCalendar()
                            {
                                id = payCalendarModel.id,
                                paygroupid = payCalendarModel.paygroupid,
                                payperiod = payCalendarModel.payperiod,
                                months = payCalendarModel.months,
                                date = payCalendarModel.date,
                                year = payCalendarModel.year,
                                cutoffhours = payCalendarModel.cutoffhours,
                                taskid = payCalendarModel.taskid,
                                taskname = payCalendarModel.taskname,
                                tasknamelocal = payCalendarModel.tasknamelocal,
                                frequency = payCalendarModel.frequency,
                                modifiedat =_dateTimeHelper.GetDateTimeNow() ,
                                modifiedby = user.UserName,
                                createdby = payCalendarModel.createdby,
                                createdat = payCalendarModel.createdat,
                                status = payCalendarModel.status,


                            };
                            _appDbContext.paycalendar.Update(payClender);
                            _appDbContext.SaveChanges();
                            response.status = true;
                            return response;
                        }
                    }
                    else
                    {
                        var payClender = new PayCalendar()
                        {
                            id = payCalendarModel.id,
                            paygroupid = payCalendarModel.paygroupid,
                            payperiod = payCalendarModel.payperiod,
                            months = payCalendarModel.months,
                            date = payCalendarModel.date,
                            year = payCalendarModel.year,
                            cutoffhours = payCalendarModel.cutoffhours,
                            taskid = payCalendarModel.taskid,
                            taskname = payCalendarModel.taskname,
                            tasknamelocal = payCalendarModel.tasknamelocal,
                            frequency = payCalendarModel.frequency,
                            modifiedat = _dateTimeHelper.GetDateTimeNow(),
                            modifiedby = user.UserName,
                            createdby = payCalendarModel.createdby,
                            createdat = payCalendarModel.createdat,
                            status = payCalendarModel.status,


                        };
                        _appDbContext.paycalendar.Update(payClender);
                        _appDbContext.SaveChanges();
                        response.status = true;
                        return response;

                    }
                }
                else
                {
                    response.status = false;
                    response.message = "Received invalid Primary key";
                    return response;
                }
            }
            else
            {
                response.status = false;
                response.message = "Received empty Provider details";
                return response;
            }
        }

        public virtual List<PayCalendarModel> GetPayCalendar(LoggedInUser user, string paygroupCode)
        {
            List<PayCalendarModel> _PayCalendarModel = new List<PayCalendarModel>();
            try
            {
                var paygroupid = _appDbContext.paygroup.Where(p => p.code == paygroupCode).FirstOrDefault()?.id;
                if (paygroupid > 0)
                {

                    _PayCalendarModel = (from P in _appDbContext.Set<PayCalendar>()
                                         where P.paygroupid == paygroupid

                                         join PG in _appDbContext.Set<PayGroup>() on P.paygroupid equals PG.id into jps
                                         from PGResult in jps.DefaultIfEmpty()

                                         join LE in _appDbContext.Set<LegalEntity>() on PGResult.legalentityid equals LE.id into jls
                                         from LEResult in jls.DefaultIfEmpty()

                                         select new PayCalendarModel()
                                         {
                                             id = P.id,
                                             paygroupid = PGResult != null ? PGResult.id : 0,
                                             legalentityid = LEResult != null ? LEResult.id : 0,
                                             legalentitycode = LEResult != null ? LEResult.code : "",
                                             paygroupcode = PGResult != null ? PGResult.code : "",
                                             payperiod = P.payperiod,
                                             months = P.months,
                                             date = P.date,
                                             year = P.year,
                                             // cutoffhours = P.cutoffhours,
                                             cutoffhours = P.cutoffhours.ToString(),
                                             tasknamelocal = P.tasknamelocal,
                                             taskname = P.taskname,
                                             frequency = P.frequency,
                                             taskid = P.taskid,
                                             createdby = P.createdby,
                                             createdat = P.createdat,
                                             modifiedby = P.modifiedby,
                                             modifiedat = P.modifiedat,
                                             status = P.status,

                                         }).OrderByDescending(a => a.id).ToList();
                }
                else
                {
                    _PayCalendarModel = (from P in _appDbContext.Set<PayCalendar>()

                                         join PG in _appDbContext.Set<PayGroup>() on P.paygroupid equals PG.id into jps
                                         from PGResult in jps.DefaultIfEmpty()

                                         join LE in _appDbContext.Set<LegalEntity>() on PGResult.legalentityid equals LE.id into jls
                                         from LEResult in jls.DefaultIfEmpty()

                                         select new PayCalendarModel()
                                         {
                                             id = P.id,
                                             paygroupid = PGResult != null ? PGResult.id : 0,
                                             legalentityid = LEResult != null ? LEResult.id : 0,
                                             legalentitycode = LEResult != null ? LEResult.code : "",
                                             paygroupcode = PGResult != null ? PGResult.code : "",
                                             payperiod = P.payperiod,
                                             months = P.months,
                                             date = P.date,
                                             year = P.year,
                                             // cutoffhours = P.cutoffhours,
                                             cutoffhours = P.cutoffhours.ToString(),
                                             tasknamelocal = P.tasknamelocal,
                                             taskname = P.taskname,
                                             frequency = P.frequency,
                                             taskid = P.taskid,
                                             createdby = P.createdby,
                                             createdat = P.createdat,
                                             modifiedby = P.modifiedby,
                                             modifiedat = P.modifiedat,
                                             status = P.status,

                                         }).OrderByDescending(a => a.id).ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return _PayCalendarModel;
        }
        public virtual async Task<List<int>> GetPayGroupCalendarYears(int id)
        {
            try
            {
                var years = await _appDbContext.Set<PayCalendar>()
                                               .Where(p => p.paygroupid == id)
                                               .Select(p => p.year)
                                               .Distinct()
                                               .OrderByDescending(year => year)
                                               .ToListAsync();

                return years;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return new List<int>();
            }
        }

        public virtual void DeletePayCalendar(LoggedInUser user, int id)
        {
            try
            {
                var _paycalendar = _appDbContext.paycalendar.Where(s => s.id == id);

                if (_paycalendar.Any())
                {
                    _appDbContext.paycalendar.RemoveRange(_paycalendar);
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }

        }

        public virtual List<int> GetPayPeriodsByYear(LoggedInUser user, int payGroupID, int year)
        {
            try
            {
                var _payPeriods = (from p in _appDbContext.Set<PayCalendar>()
                                   where p.paygroupid == payGroupID && p.taskid == "SD"
                                   select p)
                                   .Union(
                                    from q in _appDbContext.Set<PayCalendar>()
                                    where q.paygroupid == payGroupID && q.taskid == "ED"
                                    select q
                                     ).ToList();

                var res = _payPeriods.Where(t => t.year == year).Select(t => t.payperiod).ToList();
                return res;

            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return null;
        }

        public virtual List<PayPeriods> GetPayPeriods(LoggedInUser user, int payGroupID)
        {
            List<int> _payperiods = new List<int>();
            for (int i = 1; i <= 7; i++)
            {
                if (i <= 3)
                {
                    _payperiods.Add(_dateTimeHelper.GetDateTimeNow().Date.AddMonths(-i).Month);
                }
                else if (i == 4)
                {
                    _payperiods.Add(_dateTimeHelper.GetDateTimeNow().Date.Month);
                }
                else
                {
                    _payperiods.Add(_dateTimeHelper.GetDateTimeNow().Date.AddMonths(i - 4).Month);
                }
            }

            List<PayPeriods> _payPeriods = new List<PayPeriods>();
            try
            {
                _payPeriods = (from p in _appDbContext.Set<PayCalendar>()
                               where p.paygroupid == payGroupID

                               join q in _appDbContext.Set<PayCalendar>() on p.payperiod equals q.payperiod into jps
                               from PGResult in jps.Where(f => f.paygroupid == payGroupID && f.taskid == "SD").DefaultIfEmpty()

                               join r in _appDbContext.Set<PayCalendar>() on p.payperiod equals r.payperiod into jpr
                               from PCResult in jpr.Where(f => f.paygroupid == payGroupID && f.taskid == "ED").DefaultIfEmpty()
                               
                               select new PayPeriods
                               {
                                   period = p.payperiod,
                                   paygroupid = p.paygroupid,
                                   periodtext = p.payperiod.ToString() + "-" + PGResult.date.Value.ToString("yyyy/MM/dd") + "-" + PCResult.date.Value.ToString("yyyy/MM/dd")
                               }

                               ).Distinct().ToList();

                _payPeriods = _payPeriods.Where(k => _payperiods.Contains(k.period)).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return _payPeriods;

        }

        public virtual async Task<DatabaseResponse> UploadPayCalender(LoggedInUser user, PayCalendarUploadModal mdl)
        {
            var response = new DatabaseResponse();
            try
            {
                var dataImportRequestModel = new DataImportRequestModel
                {
                    file = mdl.Excelfile,
                    entityName = "PayCalendar",
                    payGroup = mdl.paygroup
                };

                IDataImportService dataImportService = _loggedInUserRoleService.GetServiceForController<IDataImportService>(user, _dataImportService);
                var result = await dataImportService.UploadDataImport(user, dataImportRequestModel);

                if (result.status == false)
                {
                    response.status = false;
                    response.message = result.message;
                    return response;
                }

                response.status = true;
                response.message = "File Uploaded";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                response.status = false;
                response.message = ex.Message;
                return response;
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