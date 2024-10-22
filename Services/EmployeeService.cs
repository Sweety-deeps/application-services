using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Users;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Persistence;
using Services.Abstractions;
using Services.Helpers;
using System.Net;
using static System.Reflection.Metadata.BlobBuilder;

namespace Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ILogger<EmployeeService> _logger;
        private readonly AppDbContext _appDbContext;
        protected readonly ISelectListHelper _selectListHelper;

        public EmployeeService(AppDbContext appDbContext, ILogger<EmployeeService> logger, ISelectListHelper selectListHelper)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _selectListHelper = selectListHelper;
        }

        #region MainTables

        public virtual List<Employee> GetEmployees(LoggedInUser user, string paygroupCode)
        {
            List<Employee> _employees = new List<Employee>();
            try
            {
                var paygroup = _appDbContext.paygroup.Where(p => p.code == paygroupCode).FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroupCode, interfacetype, "employee");

                _employees = (from e in _appDbContext.Set<Employee>()
                              where e.paygroupid == paygroup.id
                              join p in _appDbContext.Set<PayGroup>() on e.paygroupid equals p.id into ps_jointable
                              from x in ps_jointable.DefaultIfEmpty()
                                  //on q.id equals p.gpritableid

                              select new Employee()
                              {
                                  id = e.id,
                                  employeeid = e.employeeid,
                                  employeenumber = e.employeenumber,
                                  paygroupid = e.paygroupid,
                                  paygroup = x.code,
                                  hiredate = e.hiredate,
                                  lastname = e.lastname,
                                  secondlastname = e.secondlastname,
                                  firstname = e.firstname,
                                  middlenames = e.middlenames,
                                  title = SelectListHelper.GetFieldValue(filteredResults, "title", e.title, FieldValueType.DisplayValue),
                                  gender = SelectListHelper.GetFieldValue(filteredResults, "gender", e.gender, FieldValueType.DisplayValue),
                                  dateofbirth = e.dateofbirth,
                                  birthcity = e.birthcity,
                                  birthcountry = e.birthcountry,
                                  dateofleaving = e.dateofleaving,
                                  terminationreason = SelectListHelper.GetFieldValue(filteredResults, "terminationreason", e.terminationreason, FieldValueType.DisplayValue),
                                  senioritydate = e.senioritydate,
                                  maritalstatus = SelectListHelper.GetFieldValue(filteredResults, "maritalstatus", e.maritalstatus, FieldValueType.DisplayValue),
                                  workemail = e.workemail,
                                  personalemail = e.personalemail,
                                  workphone = e.workphone,
                                  mobilephone = e.mobilephone,
                                  homephone = e.homephone,
                                  createdby = e.createdby,
                                  createdat = e.createdat,
                                  modifiedby = e.modifiedby,
                                  modifiedat = e.modifiedat,
                                  status = e.status,
                                  nationality = e.nationality,
                                  comments = e.comments,
                                  companystartdate = e.companystartdate,
                                  effectivedate = e.effectivedate,

                              }).OrderByDescending(a => a.createdat).ThenByDescending(a => a.modifiedat).ThenByDescending(a => a.id).ToList();
                _logger.LogInformation("Employees Count: {Count}, Paygroup code: {paygroupCode}", _employees.Count(), paygroupCode);
                //Temp fix for DELO_MX_BW 
                if (_employees.Count() > 1000 && paygroupCode.Equals("DELO_MX_BW"))
                {
                    _employees = _employees.Take(1000).ToList();
                }
                _logger.LogInformation("Employees Count After: {Count}, Paygroup code: {paygroupCode}", _employees.Count(), paygroupCode);
                // sResponse = JsonSerializer.Serialize(response).ToString();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _employees;
        }

        public virtual List<EmployeeJob> GetEmpJobs(LoggedInUser user, string paygroupCode)
        {
            List<EmployeeJob> _jobs = new List<EmployeeJob>();
            try
            {
                //_jobs = _appDbContext.employeejob.ToList();
                var paygroup = _appDbContext.paygroup.Where(p => p.code == paygroupCode).FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroupCode, interfacetype, "employeejob");

                _jobs = (from j in _appDbContext.Set<EmployeeJob>()
                         where j.paygroupid == paygroup.id
                         join p in _appDbContext.Set<PayGroup>() on j.paygroupid equals p.id into ps_jointable
                         from x in ps_jointable.DefaultIfEmpty()
                         select new EmployeeJob()
                         {
                             id = j.id,
                             employeeid = j.employeeid,
                             paygroupid = j.paygroupid,
                             paygroup = x.code,
                             effectivedate = j.effectivedate,
                             enddate = j.enddate,
                             personaljobtitle = j.personaljobtitle,
                             employeestatus = j.employeestatus,
                             jobchangereason = SelectListHelper.GetFieldValue(filteredResults, "jobchangereason", j.jobchangereason, FieldValueType.DisplayValue),
                             ispositionchangereason = SelectListHelper.GetFieldValue(filteredResults, "ispositionchangereason", j.ispositionchangereason, FieldValueType.DisplayValue),
                             iscompensationreason = SelectListHelper.GetFieldValue(filteredResults, "iscompensationreason", j.iscompensationreason, FieldValueType.DisplayValue),
                             isterminationreason = SelectListHelper.GetFieldValue(filteredResults, "isterminationreason", j.isterminationreason, FieldValueType.DisplayValue),
                             isleavereason = SelectListHelper.GetFieldValue(filteredResults, "isleavereason", j.isleavereason, FieldValueType.DisplayValue),
                             department = j.department,
                             location = j.location,
                             orgunit1 = j.orgunit1,
                             orgunit2 = j.orgunit2,
                             orgunit3 = j.orgunit3,
                             orgunit4 = j.orgunit4,
                             payclass = j.payclass,
                             employeetype = j.employeetype,
                             employeepackage = j.employeepackage,
                             costcenter = j.costcenter,
                             createdby = j.createdby,
                             createdat = j.createdat,
                             modifiedby = j.modifiedby,
                             modifiedat = j.modifiedat,
                             status = j.status,
                             averagenumofdays = j.averagenumofdays,
                             hiringtype = SelectListHelper.GetFieldValue(filteredResults, "hiringtype", j.hiringtype, FieldValueType.DisplayValue),
                             dailyworkinghours = j.dailyworkinghours,
                             job = j.job,
                             primaryassignment = SelectListHelper.GetFieldValue(filteredResults, "primaryassignment", j.primaryassignment, FieldValueType.DisplayValue),
                             terminationpaymentdate = j.terminationpaymentdate,
                             comments = j.comments,
                             worklocationcity = j.worklocationcity,
                             worklocationstate = j.worklocationstate,
                             position = j.position,

                         }).OrderByDescending(a => a.createdat).ThenByDescending(a => a.modifiedat).ThenByDescending(a => a.id).ToList();
                _logger.LogInformation("Jobs Count: {Count}, Paygroup code: {paygroupCode}", _jobs.Count(), paygroupCode);
                //Temp fix for DELO_MX_BW 
                if (_jobs.Count() > 1000 && paygroupCode.Equals("DELO_MX_BW"))
                {
                    _jobs = _jobs.Take(1000).ToList();
                }
                _logger.LogInformation("Jobs Count After: {Count}, Paygroup code: {paygroupCode}", _jobs.Count(), paygroupCode);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _jobs;
        }

        public virtual List<EmployeeAddress> GetEmpAddress(LoggedInUser user, string paygroupCode)
        {
            List<EmployeeAddress> _Address = new List<EmployeeAddress>();
            try
            {
                //_Address = _appDbContext.employeeaddress.ToList();
                var paygroupid = _appDbContext.paygroup.Where(p => p.code == paygroupCode).FirstOrDefault().id;

                _Address = (from j in _appDbContext.Set<EmployeeAddress>()
                            where j.paygroupid == paygroupid
                            join p in _appDbContext.Set<PayGroup>() on j.paygroupid equals p.id into ps_jointable
                            from x in ps_jointable.DefaultIfEmpty()
                            select new EmployeeAddress()
                            {
                                id = j.id,
                                employeeid = j.employeeid,
                                paygroupid = j.paygroupid,
                                paygroup = x.code,
                                effectivedate = j.effectivedate,
                                enddate = j.enddate,
                                addresstype = j.addresstype,
                                streetaddress1 = j.streetaddress1,
                                streetaddress2 = j.streetaddress2,
                                streetaddress3 = j.streetaddress3,
                                streetaddress4 = j.streetaddress4,
                                streetaddress5 = j.streetaddress5,
                                streetaddress6 = j.streetaddress6,
                                city = j.city,
                                county = j.county,
                                state = j.state,
                                postalcode = j.postalcode,
                                country = j.country,
                                createdby = j.createdby,
                                createdat = j.createdat,
                                modifiedby = j.modifiedby,
                                modifiedat = j.modifiedat,
                                status = j.status,
                                comments = j.comments,
                            }).OrderByDescending(a => a.createdat).ThenByDescending(a => a.modifiedat).ThenByDescending(a => a.id).ToList();
                _logger.LogInformation("Address Count: {Count}, Paygroup code: {paygroupCode}", _Address.Count(), paygroupCode);
                //Temp fix for DELO_MX_BW 
                if (_Address.Count() > 1000 && paygroupCode.Equals("DELO_MX_BW"))
                {
                    _Address = _Address.Take(1000).ToList();
                }
                _logger.LogInformation("Address Count After: {Count}, Paygroup code: {paygroupCode}", _Address.Count(), paygroupCode);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _Address;
        }

        public virtual List<EmployeeBank> GetEmpBanks(LoggedInUser user, string paygroupCode)
        {
            List<EmployeeBank> _banks = new List<EmployeeBank>();
            try
            {
                //_banks = _appDbContext.employeebank.ToList();
                var paygroup = _appDbContext.paygroup.Where(p => p.code == paygroupCode).FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroupCode, interfacetype, "employeebank");

                _banks = (from j in _appDbContext.Set<EmployeeBank>()
                          where j.paygroupid == paygroup.id
                          join p in _appDbContext.Set<PayGroup>() on j.paygroupid equals p.id into ps_jointable
                          from x in ps_jointable.DefaultIfEmpty()
                          select new EmployeeBank()
                          {
                              id = j.id,
                              employeeid = j.employeeid,
                              paygroupid = j.paygroupid,
                              paygroup = x.code,
                              effectivedate = j.effectivedate,
                              enddate = j.enddate,
                              bankname = SelectListHelper.GetFieldValue(filteredResults, "bankname", j.bankname, FieldValueType.DisplayValue),
                              banknumber = j.banknumber,
                              accounttype = SelectListHelper.GetFieldValue(filteredResults, "accounttype", j.accounttype, FieldValueType.DisplayValue),
                              accountnumber = j.accountnumber,
                              ibancode = j.ibancode,
                              swiftcode = j.swiftcode,
                              localclearingcode = j.localclearingcode,
                              amountorpercentage = j.amountorpercentage,
                              beneficiaryname = j.beneficiaryname,
                              createdby = j.createdby,
                              createdat = j.createdat,
                              modifiedby = j.modifiedby,
                              modifiedat = j.modifiedat,
                              status = j.status,
                              priority = Convert.ToInt32(SelectListHelper.GetFieldValue(filteredResults, "priority", j.priority.ToString(), FieldValueType.DisplayValue)),
                             banksecondaryid = SelectListHelper.GetFieldValue(filteredResults, "banksecondaryid", j.banksecondaryid, FieldValueType.DisplayValue),
                              address1 = j.address1,
                              address2 = j.address2,
                              address3 = j.address3,
                              city = j.city,
                              stateprovincecanton = j.stateprovincecanton,
                              postalcode = j.postalcode,
                              countrycode = j.countrycode,
                              comments = j.comments,
                              splitbankingtype = j.splitbankingtype,
                              fundingmethod = j.fundingmethod,
                          }).OrderByDescending(a => a.createdat).ThenByDescending(a => a.modifiedat).ThenByDescending(a => a.id).ToList();
                _logger.LogInformation("Bank Count: {Count}, Paygroup code: {paygroupCode}", _banks.Count(), paygroupCode);
                //Temp fix for DELO_MX_BW 
                if (_banks.Count() > 1000 && paygroupCode.Equals("DELO_MX_BW"))
                {
                    _banks = _banks.Take(1000).ToList();
                }
                _logger.LogInformation("Bank Count After: {Count}, Paygroup code: {paygroupCode}", _banks.Count(), paygroupCode);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _banks;
        }

        public virtual List<EmployeeSalary> GetEmpSalarys(LoggedInUser user, string paygroupCode)
        {
            List<EmployeeSalary> _salarys = new List<EmployeeSalary>();
            try
            {
                var paygroup = _appDbContext.paygroup.Where(p => p.code == paygroupCode).FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroupCode, interfacetype, "employeesalary");

                _salarys = (from j in _appDbContext.Set<EmployeeSalary>()
                            where j.paygroupid == paygroup.id
                            join p in _appDbContext.Set<PayGroup>() on j.paygroupid equals p.id into ps_jointable
                            from x in ps_jointable.DefaultIfEmpty()
                            select new EmployeeSalary()
                            {
                                id = j.id,
                                employeeid = j.employeeid,
                                paygroupid = j.paygroupid,
                                paygroup = x.code,
                                effectivedate = j.effectivedate,
                                enddate = j.enddate,
                                typeofsalary = SelectListHelper.GetFieldValue(filteredResults, "typeofsalary", j.typeofsalary, FieldValueType.DisplayValue),
                                hourlyrate = j.hourlyrate,
                                annualpay = j.annualpay,
                                periodicsalary = j.periodicsalary,
                                normalweeklyhours = j.normalweeklyhours,
                                noofinstallments = j.noofinstallments,
                                fixedenddate = j.fixedenddate,
                                createdby = j.createdby,
                                createdat = j.createdat,
                                modifiedby = j.modifiedby,
                                modifiedat = j.modifiedat,
                                status = j.status,
                                comments = j.comments,
                                salaryeffectivedate = j.salaryeffectivedate,
                            }).OrderByDescending(a => a.createdat).ThenByDescending(a => a.modifiedat).ThenByDescending(a => a.id).ToList();
                _logger.LogInformation("Salary Count: {Count}, Paygroup code: {paygroupCode}", _salarys.Count(), paygroupCode);
                //Temp fix for DELO_MX_BW 
                if (_salarys.Count() > 1000 && paygroupCode.Equals("DELO_MX_BW"))
                {
                    _salarys = _salarys.Take(1000).ToList();
                }
                _logger.LogInformation("Salary Count After: {Count}, Paygroup code: {paygroupCode}", _salarys.Count(), paygroupCode);

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _salarys;
        }

        public virtual List<EmployeePayDeduction> GetEmpPayDs(LoggedInUser user, string paygroupCode)
        {
            List<EmployeePayDeduction> _payds = new List<EmployeePayDeduction>();
            try
            {
                //_payds = _appDbContext.employeepaydeduction.ToList();
                var result = _appDbContext.paygroup
                                            .Where(p => p.code == paygroupCode)
                                            .Select(p => new { p.id, p.outboundformat })
                                            .FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(result.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroupCode, interfacetype, "employeepaydeduction");

                _payds = (from j in _appDbContext.Set<EmployeePayDeduction>()
                          where j.paygroupid == result.id
                          join p in _appDbContext.Set<PayGroup>() on j.paygroupid equals p.id into ps_jointable
                          from x in ps_jointable.DefaultIfEmpty()
                          select new EmployeePayDeduction()
                          {
                              id = j.id,
                              employeeid = j.employeeid,
                              paygroupid = j.paygroupid,
                              paygroup = x.code,
                              effectivedate = j.effectivedate,
                              enddate = j.enddate,
                              payelementtype = j.payelementtype,
                              payelementcode = j.payelementcode,
                              payelementname = j.payelementname,
                              amount = j.amount,
                              percentage = j.percentage,
                              payperiodnumber = j.payperiodnumber,
                              payperiodnumbersuffix = j.payperiodnumbersuffix,
                              paydate = j.paydate,
                              overrides = SelectListHelper.GetFieldValue(filteredResults, "overrides", j.overrides, FieldValueType.DisplayValue),
                              payrollflag = SelectListHelper.GetFieldValue(filteredResults, "payrollflag", j.payrollflag, FieldValueType.DisplayValue),
                              modificationowner = j.modificationowner,
                              modificationdate = j.modificationdate,
                              createdby = j.createdby,
                              createdat = j.createdat,
                              modifiedby = j.modifiedby,
                              modifiedat = j.modifiedat,
                              status = j.status,
                              recurrentschedule = result.outboundformat != null ? result.outboundformat == "XML" ? SelectListHelper.GetFieldValue(filteredResults, "recurrence", j.recurrence, FieldValueType.DisplayValue) : SelectListHelper.GetFieldValue(filteredResults, "recurrentschedule", j.recurrentschedule, FieldValueType.DisplayValue) : SelectListHelper.GetFieldValue(filteredResults, "recurrentschedule", j.recurrentschedule, FieldValueType.DisplayValue),
                              costcenter = j.costcenter,
                              businessdate = j.businessdate,
                              message = j.message,
                              comments = j.comments,

                          }).OrderByDescending(a => a.createdat).ThenByDescending(a => a.modifiedat).ThenByDescending(a => a.id).Take(1000).ToList();
                _logger.LogInformation("Payd Count: {Count}, Paygroup code: {paygroupCode}", _payds.Count(), paygroupCode);
                //Temp fix for DELO_MX_BW 
                if (_payds.Count() > 1000 && paygroupCode.Equals("DELO_MX_BW"))
                {
                    _payds = _payds.Take(1000).ToList();
                }
                _logger.LogInformation("Payd Count After: {Count}, Paygroup code: {paygroupCode}", _payds.Count(), paygroupCode);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _payds;
        }

        public virtual async Task<List<EmployeeContrySpecific>> GetEmpCSPs(LoggedInUser user, string paygroupCode)
        {
            List<EmployeeContrySpecific> _csps = new List<EmployeeContrySpecific>();
            try
            {
                //_csps = _appDbContext.employeecontryspecific.ToList();
                var paygroup = _appDbContext.paygroup.FirstOrDefault(p => p.code == paygroupCode);
                if (paygroup != null)
                {
                    var paygroupid = paygroup.id;
                    var countryid = paygroup.countryid;

                    var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                    var countrySpecificFields = _selectListHelper.GetCountrySpecificFields("SelectList",interfacetype, countryid);

                    var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroupCode, interfacetype, "employeecontryspecific");

                    _csps = await (from j in _appDbContext.employeecontryspecific
                             where j.paygroupid == paygroupid
                             join p in _appDbContext.paygroup on j.paygroupid equals p.id into ps_jointable
                             from x in ps_jointable.DefaultIfEmpty()
                             select new EmployeeContrySpecific()
                             {
                                 id = j.id,
                                 employeeid = j.employeeid,
                                 paygroupid = j.paygroupid,
                                 paygroup = x.code,
                                 effectivedate = j.effectivedate,
                                 endate = j.endate,
                                 country = j.country,
                                 fieldname = j.fieldname,
                                 fieldvalue = countrySpecificFields.Contains( j.fieldname) ? SelectListHelper.GetFieldValue(filteredResults, j.fieldname, j.fieldvalue, FieldValueType.DisplayValue) : j.fieldvalue,
                                 createdby = j.createdby,
                                 createdat = j.createdat,
                                 modifiedby = j.modifiedby,
                                 modifiedat = j.modifiedat,
                                 status = j.status
                             }).OrderByDescending(a => a.createdat).ThenByDescending(a => a.modifiedat).ThenByDescending(a => a.id)
                             .Take(3000).ToListAsync();
                    _logger.LogInformation("CSPF Count: {Count}, Paygroup code: {paygroupCode}", _csps.Count(), paygroupCode);
                    //Temp fix for DELO_MX_BW 
                    if (_csps.Count() > 1000 && paygroupCode.Equals("DELO_MX_BW"))
                    {
                        _csps = _csps.Take(1000).ToList();
                    }
                    _logger.LogInformation("CSPF Count After: {Count}, Paygroup code: {paygroupCode}", _csps.Count(), paygroupCode);
                }
                else
                {
                    _logger.Log(LogLevel.Error, "Paygroup code is empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _csps;
        }

        public virtual List<EmployeeConf> GetEmpConf(LoggedInUser user, string paygroupCode)
        {
            List<EmployeeConf> _conf = new List<EmployeeConf>();
            try
            {
                //_csps = _appDbContext.employeeconf.ToList();
                var paygroup = _appDbContext.paygroup.Where(p => p.code == paygroupCode).FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroupCode, interfacetype, "employeeconf");

                _conf = (from j in _appDbContext.Set<EmployeeConf>()
                         where j.paygroupid == paygroup.id
                         join p in _appDbContext.Set<PayGroup>() on j.paygroupid equals p.id into ps_jointable
                         from x in ps_jointable.DefaultIfEmpty()
                         select new EmployeeConf()
                         {
                             id = j.id,
                             employeeid = j.employeeid,
                             paygroupid = j.paygroupid,
                             paygroup = x.code,
                             effectivedate = j.effectivedate,
                             enddate = j.enddate,
                             country = j.country,
                             documenttype = SelectListHelper.GetFieldValue(filteredResults, "documenttype", j.documenttype, FieldValueType.DisplayValue),
                             documentnumber = j.documentnumber,
                             createdby = j.createdby,
                             createdat = j.createdat,
                             modifiedby = j.modifiedby,
                             modifiedat = j.modifiedat,
                             status = j.status,
                             expirydate = j.expirydate,
                             issuedate = j.issuedate,
                             placeofissue = j.placeofissue,


                         }).OrderByDescending(a => a.createdat).ThenByDescending(a => a.modifiedat).ThenByDescending(a => a.id).ToList();
                _logger.LogInformation("CONF Count: {Count}, Paygroup code: {paygroupCode}", _conf.Count(), paygroupCode);
                //Temp fix for DELO_MX_BW 
                if (_conf.Count() > 1000 && paygroupCode.Equals("DELO_MX_BW"))
                {
                    _conf = _conf.Take(1000).ToList();
                }
                _logger.LogInformation("CONF Count After: {Count}, Paygroup code: {paygroupCode}", _conf.Count(), paygroupCode);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _conf;
        }

        #endregion MainTables

        #region HistoryTables
        public virtual async Task<List<HistoryEmployeeModel>> GetHistoryEmployees(LoggedInUser user, int paygroupId, int id)
        {
            var employees = new List<HistoryEmployeeModel>();
            try
            {
                var paygroup = _appDbContext.paygroup.Where(p => p.id == paygroupId).FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroup.code, interfacetype, "employee");

                employees = await (from j in _appDbContext.historyemployee
                                   join d in _appDbContext.requestdetails on j.requestid equals d.id into gj 
                                   from d in gj.DefaultIfEmpty()
                                   where j.employeetableid == id && j.paygroupid == paygroupId
                                   select new HistoryEmployeeModel()
                                   {
                                  id = j.id,
                                  employeeid = j.employeeid,
                                  employeenumber = j.employeenumber,
                                  paygroupid = j.paygroupid,
                                  hiredate = j.hiredate,
                                  lastname = j.lastname,
                                  secondlastname = j.secondlastname,
                                  firstname = j.firstname,
                                  middlenames = j.middlenames,
                                  title = SelectListHelper.GetFieldValue(filteredResults, "title", j.title, FieldValueType.DisplayValue),
                                  gender = SelectListHelper.GetFieldValue(filteredResults, "gender", j.gender, FieldValueType.DisplayValue),
                                  dateofbirth = j.dateofbirth,
                                  birthcity = j.birthcity,
                                  birthcountry = j.birthcountry,
                                  dateofleaving = j.dateofleaving,
                                  terminationreason = SelectListHelper.GetFieldValue(filteredResults, "terminationreason", j.terminationreason, FieldValueType.DisplayValue),
                                  senioritydate = j.senioritydate,
                                  maritalstatus = SelectListHelper.GetFieldValue(filteredResults, "maritalstatus", j.maritalstatus, FieldValueType.DisplayValue),
                                  workemail = j.workemail,
                                  personalemail = j.personalemail,
                                  workphone = j.workphone,
                                  mobilephone = j.mobilephone,
                                  homephone = j.homephone,
                                  createdby = j.createdby,
                                  createdat = j.createdat,
                                  modifiedby = j.modifiedby,
                                  modifiedat = j.modifiedat,
                                  status = j.status,
                                  entitystate = j.entitystate,
                                  nationality = j.nationality,
                                  comments = j.comments,
                                  effectivedate = j.effectivedate,
                                  companystartdate = j.companystartdate,
                                  filename = d.s3objectid,
                                  requestid = j.requestid
                              }).OrderByDescending(a => a.id).ToListAsync();
                _logger.LogInformation("History Employee Count: {Count}, Paygroup code: {paygroupCode}", employees.Count(), paygroup.code);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return employees;
        }

        public virtual async Task<List<HistoryEmployeeJobModel>> GetHistoryEmpJobs(LoggedInUser user, int paygroupId, int id)
        {
            var employeeJobs = new List<HistoryEmployeeJobModel>();
            try
            {
                var paygroup = _appDbContext.paygroup.Where(p => p.id == paygroupId).FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroup.code, interfacetype, "employeejob");

                employeeJobs = await (from j in _appDbContext.historyemployeejob
                                      join d in _appDbContext.requestdetails on j.requestid equals d.id into gj
                                      from d in gj.DefaultIfEmpty()
                                      where j.jobtableid == id && j.paygroupid == paygroupId
                                      select new HistoryEmployeeJobModel()
                                      {
                                     id = j.id,
                                     employeeid = j.employeeid,
                                     paygroupid = j.paygroupid,
                                     effectivedate = j.effectivedate,
                                     enddate = j.enddate,
                                     personaljobtitle = j.personaljobtitle,
                                     employeestatus = j.employeestatus,
                                     jobchangereason = SelectListHelper.GetFieldValue(filteredResults, "jobchangereason", j.jobchangereason, FieldValueType.DisplayValue),
                                     ispositionchangereason = SelectListHelper.GetFieldValue(filteredResults, "ispositionchangereason", j.ispositionchangereason, FieldValueType.DisplayValue),
                                     iscompensationreason = SelectListHelper.GetFieldValue(filteredResults, "iscompensationreason", j.iscompensationreason, FieldValueType.DisplayValue),
                                     isterminationreason = SelectListHelper.GetFieldValue(filteredResults, "isterminationreason", j.isterminationreason, FieldValueType.DisplayValue),
                                     isleavereason = SelectListHelper.GetFieldValue(filteredResults, "isleavereason", j.isleavereason, FieldValueType.DisplayValue),
                                     department = j.department,
                                     location = j.location,
                                     orgunit1 = j.orgunit1,
                                     orgunit2 = j.orgunit2,
                                     orgunit3 = j.orgunit3,
                                     orgunit4 = j.orgunit4,
                                     payclass = j.payclass,
                                     employeetype = j.employeetype,
                                     employeepackage = j.employeepackage,
                                     costcenter = j.costcenter,
                                     createdby = j.createdby,
                                     createdat = j.createdat,
                                     modifiedby = j.modifiedby,
                                     modifiedat = j.modifiedat,
                                     status = j.status,
                                     entitystate = j.entitystate,
                                     averagenumofdays = j.averagenumofdays,
                                     hiringtype = SelectListHelper.GetFieldValue(filteredResults, "hiringtype", j.hiringtype, FieldValueType.DisplayValue),
                                     dailyworkinghours = j.dailyworkinghours,
                                     job = j.job,
                                     primaryassignment = SelectListHelper.GetFieldValue(filteredResults, "primaryassignment", j.primaryassignment, FieldValueType.DisplayValue),
                                     terminationpaymentdate = j.terminationpaymentdate,
                                     comments = j.comments,
                                     worklocationstate = j.worklocationstate,
                                     worklocationcity = j.worklocationcity,
                                     position = j.position,
                                     filename = d.s3objectid,
                                     requestid = j.requestid
                                 }).OrderByDescending(a => a.id).ToListAsync();
                _logger.LogInformation("History employeeJobs Count: {Count}, Paygroup code: {paygroupCode}", employeeJobs.Count(), paygroup.code);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return employeeJobs;
        }

        public virtual async Task<List<HistoryEmployeeAddressModel>> GetHistoryEmpAddress(LoggedInUser user, int paygroupId, int id)
        {
            var employeeAddress = new List<HistoryEmployeeAddressModel>();
            try
            {
                employeeAddress = await (from j in _appDbContext.historyemployeeaddress
                                         join d in _appDbContext.requestdetails on j.requestid equals d.id into gj
                                         from d in gj.DefaultIfEmpty()
                                         where j.addresstableid == id && j.paygroupid == paygroupId
                                         select new HistoryEmployeeAddressModel()
                                         {
                                        id = j.id,
                                        employeeid = j.employeeid,
                                        paygroupid = j.paygroupid,
                                        effectivedate = j.effectivedate,
                                        enddate = j.enddate,
                                        addresstype = j.addresstype,
                                        streetaddress1 = j.streetaddress1,
                                        streetaddress2 = j.streetaddress2,
                                        streetaddress3 = j.streetaddress3,
                                        streetaddress4 = j.streetaddress4,
                                        streetaddress5 = j.streetaddress5,
                                        streetaddress6 = j.streetaddress6,
                                        city = j.city,
                                        county = j.county,
                                        state = j.state,
                                        postalcode = j.postalcode,
                                        country = j.country,
                                        createdby = j.createdby,
                                        createdat = j.createdat,
                                        modifiedby = j.modifiedby,
                                        modifiedat = j.modifiedat,
                                        status = j.status,
                                        entitystate = j.entitystate,
                                        comments = j.comments,
                                        filename = d.s3objectid,
                                        requestid = j.requestid
                                    }).OrderByDescending(a => a.id).ToListAsync();
                _logger.LogInformation("History employeeAddress Count: {Count}, Paygroup Id: {paygroupId}", employeeAddress.Count(), paygroupId);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return employeeAddress;
        }

        public virtual async Task<List<HistoryEmployeeBankModel>> GetHistoryEmpBanks(LoggedInUser user, int paygroupId, int id)
        {
            var employeeBanks = new List<HistoryEmployeeBankModel>();
            try
            {
                var paygroup = _appDbContext.paygroup.Where(p => p.id == paygroupId).FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroup.code, interfacetype, "employeebank");

                employeeBanks = await (from j in _appDbContext.historyemployeebank
                                       join d in _appDbContext.requestdetails on j.requestid equals d.id into gj
                                       from d in gj.DefaultIfEmpty()
                                       where j.banktableid == id && j.paygroupid == paygroupId
                                       select new HistoryEmployeeBankModel()
                                       {
                                          id = j.id,
                                          employeeid = j.employeeid,
                                          paygroupid = j.paygroupid,
                                          effectivedate = j.effectivedate,
                                          enddate = j.enddate,
                                          bankname = SelectListHelper.GetFieldValue(filteredResults, "bankname", j.bankname, FieldValueType.DisplayValue),
                                          banknumber = j.banknumber,
                                          accounttype = SelectListHelper.GetFieldValue(filteredResults, "accounttype", j.accounttype, FieldValueType.DisplayValue),
                                          accountnumber = j.accountnumber,
                                          ibancode = j.ibancode,
                                          swiftcode = j.swiftcode,
                                          localclearingcode = j.localclearingcode,
                                          beneficiaryname = j.beneficiaryname,
                                          createdby = j.createdby,
                                          createdat = j.createdat,
                                          modifiedby = j.modifiedby,
                                          modifiedat = j.modifiedat,
                                          status = j.status,
                                          entitystate = j.entitystate,
                                          amountorpercentage= j.amountorpercentage,
                                          priority = Convert.ToInt32(SelectListHelper.GetFieldValue(filteredResults, "priority", j.priority.ToString(), FieldValueType.DisplayValue)),
                                          banksecondaryid = SelectListHelper.GetFieldValue(filteredResults, "banksecondaryid", j.banksecondaryid, FieldValueType.DisplayValue),
                                          address1 = j.address1,
                                          address2 = j.address2,
                                          address3 = j.address3,
                                          city = j.city,
                                          stateprovincecanton = j.stateprovincecanton,
                                          postalcode = j.postalcode,
                                          countrycode   = j.countrycode,
                                          comments = j.comments,
                                          splitbankingtype = j.splitbankingtype,
                                          fundingmethod = j.fundingmethod,
                                          requestid = j.requestid,
                                          filename = d.s3objectid
                                  }).OrderByDescending(a => a.id).ToListAsync();
                _logger.LogInformation("History employeeBanks Count: {Count}, Paygroup Code: {paygroupId}", employeeBanks.Count(), paygroup.code);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return employeeBanks;
        }

        public virtual async Task<List<HistoryEmployeeSalaryModel>> GetHistoryEmpSalarys(LoggedInUser user, int paygroupId, int id)
        {
            var employeeSalarys = new List<HistoryEmployeeSalaryModel>();
            try
            {
                var paygroup = _appDbContext.paygroup.Where(p => p.id == paygroupId).FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroup.code, interfacetype, "employeesalary");

                employeeSalarys = await (from j in _appDbContext.historyemployeesalary
                                         join d in _appDbContext.requestdetails on j.requestid equals d.id into gj
                                         from d in gj.DefaultIfEmpty()
                                         where j.salarytableid == id && j.paygroupid == paygroupId
                                         select new HistoryEmployeeSalaryModel()
                                         {
                                        id = j.id,
                                        employeeid = j.employeeid,
                                        paygroupid = j.paygroupid,
                                        effectivedate = j.effectivedate,
                                        enddate = j.enddate,
                                        typeofsalary = SelectListHelper.GetFieldValue(filteredResults, "typeofsalary", j.typeofsalary, FieldValueType.DisplayValue),
                                        hourlyrate = j.hourlyrate,
                                        annualpay = j.annualpay,
                                        periodicsalary = j.periodicsalary,
                                        normalweeklyhours = j.normalweeklyhours,
                                        noofinstallments = j.noofinstallments,
                                        fixedenddate = j.fixedenddate,
                                        createdby = j.createdby,
                                        createdat = j.createdat,
                                        modifiedby = j.modifiedby,
                                        modifiedat = j.modifiedat,
                                        status = j.status,
                                        entitystate = j.entitystate,
                                        comments = j.comments,
                                        salaryeffectivedate = j.salaryeffectivedate,
                                        filename = d.s3objectid,
                                        requestid = j.requestid
                                    }).OrderByDescending(a => a.id).ToListAsync();
                _logger.LogInformation("History employeeSalarys Count: {Count}, Paygroup Code: {paygroupId}", employeeSalarys.Count(), paygroup.code);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return employeeSalarys;
        }

        public virtual async Task<List<HistoryEmployeePayDeductionModel>> GetHistoryEmpPayDs(LoggedInUser user, int paygroupId, int id)
        {
            var employeePayDs = new List<HistoryEmployeePayDeductionModel>();
            try
            {
                var paygroup = _appDbContext.paygroup.Where(p => p.id == paygroupId).FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroup.code, interfacetype, "employeepaydeduction");

                employeePayDs = await (from j in _appDbContext.historyemployeepaydeduction
                                       join d in _appDbContext.requestdetails on j.requestid equals d.id into gj
                                       from d in gj.DefaultIfEmpty()
                                       where j.paydeductiontableid == id && j.paygroupid == paygroupId
                                       select new HistoryEmployeePayDeductionModel()
                                       {
                                      id = j.id,
                                      employeeid = j.employeeid,
                                      paygroupid = j.paygroupid,
                                      effectivedate = j.effectivedate,
                                      enddate = j.enddate,
                                      payelementtype = j.payelementtype,
                                      payelementcode = j.payelementcode,
                                      payelementname = j.payelementname,
                                      amount = j.amount,
                                      percentage = j.percentage,
                                      payperiodnumber = j.payperiodnumber,
                                      payperiodnumbersuffix = j.payperiodnumbersuffix,
                                      paydate = j.paydate,
                                      overrides = SelectListHelper.GetFieldValue(filteredResults, "overrides", j.overrides, FieldValueType.DisplayValue),
                                      payrollflag = SelectListHelper.GetFieldValue(filteredResults, "payrollflag", j.payrollflag, FieldValueType.DisplayValue),
                                      modificationowner = j.modificationowner,
                                      modificationdate = j.modificationdate,
                                      createdby = j.createdby,
                                      createdat = j.createdat,
                                      modifiedby = j.modifiedby,
                                      modifiedat = j.modifiedat,
                                      status = j.status,
                                      entitystate = j.entitystate,
                                      recurrentschedule = paygroup.outboundformat != null ? paygroup.outboundformat == "XML" ? SelectListHelper.GetFieldValue(filteredResults, "recurrence", j.recurrence, FieldValueType.DisplayValue) : SelectListHelper.GetFieldValue(filteredResults, "recurrentschedule", j.recurrentschedule, FieldValueType.DisplayValue) : SelectListHelper.GetFieldValue(filteredResults, "recurrentschedule", j.recurrentschedule, FieldValueType.DisplayValue),
                                      costcenter = j.costcenter,
                                      businessdate = j.businessdate,
                                      message = j.message,
                                      comments = j.comments,
                                      filename = d.s3objectid,
                                      requestid = j.requestid

                                  }).OrderByDescending(a => a.id).ToListAsync();
                _logger.LogInformation("History employeePayDs Count: {Count}, Paygroup Code: {paygroupId}", employeePayDs.Count(), paygroup.code);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return employeePayDs;
        }

        public virtual async Task<List<HistoryEmployeeCspfModel>> GetHistoryEmpCSPs(LoggedInUser user, int paygroupId, int id)
        {
            var employeeCSPs = new List<HistoryEmployeeCspfModel>();
            var paygroup = _appDbContext.paygroup.FirstOrDefault(p => p.id == paygroupId);
            try
            {
                    var paygroupid = paygroupId;
                    var countryid = paygroup.countryid;

                    var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                    var countrySpecificFields = _selectListHelper.GetCountrySpecificFields("SelectList", interfacetype, countryid);

                    var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroup.code, interfacetype, "employeecontryspecific");

                employeeCSPs = await (from j in _appDbContext.historyemployeecontryspecific
                                      join d in _appDbContext.requestdetails on j.requestid equals d.id into gj
                                      from d in gj.DefaultIfEmpty()
                                      where j.contryspecifictableid == id && j.paygroupid == paygroupId
                                      select new HistoryEmployeeCspfModel()
                                      {
                                     id = j.id,
                                     employeeid = j.employeeid,
                                     paygroupid = j.paygroupid,
                                     effectivedate = j.effectivedate,
                                     endate = j.endate,
                                     country = j.country,
                                     fieldname = j.fieldname,
                                     fieldvalue = countrySpecificFields.Contains(j.fieldname) ? SelectListHelper.GetFieldValue(filteredResults, j.fieldname, j.fieldvalue, FieldValueType.DisplayValue) : j.fieldvalue,
                                     createdby = j.createdby,
                                     createdat = j.createdat,
                                     modifiedby = j.modifiedby,
                                     modifiedat = j.modifiedat,
                                     status = j.status,
                                     entitystate = j.entitystate,
                                     requestid = j.requestid,
                                     filename = d.s3objectid
                                 }).OrderByDescending(a => a.id).ToListAsync();
                _logger.LogInformation("History employeeCSPs Count: {Count}, Paygroup Code: {paygroupId}", employeeCSPs.Count(), paygroup.code);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return employeeCSPs;
        }

        public virtual async Task<List<HistoryEmployeeConfModel>> GetHistoryEmpConf(LoggedInUser user, int paygroupId, int id)
        {
            var employeeConf = new List<HistoryEmployeeConfModel>();
            try
            {
                var paygroup = _appDbContext.paygroup.Where(p => p.id == paygroupId).FirstOrDefault();

                var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                var filteredResults = _selectListHelper.GetFilteredSelectListValues(paygroup.code, interfacetype, "employeeconf");

                employeeConf = await (from j in _appDbContext.historyemployeeconf
                                      join d in _appDbContext.requestdetails on j.requestid equals d.id into gj
                                      from d in gj.DefaultIfEmpty()
                                      where j.conftableid == id && j.paygroupid == paygroupId
                                      select new HistoryEmployeeConfModel()
                                      {
                                     id = j.id,
                                     entitystate = j.entitystate,
                                     employeeid = j.employeeid,
                                     paygroupid = j.paygroupid,
                                     effectivedate = j.effectivedate,
                                     enddate = j.enddate,
                                     country = j.country,
                                     documentnumber = j.documentnumber,
                                     documenttype = SelectListHelper.GetFieldValue(filteredResults, "documenttype", j.documenttype, FieldValueType.DisplayValue),
                                     createdby = j.createdby,
                                     createdat = j.createdat,
                                     modifiedby = j.modifiedby,
                                     modifiedat = j.modifiedat,
                                     status = j.status,
                                     expirydate = j.expirydate,
                                     issuedate = j.issuedate,
                                     placeofissue = j.placeofissue,
                                     requestid = j.requestid,
                                     filename = d.s3objectid
                                 }).OrderByDescending(a => a.id).ToListAsync();
                _logger.LogInformation("History employeeConf Count: {Count}, Paygroup Code: {paygroupId}", employeeConf.Count(), paygroup.code);

            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return employeeConf;
        }


        #endregion HistoryTables

        #region Time And Attendance

        public virtual List<TimeAndAttendance> GetTimeAndAttendance(LoggedInUser user, string paygroupCode)
        {
            List<TimeAndAttendance> _timeandattendance = new List<TimeAndAttendance>();
            try
            {
                //_timeandattendance = _appDbContext.timeandattendance.ToList();
                var paygroupid = _appDbContext.paygroup.Where(p => p.code == paygroupCode).FirstOrDefault().id;


                _timeandattendance = (from e in _appDbContext.Set<TimeAndAttendance>()
                                      where e.paygroupid == paygroupid
                                      join E in _appDbContext.Set<Employee>() on e.employeeid equals E.id
                                      join p in _appDbContext.Set<PayGroup>() on e.paygroupid equals p.id into ps_jointable
                                      from x in ps_jointable.DefaultIfEmpty()
                                          //on q.id equals p.gpritableid

                                      select new TimeAndAttendance()
                                      {
                                          id = e.id,
                                          employeeid = e.employeeid,
                                          employees = E.employeeid,
                                          businessdate = e.businessdate,
                                          paygroupid = e.paygroupid,
                                          paygroup = x.code,
                                          nethours = e.nethours,
                                          rate = e.rate,
                                          amountvalue = e.amountvalue,
                                          costcenter = e.costcenter,
                                          position = e.position,
                                          departmentxrefcode = e.departmentxrefcode,
                                          paycodexrefcode = e.paycodexrefcode,
                                          location = e.location,
                                          project = e.project,
                                          custom0 = e.custom0,
                                          custom1 = e.custom1,
                                          custom2 = e.custom2,
                                          custom3 = e.custom3,
                                          custom4 = e.custom4,
                                          custom5 = e.custom5,
                                          custom6 = e.custom6,
                                          custom7 = e.custom7,
                                          custom8 = e.custom8,
                                          custom9 = e.custom9,
                                          isretro = e.isretro,
                                          createdby = e.createdby,
                                          createdat = e.createdat,
                                          modifiedby = e.modifiedby,
                                          modifiedat = e.modifiedat,
                                          status = e.status,
                                          EffectiveDate = e.EffectiveDate,
                                          EndDate = e.EndDate,
                                          StartDate =e.StartDate,
                                          PayElementName = e.PayElementName
                                      }).OrderByDescending(a => a.id).Take(2000).ToList();

                _logger.LogInformation("_timeandattendance Count: {Count}, Paygroup Code: {paygroupCode}", _timeandattendance.Count(), paygroupCode);
                // sResponse = JsonSerializer.Serialize(response).ToString();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _timeandattendance;
        }

        #endregion Time And Attendance

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
