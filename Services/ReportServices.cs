using Services.Abstractions;
using Microsoft.Extensions.Logging;
using Persistence;
using Domain.Entities;
using System.Text.Json;
using System.Reflection;
using Domain.Models;
using Amazon.S3;
using Amazon.S3.Model;
using ClosedXML.Excel;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Npgsql;
using Services.Helpers;
using System.Data.Common;
using Domain.Models.Users;
using Azure.Core;
using ClosedXML.Graphics;
using System.Linq;
using Domain.Enums;
using System.Globalization;
using System.Diagnostics;
using Domain.Entities.Users;
using DocumentFormat.OpenXml.Spreadsheet;
using Renci.SshNet;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Drawing;

namespace Services
{
    public class ReportServices : IReportServices
    {
        protected readonly ILogger<ReportServices> _logger;
        protected readonly ISelectListHelper _selectListHelper;
        protected readonly IReportServiceHelper _reportServiceHelper;
        protected IAmazonS3 _s3Client { get; set; }
        protected readonly AppDbContext _appDbContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly string _reportTemplateBucketName = "ultipay-report-template";
        private readonly string _reportOutputBucketName;
        protected readonly Config _config;
        public ReportServices(AppDbContext appDbContext, IAmazonS3 s3Client, ILogger<ReportServices> logger, ISelectListHelper selectListHelper, IDateTimeHelper dateTimeHelper, IReportServiceHelper reportServiceHelper, Config config)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _s3Client = s3Client;
            _selectListHelper = selectListHelper;
            _dateTimeHelper = dateTimeHelper;
            _reportTemplateBucketName = Environment.GetEnvironmentVariable("UltipayReportTemplateBucket") ?? "ultipay-report-template";
            _reportServiceHelper = reportServiceHelper;
            _config = config;
            _reportOutputBucketName = _config.S3ReportOutputBucketName;
        }

        public virtual List<RequestHighLevelDetails> GetRequestHighLevelDetails(LoggedInUser user)
        {
            string sResponse = string.Empty;
            List<RequestHighLevelDetails> res = new List<RequestHighLevelDetails>();
            try
            {
                res = _appDbContext.requesthighleveldetails.ToList().OrderByDescending(t => t.requestdetailsid).ToList();

                return res;
                //sResponse = JsonSerializer.Serialize(vResponse).ToString();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return res;
        }

        public virtual List<RequestLowLevelDetails> GetLowLevelDetails(LoggedInUser user)
        {
            string sResponse = string.Empty;
            List<RequestLowLevelDetails> res = new List<RequestLowLevelDetails>();
            try
            {
                //var vResponse = (from s in _appDbContext.Set<RequestLowLevelDetails>()
                //                 join t in _appDbContext.Set<RequestDetails>()
                //                 on s.requestdetailsid equals t.id
                //                 select new
                //                 {
                //                     requestdetailid = t.id,
                //                     eventname = s.eventname,
                //                     employeeid = s.employeeid,
                //                     failureentity = s.failureentity,
                //                     failurereason = s.failurereason,
                //                     timetaken = GetTimeTaken(t.createdat, t.modifiedat),
                //                     status = s.status
                //                 }
                //       ).ToList();
                //sResponse = JsonSerializer.Serialize(vResponse).ToString();

                res = _appDbContext.requestlowleveldetails.ToList().OrderByDescending(t => t.requestdetailsid).ToList();

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return res;
        }

        public virtual double GetTimeTaken(DateTime from, DateTime? to)
        {
            TimeSpan ts = TimeSpan.Zero;
            ts = (TimeSpan)(to - from);
            return ts.TotalMinutes;
        }
        


        public virtual string GetComparisonData(LoggedInUser user, int? iRequestID)
        {
            string sResponse = string.Empty;
            try
            {
                //TODO Arumugam
                var vResponse = (from s in _appDbContext.Set<StagingEmployee>().Where(x => x.requestid == iRequestID)
                                 join t in _appDbContext.Set<TransformedEmployee>().Where(y => y.requestid == iRequestID)
                                 on s.employeeid equals t.employeeid
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
                sResponse = System.Text.Json.JsonSerializer.Serialize(vResponse).ToString();

            }
            catch (Exception ex)
            {
                //TODO Need to do error logging
                _logger.Log(LogLevel.Error, ex.ToString());
            }

            return sResponse;

        }
        public virtual string GetEmployeeDetails(LoggedInUser user, string iEmployeeID)
        {
            string sResponse = string.Empty;
            try
            {
                var vResponse = (from s in _appDbContext.Set<Employee>().Where(x => x.employeeid == iEmployeeID)
                                 join h in _appDbContext.Set<PayGroup>() on s.paygroupid equals h.id
                                 select new
                                 {
                                     semployeeid = s.employeeid,
                                     semployeenumber = s.employeenumber,
                                     paygroup = h.code,
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
                sResponse = System.Text.Json.JsonSerializer.Serialize(vResponse).ToString();

            }
            catch (Exception ex)
            {
                //TODO Need to do error logging
                _logger.Log(LogLevel.Error, ex.ToString());
            }

            return sResponse;

        }

        #region Download Reports

        public virtual int? GetCountryIDByCode(LoggedInUser user, string strCode)
        {
            var response = (from c in _appDbContext.Set<Country>()
                            where c.code == strCode
                            select c.id).FirstOrDefault();

            return response;

        }
        public virtual async Task<List<PayPeriodRegistorDBValuesDetail>> GetPayPeriodRegisterImportDataAsync(LoggedInUser user, PayPeriodRegisterRequestModel requestModel)
        {
            var lstPayPeriodRegistorDBValues = new List<PayPeriodRegistorDBValuesDetail>();
            try
            {
                string sqlQuery;
                var sqlParameters = new List<NpgsqlParameter>();
                if (requestModel.gpriId != null)
                {
                    sqlQuery = "SELECT * FROM dbo.getpayperiodregisterforid(@gpriId)";
                    sqlParameters.Add(new NpgsqlParameter("@gpriId", requestModel.gpriId));
                }
                else
                {
                    sqlQuery = "SELECT * FROM dbo.getpayperiodregister(@paygroup, @year, @startpp, @endpp)";

                    sqlParameters.Add(new NpgsqlParameter("@paygroup", requestModel.paygroup));
                    sqlParameters.Add(new NpgsqlParameter("@year", requestModel.year));
                    sqlParameters.Add(new NpgsqlParameter("@startpp", requestModel.startpp));
                    sqlParameters.Add(new NpgsqlParameter("@endpp", requestModel.endpp));
                }

                lstPayPeriodRegistorDBValues = await _appDbContext.PayPeriodRegistorDBValuesDetail
                    .FromSqlRaw(sqlQuery, sqlParameters.ToArray())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in getting records");
            }
            return lstPayPeriodRegistorDBValues;
        }

        public virtual async Task<List<PayPeriodRegisterResponseModel>> GetPayPeriodRegistersAsync(LoggedInUser user, PayPeriodRegisterRequestModel requestModel, int iPayGroupID)
        {
            var lsPayPeriodRegisterResponseModel = new List<PayPeriodRegisterResponseModel>();

            try
            {
                var lstPayPeriodRegistorDBValues = await GetPayPeriodRegisterImportDataAsync(user, requestModel);

                var groupedData = lstPayPeriodRegistorDBValues
                    .GroupBy(x => new { x.EmployeeID, x.PayPeriod })
                    .Select(g => new
                    {
                        Group = g,
                        FirstEntry = g.First(),
                        EREEarnings = g.Where(x => x.Froms == "Employer" && x.Tos == "Employee" && x.ItemType == "Earning").Sum(x => Convert.ToDecimal(x.ItmeAmount)),
                        EEERDeductions = g.Where(x => x.Froms == "Employee" && x.Tos == "Employer" && x.ItemType == "Deduction").Sum(x => Convert.ToDecimal(x.ItmeAmount)),
                        EEOTEEContributions = g.Where(x => x.Froms == "Employee" && x.Tos == "Other").Sum(x => Convert.ToDecimal(x.ItmeAmount)),
                        EROTERContributions = g.Where(x => x.Froms == "Employer" && x.Tos == "Other").Sum(x => Convert.ToDecimal(x.ItmeAmount))
                    }).ToList();

                foreach (var group in groupedData)
                {
                    PayPeriodRegisterResponseModel objTemp = new PayPeriodRegisterResponseModel
                    {
                        PayGroup = group.FirstEntry.PayGroup,
                        EmployeeID = group.FirstEntry.EmployeeID,
                        EmployeeNumber = group.FirstEntry.EmployeeNumber,
                        PayPeriod = group.FirstEntry.PayPeriod,
                        Offcycle = group.FirstEntry.Offcycle,
                        HireDate = group.FirstEntry.HireDate,
                        TerminationDate = group.FirstEntry.TerminationDate,
                        LastName = group.FirstEntry.LastName,
                        SecondLastName = group.FirstEntry.SecondLastName,
                        FirstName = group.FirstEntry.FirstName,
                        MiddleNames = group.FirstEntry.MiddleNames,
                        EREEEarnings = group.EREEarnings,
                        EEERDeductions = group.EEERDeductions,
                        EEOTEEContributions = group.EEOTEEContributions,
                        EROTERContributions = group.EROTERContributions,
                        TotalDeductions = group.EEERDeductions + group.EROTERContributions,
                        NetPay = group.EREEarnings - (group.EEERDeductions + group.EROTERContributions),
                        TotalDisbursement = group.EEOTEEContributions + group.EROTERContributions,
                        EmployerCost = group.EREEarnings - (group.EEERDeductions + group.EROTERContributions) + group.EEOTEEContributions + group.EROTERContributions,
                        TotalGrossPay = group.EREEarnings
                    };

                    lsPayPeriodRegisterResponseModel.Add(objTemp);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in getting records");
            }
            return lsPayPeriodRegisterResponseModel;
        }

        public virtual async Task<List<PayPeriodRegisterDetailResponseModel>> GetPayPeriodDetailRegistersAsync(LoggedInUser user, PayPeriodRegisterRequestModel requestModel, int iPayGroupID)
        {
            var lsPayPeriodRegisterResponseModel = new List<PayPeriodRegisterDetailResponseModel>();

            try
            {
                var lstPayPeriodRegistorDBValues = await GetPayPeriodRegisterImportDataAsync(user, requestModel);

                var groupedData = lstPayPeriodRegistorDBValues
                    .GroupBy(x => new { x.EmployeeID, x.PayPeriod })
                    .ToList();

                var payrollElements = await _appDbContext.payrollelements
                    .Where(pe => pe.paygroupid == iPayGroupID)
                    .Select(pe => new
                    {
                        pe.froms,
                        pe.tos,
                        pe.type,
                        pe.code,
                        pe.name,
                        displayname = pe.code + " - " + pe.name
                    })
                    .ToListAsync();

                foreach (var group in groupedData)
                {
                    var firstEntry = group.First();

                    var employerToEmployee = payrollElements
                        .Where(pe => pe.froms == "Employer" && pe.tos == "Employee" && pe.type == "Earning")
                        .Select(pe => new EmployerToEmployee
                        {
                            code = pe.code,
                            name = pe.name,
                            displayname = pe.displayname,
                            value = group
                                .Where(x => x.Froms == "Employer" && x.Tos == "Employee" && x.ItemType == "Earning" && x.PECode == pe.code && x.PEName == pe.name)
                                .Sum(x => Convert.ToDecimal(x.ItmeAmount))
                        })
                        .ToList();

                    var employeeToEmployer = payrollElements
                        .Where(pe => pe.froms == "Employee" && pe.tos == "Employer" && pe.type == "Deduction")
                        .Select(pe => new EmployeeToEmployer
                        {
                            code = pe.code,
                            name = pe.name,
                            displayname = pe.displayname,
                            value = group
                                .Where(x => x.Froms == "Employee" && x.Tos == "Employer" && x.ItemType == "Deduction" && x.PECode == pe.code && x.PEName == pe.name)
                                .Sum(x => Convert.ToDecimal(x.ItmeAmount))
                        })
                        .ToList();

                    var employeeToOther = payrollElements
                        .Where(pe => pe.froms == "Employee" && pe.tos == "Other")
                        .Select(pe => new EmployeeToOther
                        {
                            code = pe.code,
                            name = pe.name,
                            displayname = pe.displayname,
                            value = group
                                .Where(x => x.Froms == "Employee" && x.Tos == "Other" && x.PECode == pe.code && x.PEName == pe.name)
                                .Sum(x => Convert.ToDecimal(x.ItmeAmount))
                        })
                        .ToList();

                    var employerToOther = payrollElements
                        .Where(pe => pe.froms == "Employer" && pe.tos == "Other")
                        .Select(pe => new EmployerToOther
                        {
                            code = pe.code,
                            name = pe.name,
                            displayname = pe.displayname,
                            value = group
                                .Where(x => x.Froms == "Employer" && x.Tos == "Other" && x.PECode == pe.code && x.PEName == pe.name)
                                .Sum(x => Convert.ToDecimal(x.ItmeAmount))
                        })
                        .ToList();

                    decimal? dEmployerToEmployee = employerToEmployee.Sum(x => x.value);
                    decimal? dEmployeeToEmployer = employeeToEmployer.Sum(x => x.value);
                    decimal? dEmployeeToOther = employeeToOther.Sum(x => x.value);
                    decimal? dEmployerToOther = employerToOther.Sum(x => x.value);

                    PayPeriodRegisterDetailResponseModel objTemp = new PayPeriodRegisterDetailResponseModel
                    {
                        PayGroup = firstEntry.PayGroup,
                        EmployeeID = firstEntry.EmployeeID,
                        EmployeeNumber = firstEntry.EmployeeNumber,
                        PayPeriod = firstEntry.PayPeriod,
                        Offcycle = firstEntry.Offcycle,
                        HireDate = firstEntry.HireDate,
                        TerminationDate = firstEntry.TerminationDate,
                        LastName = firstEntry.LastName,
                        SecondLastName = firstEntry.SecondLastName,
                        FirstName = firstEntry.FirstName,
                        MiddleNames = firstEntry.MiddleNames,
                        EmployerToEmployee = employerToEmployee,
                        EmployeeToEmployer = employeeToEmployer,
                        EmployeeToOther = employeeToOther,
                        EmployerToOther = employerToOther,
                        EmployerCost = dEmployerToEmployee + dEmployerToOther,
                        TotalDeductions = dEmployeeToOther + dEmployeeToEmployer,
                        TotalGrossPay = dEmployerToEmployee,
                        NetPay = dEmployerToEmployee - (dEmployeeToOther + dEmployeeToEmployer),
                        TotalDisbursement = dEmployeeToOther + dEmployerToOther
                    };

                    lsPayPeriodRegisterResponseModel.Add(objTemp);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in getting records");
            }

            return lsPayPeriodRegisterResponseModel;
        }
        public virtual DateTime GetPayPeriodDate(LoggedInUser user, string strPayGroupCode, string strTaskName, int iPayPeriod)
        {
            var PayPeriodDate = (from p in _appDbContext.Set<PayGroup>()
                                 join pc in _appDbContext.Set<PayCalendar>() on p.id equals pc.paygroupid
                                 where p.code == strPayGroupCode && pc.payperiod == iPayPeriod
                                                                 && pc.taskname == strTaskName
                                 select pc.date).FirstOrDefault();


            return Convert.ToDateTime(PayPeriodDate);
        }

        public virtual List<HrDatawarehouseResponseModel> GetHrDatawarehouse(LoggedInUser user, HrDatawarehouseRequestModel _data)
        {
            var result = new List<HrDatawarehouseResponseModel>();

            try
            {
                var query = $"select * from dbo.get_HRD_Report('{_data.paygroup}', null, '{_data.includeterminated}')";
                var dt = _appDbContext.DataTable(query, new DbParameter[] { });

                if (dt != null && dt.Rows.Count > 0)
                {
                    result = dt.AsEnumerable()
                        .Select(row =>
                        {
                            var dateOfLeaving = row.Field<DateTime?>("F_dateofleaving");
                            var employeeStatus = dateOfLeaving < DateTime.Today ? "Terminated" : "Active";

                            if (_data.includeterminated.ToLower() == "yes")
                            {
                                employeeStatus = dateOfLeaving < DateTime.Today ? "Terminated" : "Active";
                            }
                            else
                            {
                                employeeStatus = "Active";
                            }

                            return new HrDatawarehouseResponseModel()
                            {
                                PayGroup = _data.paygroup,
                                EmployeeID = row.Field<string>("F_employeeid"),
                                EmployeeNumber = row.Field<string>("F_employeenumber"),
                                HireDate = row.Field<DateTime?>("F_hiredate")?.ToString("yyyy-MM-dd"),
                                LastName = row.Field<string>("F_lastname"),
                                SecondLastName = row.Field<string>("F_secondlastname"),
                                FirstName = row.Field<string>("F_firstname"),
                                MiddleNames = row.Field<string>("F_middlenames"),
                                Title = row.Field<string>("F_title"),
                                Gender = row.Field<string>("F_gender"),
                                DateofBirth = row.Field<DateTime?>("F_dateofbirth")?.ToString("yyyy-MM-dd"),
                                BirthCity = row.Field<string>("F_birthcity"),
                                BirthCountry = row.Field<string>("F_birthcountry"),
                                Nationality = row.Field<string>("F_nationality"),
                                TerminationDate = row.Field<DateTime?>("F_dateofleaving")?.ToString("yyyy-MM-dd"),
                                SeniorityDate = row.Field<DateTime?>("F_senioritydate")?.ToString("yyyy-MM-dd"),
                                WorkEmail = row.Field<string>("F_workemail"),
                                PersonalEmail = row.Field<string>("F_personalemail"),
                                WorkPhone = row.Field<string>("F_workphone"),
                                MobilePhone = row.Field<string>("F_mobilephone"),
                                HomePhone = row.Field<string>("F_homephone"),
                                MaritalStatus = row.Field<string>("F_maritalstatus"),
                                EmployeeStatus = employeeStatus,
                                TerminationReason = row.Field<string>("F_terminationreason"),
                                PayClass = row.Field<string>("F_payclass"),
                                SalaryEffectiveDate = row.Field<DateTime?>("F_effectivedate")?.ToString("yyyy-MM-dd"),
                                SalaryEndDate = row.Field<DateTime?>("F_enddate")?.ToString("yyyy-MM-dd"),
                                TypeofSalary = row.Field<string>("F_typeofsalary"),
                                HourlyRate = row.Field<decimal?>("F_hourlyrate"),
                                AnnualPay = row.Field<decimal?>("F_annualpay"),
                                PeriodicSalary = row.Field<decimal?>("F_periodicsalary"),
                                NoofInstallments = row.Field<decimal?>("F_noofinstallments"),
                                DailyWorkingHours = row.Field<decimal?>("F_dailyworkinghours"),
                                TerminationPaymentDate = row.Field<DateTime?>("F_terminationpaymentdate")?.ToString("yyyy-MM-dd"),
                                HiringType = row.Field<string>("F_hiringtype"),
                                WorkingDaysPerWeek = row.Field<int?>("F_averagenumofdays"),
                                JobChangeReason = row.Field<string>("F_jobchangereason"),
                                PersonalJobTitle = row.Field<string>("F_personaljobtitle"),
                                Department = row.Field<string>("F_department"),
                                CostCenter = row.Field<string>("F_costcenter"),
                                EmployeePackage = row.Field<string>("F_employeepackage"),
                                EmployeeType = row.Field<string>("F_employeetype"),
                                Job = row.Field<string>("F_job"),
                                Location = row.Field<string>("F_location"),
                                PrimaryAssignment = row.Field<string>("F_primaryassignment"),
                                OrgUnit1 = row.Field<string>("F_orgunit1"),
                                OrgUnit2 = row.Field<string>("F_orgunit2"),
                                OrgUnit3 = row.Field<string>("F_orgunit3"),
                                OrgUnit4 = row.Field<string>("F_orgunit4"),
                                AddressType = row.Field<string>("F_addresstype"),
                                StreetAddress1 = row.Field<string>("F_streetaddress1"),
                                StreetAddress2 = row.Field<string>("F_streetaddress2"),
                                StreetAddress3 = row.Field<string>("F_streetaddress3"),
                                StreetAddress4 = row.Field<string>("F_streetaddress4"),
                                StreetAddress5 = row.Field<string>("F_streetaddress5"),
                                StreetAddress6 = row.Field<string>("F_streetaddress6"),
                                City = row.Field<string>("F_city"),
                                County = row.Field<string>("F_county"),
                                State = row.Field<string>("F_state"),
                                country = row.Field<string>("F_country"),
                                PostalCode = row.Field<string>("F_postalcode"),
                                BankName = row.Field<string>("F_bankname"),
                                BankNumber = row.Field<string>("F_banknumber"),
                                AccountType = row.Field<string>("F_accounttype"),
                                AccountNumber = row.Field<string>("F_accountnumber"),
                                IbanCode = row.Field<string>("F_ibancode"),
                                SwiftCode = row.Field<string>("F_swiftcode"),
                                LocalClearingCode = row.Field<string>("F_localclearingcode"),
                                BeneficiaryName = row.Field<string>("F_beneficiaryname"),
                                payperiod = Convert.ToString(row.Field<int?>("F_payperiod")),
                                year = row.Field<int?>("F_years"),
                            };
                        })
                        .ToList();
                }
                else
                {
                    _logger.LogInformation("No record found to export in hr dataware house report");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving HR data warehouse.");
            }

            return result;
        }


        public virtual CSAndCFDatawarehouseResponseModel GetCSPFDatawarehouse(LoggedInUser user, CSPFDatawarehouseRequestModel _data)
        {
            List<CSPFDatawarehouseResponseModel> lstCSPFDatawarehouseResponseModel = new List<CSPFDatawarehouseResponseModel>();
            List<CFDatawarehouseResponseModel> lstCFDatawarehouseResponseModel = new List<CFDatawarehouseResponseModel>();
            CSAndCFDatawarehouseResponseModel lstCSAndCFDatawarehouseResponseModel = new CSAndCFDatawarehouseResponseModel();
            try
            {
                DateTime currentDate = _dateTimeHelper.GetDateTimeNow();
                var paygroup = _appDbContext.paygroup.FirstOrDefault(p => p.code == _data.paygroup);
                if (paygroup != null)
                {
                    var paygroupid = paygroup.id;
                    var countryid = paygroup.countryid;

                    var interfacetype = SelectListHelper.GetInterfaceType(paygroup.outboundformat);

                    var countrySpecificFields = _selectListHelper.GetCountrySpecificFields("SelectList", interfacetype, countryid);

                    var filteredResults = _selectListHelper.GetFilteredSelectListValues(_data.paygroup, interfacetype, "employeecontryspecific");
                    if (_data.includeterminated.ToLower() == "yes")
                    {
                        lstCSPFDatawarehouseResponseModel = (from E in _appDbContext.employee
                                                             join C in _appDbContext.employeecontryspecific on new { E.employeeid, E.paygroupid } equals new { C.employeeid, C.paygroupid }
                                                             join PG in _appDbContext.paygroup on E.paygroupid equals PG.id
                                                             where (E.hiredate >= (_data.startdate != null ? _data.startdate : E.hiredate) && E.hiredate <= (_data.enddate != null ? _data.enddate : E.hiredate)) && (C.effectivedate <= currentDate && (C.endate >= currentDate || C.endate == null))
                                                             && PG.code == (_data.paygroup != null ? _data.paygroup : PG.code)
                                                             select new CSPFDatawarehouseResponseModel()
                                                             {
                                                                 StartDate = E.hiredate,
                                                                 EndDate = C.endate,
                                                                 EffectiveDate = C.effectivedate,
                                                                 PayGroup = PG.code,
                                                                 EmployeeID = C.employeeid,
                                                                 CsFIdName = C.fieldname,
                                                                 Value = countrySpecificFields.Contains(C.fieldname) ? SelectListHelper.GetFieldValue(filteredResults, C.fieldname, C.fieldvalue, FieldValueType.OutputValue) : C.fieldvalue,
                                                                 EmployeeStatus = (E.dateofleaving >= currentDate || E.dateofleaving == null) ? "Active" : "Terminated"
                                                             }).Distinct().OrderBy(x => x.EmployeeID).ThenBy(x => x.CsFIdName).ToList();
                    }
                    else
                    {
                        lstCSPFDatawarehouseResponseModel = (from E in _appDbContext.employee
                                                             join C in _appDbContext.employeecontryspecific on new { E.employeeid, E.paygroupid } equals new { C.employeeid, C.paygroupid }
                                                             join PG in _appDbContext.paygroup on E.paygroupid equals PG.id
                                                             where (E.dateofleaving == null || E.dateofleaving > currentDate) && (C.effectivedate <= currentDate && (C.endate >= currentDate || C.endate == null))
                                                             && PG.code == (_data.paygroup != null ? _data.paygroup : PG.code)
                                                             select new CSPFDatawarehouseResponseModel()
                                                             {
                                                                 StartDate = E.hiredate,
                                                                 EndDate = C.endate,
                                                                 EffectiveDate = C.effectivedate,
                                                                 PayGroup = PG.code,
                                                                 EmployeeID = C.employeeid,
                                                                 CsFIdName = C.fieldname,
                                                                 Value = countrySpecificFields.Contains(C.fieldname) ? SelectListHelper.GetFieldValue(filteredResults, C.fieldname, C.fieldvalue, FieldValueType.OutputValue) : C.fieldvalue,
                                                                 EmployeeStatus = "Active"
                                                             }).Distinct().OrderBy(x => x.EmployeeID).ThenBy(x => x.CsFIdName).ToList();
                    }

                    lstCFDatawarehouseResponseModel = (from E in _appDbContext.employee
                                                       join C in _appDbContext.employeeconf on new { E.employeeid, E.paygroupid } equals new { C.employeeid, C.paygroupid }
                                                       join PG in _appDbContext.paygroup on E.paygroupid equals PG.id
                                                       where PG.code == (_data.paygroup != null ? _data.paygroup : PG.code)
                                                       select new CFDatawarehouseResponseModel()
                                                       {
                                                           EmployeeID = C.employeeid,
                                                           PayGroup = PG.code,
                                                           EffectiveDate = C.effectivedate,
                                                           EndDate = C.enddate,
                                                           DocumentType = C.documenttype,
                                                           DocumentNumber = C.documentnumber,
                                                           Country = C.country,
                                                           PlaceOfIssue = C.placeofissue,
                                                           IssueDate = C.issuedate,
                                                           ExpiryDate = C.expirydate,

                                                       }).Distinct().OrderBy(x => x.EmployeeID).ThenBy(x => x.DocumentNumber).ToList();
                }
                else
                {
                    _logger.LogInformation("Paygroup not found");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            lstCSAndCFDatawarehouseResponseModel.cspfDatawarehouseResponse = lstCSPFDatawarehouseResponseModel;
            lstCSAndCFDatawarehouseResponseModel.confidentialDatawarehouseResponse = lstCFDatawarehouseResponseModel;
            return lstCSAndCFDatawarehouseResponseModel;
        }
        public virtual List<PaydDatawarehouseResponseModel> GetPAYDDatawarehouse(LoggedInUser user, PaydDatawarehouseRequestModel _data)
        {
            List<PaydDatawarehouseResponseModel> lstPaydDatawarehouseResponseModel = new List<PaydDatawarehouseResponseModel>();

            try
            {
                var result = (from p in _appDbContext.Set<PayGroup>()
                              where p.code == _data.paygroup
                              select new { p.id, p.outboundformat }).FirstOrDefault();

                int? paygroupId = result?.id;
                string? interfacetype = SelectListHelper.GetInterfaceType(result?.outboundformat);
                var selectListValues = _selectListHelper.GetFilteredSelectListValues(_data.paygroup, interfacetype, "employeepaydeduction");

                lstPaydDatawarehouseResponseModel = (
                    from E in _appDbContext.employee
                    join PD in _appDbContext.employeepaydeduction
                    on new { Employeeid = E.employeeid, Paygroupid = (int?)E.paygroupid } equals new { Employeeid = PD.employeeid, Paygroupid = (int?)PD.paygroupid } into EPD
                    from PDResult in EPD.DefaultIfEmpty()
                    join PG in _appDbContext.paygroup on PDResult.paygroupid equals PG.id into PPG
                    from PGResult in PPG.DefaultIfEmpty()
                    join PE in _appDbContext.payrollelements
                    on new { PayElementCode = PDResult.payelementcode, PayGroupId = (int?)PDResult.paygroupid }
                    equals new { PayElementCode = PE.exportcode, PayGroupId = (int?)PE.paygroupid } into PPE
                    from PEResult in PPE.DefaultIfEmpty()
                    where PGResult.code == _data.paygroup
                      && (
                          _data.includeterminated == "yes"
                          || (_data.includeterminated == "no" && (E.dateofleaving == null || E.dateofleaving.HasValue ? E.dateofleaving >= _dateTimeHelper.GetDateTimeNow() : false))
                      )
                      && (PDResult.enddate == null || (PDResult.enddate.HasValue ? PDResult.enddate >= _dateTimeHelper.GetDateTimeNow() : true))
                    select new PaydDatawarehouseResponseModel()
                    {
                        PayGroup = PGResult.code,
                        EmployeeID = E.employeeid,
                        EffectiveDate = PDResult.effectivedate.HasValue ? PDResult.effectivedate.Value.ToString("yyyy-MM-dd") : null,
                        EndDate = PDResult.enddate.HasValue ? PDResult.enddate.Value.ToString("yyyy-MM-dd") : null,
                        PayElementType = PDResult.payelementtype,
                        PayElementName = PDResult.payelementname,
                        PayElementCode = PEResult.code,
                        ExportCode = PEResult.exportcode,
                        RecurrentSchedule = result.outboundformat != null ? result.outboundformat == "XML" ? SelectListHelper.GetFieldValue(selectListValues, "recurrence", PDResult.recurrence, FieldValueType.OutputValue) : SelectListHelper.GetFieldValue(selectListValues, "recurrentschedule", PDResult.recurrentschedule, FieldValueType.OutputValue) : SelectListHelper.GetFieldValue(selectListValues, "recurrentschedule", PDResult.recurrentschedule, FieldValueType.OutputValue),
                        Amount = PDResult.amount,
                        Percentage = PDResult.percentage,
                        PayDate = PDResult.paydate.HasValue ? PDResult.paydate.Value.ToString("yyyy-MM-dd") : null,
                        BussinessDate = PDResult.businessdate.HasValue ? PDResult.businessdate.Value.ToString("yyyy-MM-dd") : null,
                        PayPeriodNumber = PDResult.payperiodnumber,
                        PayPeriodNumberSuffix = PDResult.payperiodnumbersuffix,
                        PayrollFlag = (PDResult.payrollflag != null) ? SelectListHelper.GetFieldValue(selectListValues, "payrollflag", PDResult.payrollflag, FieldValueType.OutputValue) : PDResult.payrollflag,
                        Message = PDResult.message,
                        CostCenter = PDResult.costcenter,
                        ModificationOwner = PDResult.modificationowner,
                        ModificationDate = PDResult.modificationdate.HasValue ? PDResult.modificationdate.Value.ToString("yyyy-MM-dd") : null,
                        EmployeeStatus = (E.dateofleaving != null && E.dateofleaving.HasValue && E.dateofleaving < _dateTimeHelper.GetDateTimeNow()) ? "Terminated" : "Active"
                    }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }

            return lstPaydDatawarehouseResponseModel;
        }


        public virtual List<PayElementResponseModel> GetPayElementReport(LoggedInUser user, PayElementRequestModel _data)
        {

            List<PayElementResponseModel> lstPayElementResponseModel = new List<PayElementResponseModel>();
            try
            {
                lstPayElementResponseModel = (

                    from PE in _appDbContext.payrollelements

                    join PG in _appDbContext.paygroup on PE.paygroupid equals PG.id into jpg
                    from PGResult in jpg.DefaultIfEmpty()

                    join LE in _appDbContext.legalentity on PGResult.legalentityid equals LE.id into jle
                    from LEResult in jle.DefaultIfEmpty()

                    join C in _appDbContext.client on LEResult.clientid equals C.id into jc
                    from CResult in jc.DefaultIfEmpty()

                        //from PE in _appDbContext.payrollelements
                        // join PG in _appDbContext.paygroup on PE.paygroupid equals PG.id
                        // join LE in _appDbContext.legalentity on PG.legalentityid equals LE.id
                        // join C in _appDbContext.client on LE.clientid equals C.id
                        // where PG.code == (_data.paygroup != null ? _data.paygroup : PG.code)
                        // && C.code == (_data.clientname != null ? _data.clientname : C.code)
                    select new PayElementResponseModel()
                    {

                        PayGroup = PGResult.code,
                        PayElementNameLocal = PE.namelocal,
                        PayElementName = PE.name,
                        PayElementCode = PE.code,
                        ExportCode = PE.exportcode,
                        Status = PE.status,
                        Type = PE.itemtype,
                        Format = PE.format,
                        GLCreditCode = PE.glcreditcode,
                        GLDebitCode = PE.gldebitcode,
                        ClientReported = PE.clientreported,
                        PayslipPrint = PE.payslipprint,
                        Comments = PE.comments,
                        From = PE.froms,
                        To = PE.tos,
                        ContributesToNetPay = PE.contributetonetpay,
                        IsEmployerTax = PE.isemployertax,
                        IsEmployeeDeduction = PE.isemployerdeduction,
                        Clientname = CResult.code,

                    }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return lstPayElementResponseModel;
        }
        public virtual async Task<ConfigurationResponseModel> GetConfigurationData(LoggedInUser user, ConfigurationRequestModel model)
        {
            ConfigurationResponseModel configurationResponseModel = new ConfigurationResponseModel();

            try
            {
                switch (model.configname)
                {
                    case "payrollelements":
                        // Code to handle payrollelements
                        //HandlePayRoleElements(model.payelements);
                        configurationResponseModel.payelements = (from pe in _appDbContext.payrollelements.AsNoTracking()
                                                                  join pg in _appDbContext.paygroup.AsNoTracking() on pe.paygroupid equals pg.id
                                                                  join ta in _appDbContext.taxauthority.AsNoTracking() on pe.taxauthorityid equals ta.id into jta
                                                                  from TAResult in jta.DefaultIfEmpty()
                                                                  where pg.code == model.paygroup
                                                                  select new PayrollElementsModel()
                                                                  {
                                                                      paygroupcode = pg.code,
                                                                      code = pe.code,
                                                                      name = pe.name,
                                                                      namelocal = pe.namelocal,
                                                                      exportcode = pe.exportcode,
                                                                      taxauthoritycode = TAResult != null ? TAResult.code : null,
                                                                      pestatus = pe.pestatus,
                                                                      itemtype = pe.itemtype,
                                                                      format = pe.format,
                                                                      glcreditcode = pe.glcreditcode,
                                                                      gldebitcode = pe.gldebitcode,
                                                                      clientreported = pe.clientreported,
                                                                      payslipprint = pe.payslipprint,
                                                                      comments = pe.comments,
                                                                      froms = pe.froms,
                                                                      tos = pe.tos,
                                                                      status = pe.status,
                                                                      contributetonetpay = pe.contributetonetpay,
                                                                      isemployertax = pe.isemployertax,
                                                                      isemployerdeduction = pe.isemployerdeduction,
                                                                  }).ToList();
                        configurationResponseModel.filename = model.paygroup + "_PE_" + _dateTimeHelper.GetDateTimeNow().ToString("yyyyMMdd") + "_" + _dateTimeHelper.GetDateTimeNow().ToString("HHmmss");
                        break;
                    case "paycalendar":
                        // Code to handle paycalendar
                        //HandlePayCalendar(model.paycalendar);
                        configurationResponseModel.paycalendar = (from P in _appDbContext.paycalendar
                                                                  join PG in _appDbContext.paygroup on P.paygroupid equals PG.id into jps
                                                                  from PGResult in jps.DefaultIfEmpty()
                                                                  where PGResult.code == model.paygroup && P.year == model.year

                                                                  select new PayCalendarModel()
                                                                  {
                                                                      paygroupcode = PGResult != null ? PGResult.code : "",
                                                                      payperiod = P.payperiod,
                                                                      months = P.months,
                                                                      date = P.date,
                                                                      year = P.year,
                                                                      cutoffhours = P.cutoffhours,
                                                                      tasknamelocal = P.tasknamelocal,
                                                                      taskname = P.taskname,
                                                                      frequency = P.frequency,
                                                                      taskid = P.taskid

                                                                  }).OrderBy(p => p.payperiod).ThenBy(p => p.taskid).ToList();
                        configurationResponseModel.filename = model.paygroup + "_PC_" + _dateTimeHelper.GetDateTimeNow().ToString("yyyyMMdd") + "_" + _dateTimeHelper.GetDateTimeNow().ToString("HHmmss");
                        break;

                    // Add additional cases for other configuration names if needed
                    default:
                        // Code to handle unknown or default configuration names
                        //HandleUnknownConfiguration();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception {ex}", ex);
            }
            return configurationResponseModel;
        }
        public string GetTemplateForConfigurationReport(string? configName)
        {
            switch (configName.ToLower())
            {
                case "payrollelements":
                    return Constants.payrollelementsreportExcelTemplate;
                case "paycalendar":
                    return Constants.paycalendarreportExcelTemplate;  // Replace with the actual constant for pay calendar
                default:
                    return null;
            }
        }
        public virtual async Task<string> GetConfigurationReportAsync(LoggedInUser loggedInUser, Task<ConfigurationResponseModel> configList, ConfigurationRequestModel model, string filename)
        {
            // Step 1: Create and set up the DataTable with your columns
            DataTable dt = new DataTable();
            switch (filename)
            {
                case Constants.payrollelementsreportExcelTemplate:
                    dt.Columns.Add("Pay Group", typeof(string));
                    dt.Columns.Add("Pay Element Name Local", typeof(string));
                    dt.Columns.Add("Pay Element Name", typeof(string));
                    dt.Columns.Add("Local Pay Code", typeof(string));
                    dt.Columns.Add("Partner Pay Code", typeof(string));
                    dt.Columns.Add("Tax Authority Code", typeof(string));
                    dt.Columns.Add("Status", typeof(string));
                    dt.Columns.Add("Item Type", typeof(string));
                    dt.Columns.Add("Format", typeof(string));
                    dt.Columns.Add("GL Credit Code", typeof(string));
                    dt.Columns.Add("GL Debit Code", typeof(string));
                    dt.Columns.Add("Client Reported", typeof(string));
                    dt.Columns.Add("Payslip Print", typeof(string));
                    dt.Columns.Add("Comments", typeof(string));
                    dt.Columns.Add("From", typeof(string));
                    dt.Columns.Add("To", typeof(string));
                    dt.Columns.Add("Contributes To Net Pay", typeof(string));
                    dt.Columns.Add("IsEmployerTax", typeof(string));
                    dt.Columns.Add("IsEmployerDeduction", typeof(string));
                    foreach (var payrollElement in configList.Result.payelements) // Replace 'yourDataSource' with your actual data source
                    {
                        dt.Rows.Add(
                            payrollElement.paygroupcode,
                            payrollElement.namelocal,
                            payrollElement.name,
                            payrollElement.code,
                            payrollElement.exportcode,
                            payrollElement.taxauthoritycode,
                            payrollElement.status,
                            payrollElement.itemtype,
                            payrollElement.format,
                            payrollElement.glcreditcode,
                            payrollElement.gldebitcode,
                            payrollElement.clientreported,
                            payrollElement.payslipprint,
                            payrollElement.comments,
                            payrollElement.froms,
                            payrollElement.tos,
                            payrollElement.contributetonetpay,
                            payrollElement.isemployertax,
                            payrollElement.isemployerdeduction
                        );
                    }
                    break;

                case Constants.paycalendarreportExcelTemplate:
                    dt.Columns.Add("PayGroupXrefCode", typeof(string));
                    dt.Columns.Add("Task ID", typeof(string));
                    dt.Columns.Add("Task Name", typeof(string));
                    dt.Columns.Add("Local Task Name", typeof(string));
                    dt.Columns.Add("Date", typeof(DateTime));
                    dt.Columns.Add("Cut-Off Hour", typeof(DateTime));
                    dt.Columns.Add("PayPeriod", typeof(int));
                    dt.Columns.Add("Month", typeof(string));
                    dt.Columns.Add("Frequency", typeof(string));
                    dt.Columns.Add("Year", typeof(int));

                    foreach (var paycalendar in configList.Result.paycalendar) // Replace 'yourDataSource' with your actual data source
                    {
                        DateTime cutOffDateTime;
                        if (DateTime.TryParse(paycalendar.cutoffhours, out cutOffDateTime))
                        {
                            // If parsing succeeds, use the parsed DateTime
                            cutOffDateTime = cutOffDateTime.Date.Add(cutOffDateTime.TimeOfDay);
                        }
                        else
                        {
                            // If parsing fails, set a default time (e.g., midnight)
                            cutOffDateTime = DateTime.Today;
                        }
                        dt.Rows.Add(
                            paycalendar.paygroupcode,
                            paycalendar.taskid,
                            paycalendar.taskname,
                            paycalendar.tasknamelocal,
                            paycalendar.date,
                            cutOffDateTime,
                            paycalendar.payperiod,
                            paycalendar.months,
                            paycalendar.frequency,
                            paycalendar.year
                        );
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid template provided.");
            }

            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = _reportTemplateBucketName,
                Key = filename
            };
            using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
            using (Stream responseStream = response.ResponseStream)
            using (MemoryStream memStream = new MemoryStream())
            {
                responseStream.CopyTo(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                memStream.Position = 0;

                using (var wb = new XLWorkbook(memStream))
                {

                    var sheet = wb.Worksheet(1);
                    sheet.Name = model.configname;

                    var table = sheet.Cell(1, 1).InsertTable(dt);
                    table.HeadersRow().Style.Font.FontColor = XLColor.White;
                    wb.Worksheet(1).Tables.FirstOrDefault().ShowAutoFilter = false;

                    table.Row(1).Style.Fill.SetBackgroundColor(XLColor.MediumElectricBlue).Font.SetFontColor(XLColor.White);

                    for (int i = 1; i <= (dt.Rows.Count) + 1; i++)
                    {
                        wb.Worksheet(1).Row(i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                    if (filename == Constants.paycalendarreportExcelTemplate)
                    {
                        int cutOffHourColumnIndex = dt.Columns["Cut-Off Hour"].Ordinal + 1;
                        var cutOffHourColumn = sheet.Column(cutOffHourColumnIndex);
                        cutOffHourColumn.Style.NumberFormat.Format = "hh:mm:ss";

                        for (int row = 2; row <= sheet.LastRowUsed().RowNumber(); row++)
                        {
                            var cell = sheet.Cell(row, cutOffHourColumnIndex);
                            if (cell.Value.IsDateTime)
                            {
                                DateTime dateTime = cell.Value.GetDateTime();
                                cell.Value = dateTime.TimeOfDay.TotalDays;
                            }
                        }
                    }

                    using (var ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        var base64String = Convert.ToBase64String(ms.ToArray());
                        return base64String;
                    }
                }
            }
            return null;
            // Step 2: Populate the DataTable with data (you need to provide the data source)

            return null;
        }

        public virtual List<SystemUserResponseModel> GetSystemUserReport(LoggedInUser user, SystemUserRequestModel _data)
        {

            List<SystemUserResponseModel> lstSystemUserResponseModel = new List<SystemUserResponseModel>();
            try
            {
                lstSystemUserResponseModel = (
                                                from U in _appDbContext.users
                                                join UPG in _appDbContext.userpaygroupassignment on U.Id equals UPG.UserId into UPGResult
                                                from UU2Result in UPGResult.DefaultIfEmpty()
                                                join P in _appDbContext.paygroup on UU2Result.PaygroupId equals P.id into PP
                                                from PPResult in PP.DefaultIfEmpty()
                                                join L in _appDbContext.legalentity on PPResult.legalentityid equals L.id into LL
                                                from LLResult in LL.DefaultIfEmpty()
                                                join C in _appDbContext.client on LLResult.clientid equals C.id into CC
                                                from CCResult in CC.DefaultIfEmpty()
                                                where U.Status == "ACTIVE" &&
                                                    (U.Role == "superuser" ||
                                                    (PPResult != null && PPResult.code == _data.paygroup))
                                                select new SystemUserResponseModel()
                                                {
                                                    Userid = U.UserId,
                                                    Clientname = CCResult.code,
                                                    Paygroup = PPResult.code,
                                                    FirstName = U.FirstName,
                                                    LastName = U.LastName,
                                                    UserStatus = U.Status == "ACTIVE" ? "Active" : "Inactive",
                                                    UserRole = _selectListHelper.GetDisplayValue(U.Role),
                                                    UserGroup = U.UserGroup,
                                                    Email = U.Email,
                                                    UserLastLoginDate = U.LastLoggedOn

                                                }).OrderBy(x => x.Userid).ToList();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return lstSystemUserResponseModel;
        }
        public virtual List<TransactionResponseModel> GetTransactionByPayGroupReport(LoggedInUser user, TransactionRequestModel _data)
        {

            var lstTransactionResponseModel = new List<TransactionResponseModel>();
            try
            {
                if (string.IsNullOrEmpty(_data.endpp))
                {
                    var query = from pc in _appDbContext.paycalendar
                                join pg in _appDbContext.paygroup on pc.paygroupid equals pg.id
                                where pc.taskid == "SD"
                                  && pc.date <= _dateTimeHelper.GetDateTimeNow()
                                orderby pc.date descending
                                select pc.payperiod;

                    var payPeriod = query.FirstOrDefault();

                    _data.endpp = payPeriod != 0 ? payPeriod.ToString() : "";
                }

                var paygroupCodeParam = new NpgsqlParameter("@paygroupcode", _data.paygroup);
                var startPPParam = new NpgsqlParameter("@startpp", _data.startpp);
                var endPPParam = new NpgsqlParameter("@endpp", _data.endpp);

                string sqlQuery = $"SELECT * FROM dbo.gettransactionbypaygroupreport(@paygroupcode, @startpp, @endpp)";
                lstTransactionResponseModel = _appDbContext.TransactionResponseModel
                                   .FromSqlRaw(sqlQuery, paygroupCodeParam, startPPParam, endPPParam)
                                   .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return lstTransactionResponseModel;
        }
        public virtual List<TransactionCountryResponseModel> GetTransactionByCountryReport(LoggedInUser user, TransactionCountryRequestModel _data)
        {

            List<TransactionCountryResponseModel> lstTransactionCountryResponseModel = new List<TransactionCountryResponseModel>();
            try
            {
                lstTransactionCountryResponseModel = (

                    from E in _appDbContext.employee
                    join J in _appDbContext.employeejob on new { E.employeeid, E.paygroupid } equals new { J.employeeid, J.paygroupid } into EJ
                    from EJResult in EJ.DefaultIfEmpty()
                    join P in _appDbContext.paygroup on E.paygroupid equals P.id into PG
                    from PGResult in PG.DefaultIfEmpty()
                    join C in _appDbContext.paycalendar on E.paygroupid equals C.paygroupid into PC
                    from PCResult in PC.DefaultIfEmpty()
                    join G in _appDbContext.gpripayrunimports on E.employeeid equals G.employeexrefcode into GP
                    from GPResult in GP.DefaultIfEmpty()
                    join PR in _appDbContext.payrollelements on E.paygroupid equals PR.paygroupid into PRE
                    from PREResult in PRE.DefaultIfEmpty()
                    join LE in _appDbContext.legalentity on PGResult.legalentityid equals LE.id into LEI
                    from LEResult in LEI.DefaultIfEmpty()
                    join CL in _appDbContext.client on LEResult.clientid equals CL.id into CLI
                    from CLResult in CLI.DefaultIfEmpty()
                    join CO in _appDbContext.country on PGResult.countryid equals CO.id into COI
                    from COResult in COI.DefaultIfEmpty()

                    where GPResult.itemcode == PREResult.code &&
                            CLResult.code == (_data.clientname != null ? _data.clientname : CLResult.code)

                    select new TransactionCountryResponseModel()
                    {
                        ClientName = CLResult.code,
                        PayGroup = PGResult.code,
                        PayrollYear = GPResult.paydate.Value.ToString("yyyy"),
                        PayPeriod = GPResult.payperiod.ToString(),
                        PPStartDate = GPResult.payperiodstart.Value.ToString("yyyy-MM-dd"),
                        PPEndDate = GPResult.payperiodend.Value.ToString("yyyy-MM-dd"),
                        PayDate = GPResult.paydate.Value.ToString("yyyy-MM-dd"),
                        LastName = E.lastname,
                        SecondLastName = E.secondlastname,
                        FirstName = E.firstname,
                        MiddleName = E.middlenames,
                        EmployeeID = E.employeeid,
                        PayElementName = PREResult.name,
                        PayElementNameLocal = PREResult.namelocal,
                        PayElementCode = PREResult.code,
                        ExportCode = PREResult.exportcode,
                        ElementType = PREResult.itemtype,
                        GLCreditCode = PREResult.glcreditcode,
                        GLDebitCode = PREResult.gldebitcode,
                        From = PREResult.froms,
                        To = PREResult.tos,
                        Amount = GPResult.itemamount,
                        CostCenter = EJResult.costcenter,
                        Orglevel1 = EJResult.orgunit1,
                        Orglevel2 = EJResult.orgunit2,
                        Orglevel3 = EJResult.orgunit3,
                        Orglevel4 = EJResult.orgunit4,
                        Countryid = PGResult.countryid,
                        Country = COResult.code,
                    }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return lstTransactionCountryResponseModel;
        }

        public virtual List<VarianceResponseModel> GetVarianceReport(LoggedInUser user, VarianceRequestModel _data)
        {

            List<VarianceResponseModel> lstVarianceResponseModel = new List<VarianceResponseModel>();
            try
            {
                lstVarianceResponseModel = (
                from GP in _appDbContext.gpripayrunimports
                join E in _appDbContext.employee on GP.employeexrefcode equals E.employeeid into EE
                from EEResult in EE.DefaultIfEmpty()
                join PE in _appDbContext.payrollelements on GP.itemcode equals PE.code into PEE
                from PEResult in PEE.DefaultIfEmpty()
                join G in _appDbContext.gpri on GP.requestid equals G.id into GE
                from GEResult in GE.DefaultIfEmpty()
                join PG in _appDbContext.paygroup on PEResult.paygroupid equals PG.id into PGE
                from PGResult in PGE.DefaultIfEmpty()
                where PGResult.code == _data.paygroup
                && Convert.ToInt32(_data.payperiod) == GP.payperiod
                && GP.isoffcycle == 0
                && PGResult.id == PEResult.paygroupid
                select new VarianceResponseModel()
                {
                    PayGroup = PGResult.code,
                    LastName = EEResult.lastname,
                    MiddleName = EEResult.middlenames,
                    FirstName = EEResult.firstname,
                    SecondLastName = EEResult.secondlastname,
                    EmpolyeeID = EEResult.employeeid,
                    PayElementNameLocal = PEResult.namelocal,
                    PayElementName = PEResult.name,
                    PayElementCode = PEResult.code,
                    ExportCode = PEResult.exportcode,
                    NewAmount = GP.itemamount,
                    NewPeriod = _data.year + "-" + _data.payperiod,
                }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return lstVarianceResponseModel;
        }
        public virtual List<PayPeriodWithPrevioudCutOff> GetPayPeriodCutoff(LoggedInUser user, int paygroupid, int year, int? payperiod)
        {
            var query = GetCutOffDateForPayPeriodAndPrevious(user, paygroupid, year, payperiod);

            var cutOffResult = _appDbContext.payperiodcutoffs.FromSqlRaw(query).ToList();
            return cutOffResult;
        }
        public virtual VariancePayPeriodDetails GetPreviousPayPeriod(LoggedInUser user, int iPayGroupID, int iPayPeriodYear)
        {
            VariancePayPeriodDetails objPD = new VariancePayPeriodDetails();

            try
            {
                var resposne = (from g in _appDbContext.Set<GPRI>().Where(x => x.paygroupid == iPayGroupID && x.payperiodyear < iPayPeriodYear)
                                select new VariancePayPeriodDetails()
                                {
                                    PayPeriod = g.payperiod,
                                    PayPeriodYear = g.payperiodyear
                                }).OrderByDescending(x => x.PayPeriodYear).ThenByDescending(x => x.PayPeriod).FirstOrDefault();

                objPD = resposne;


            }
            catch (Exception ex)
            { }

            return objPD;
        }
        public virtual List<ErrorLogResponseModel> GetErrorLogReport(LoggedInUser user, ErrorLogRequestModel _data)
        {  
            List<ErrorLogResponseModel> lstErrorLogResponseModel = new List<ErrorLogResponseModel>();
            try
            {
                if (_data.enddate == null)
                {
                    _data.enddate = _dateTimeHelper.GetDateTimeNow();
                }
                DateTime? startDateNullable = _data.startdate;
                DateTime? endDateNullable = _data.enddate;
                var startTime = _data.startdate.Value.TimeOfDay == TimeSpan.Zero ? TimeSpan.Zero : _data.startdate.Value.TimeOfDay;
                var endTime = _data.enddate.Value.TimeOfDay == TimeSpan.Zero ? new TimeSpan(23, 59, 59) : _data.enddate.Value.TimeOfDay;
                if (_data.startdate.HasValue && _data.startdate.Value.TimeOfDay == TimeSpan.Zero)
                {
                    startDateNullable = new DateTime(_data.startdate.Value.Year, _data.startdate.Value.Month, _data.startdate.Value.Day, startTime.Hours, startTime.Minutes, startTime.Seconds);
                }
                if (_data.enddate.HasValue && _data.enddate.Value.TimeOfDay == TimeSpan.Zero)
                {
                    endDateNullable = new DateTime(_data.enddate.Value.Year, _data.enddate.Value.Month, _data.enddate.Value.Day, endTime.Hours, endTime.Minutes, endTime.Seconds);
                }
                if (_data.paygroup == null && _data.startdate == null & _data.enddate == null)
                {
                    lstErrorLogResponseModel = (from EL in _appDbContext.errorlog
                                                join PG in _appDbContext.paygroup on EL.paygroupid equals PG.id
                                                join RD in _appDbContext.requestdetails on EL.requestdetailsid equals RD.id
                                                where PG.status == "Active" && EL.log != "Success"

                                                select new ErrorLogResponseModel()
                                                {
                                                    id = EL.id,
                                                    RequestDetailsId = EL.requestdetailsid,
                                                    PayGroup = PG.code,
                                                    EmployeeID = EL.employeeid,
                                                    EventType = EL.eventtype,
                                                    EntityState = EL.entitystate,
                                                    Log = EL.log,
                                                    TableName = EL.tablename,
                                                    Field = EL.field,
                                                    Description = EL.description,
                                                    Type = EL.type,
                                                    File = RD.s3objectid,

                                                    ErrorRepotedOn = EL.createdat.ToString("yyyy-MM-dd")
                                                }).ToList();
                }
                else
                {
                    lstErrorLogResponseModel = (from EL in _appDbContext.errorlog
                                                join PG in _appDbContext.paygroup on EL.paygroupid equals PG.id
                                                join RD in _appDbContext.requestdetails on EL.requestdetailsid equals RD.id
                                                where PG.status == "Active" && EL.log != "Success"
                                                && (EL.createdat >= (startDateNullable != null ? startDateNullable : EL.createdat) && EL.createdat < (endDateNullable != null ? endDateNullable : EL.createdat))
                                                && PG.code == (_data.paygroup != null ? _data.paygroup : PG.code)


                                                select new ErrorLogResponseModel()
                                                {
                                                    id = EL.id,
                                                    PayGroup = PG.code,
                                                    RequestDetailsId = EL.requestdetailsid,
                                                    EmployeeID = EL.employeeid,
                                                    EventType = EL.eventtype,
                                                    EntityState = EL.entitystate,
                                                    Log = EL.log,
                                                    TableName = EL.tablename,
                                                    Field = EL.field,
                                                    Description = EL.description,
                                                    Type = EL.type,
                                                    File = RD.s3objectid,
                                                    ErrorRepotedOn = EL.createdat.ToString("yyyy-MM-dd"),
                                                }).ToList();

                }
            }
            catch (Exception ex)
            {
                InsertError(user, 0, "Application Service", "400", "GetErrorLogReport", ex.ToString());
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return lstErrorLogResponseModel;
        }

        public virtual async Task<string> GetCalendarReport(LoggedInUser user, CalendarRequestModel requestModel, string fileName)
        {
            try
            {
                var results = _appDbContext.paycalendar.Join(
                    _appDbContext.paygroup,
                     pc => pc.paygroupid,
                     pg => pg.id,
                    (pc, pg) => new
                    {
                        PayPeriod = pc.payperiod,
                        Date = pc.date,
                        TaskId = pc.taskid,
                        TaskName = pc.taskname,
                        Months = pc.months,
                        Frequency = pc.frequency,
                        PayGroupCode = pg.code,
                        Year = pc.year
                    })
                .Where(result => result.PayGroupCode == requestModel.paygroup && result.Year == requestModel.year)
                .ToList();

                var calendarDictionary = new Dictionary<string, CalendarReport>();

                var groupedResults = results.GroupBy(r => r.PayPeriod).OrderBy(r => r.Key);
                foreach (var group in groupedResults)
                {
                    string payPeriod = group.Key.ToString();
                    var calendarEntry = new CalendarReport { PayPeriod = payPeriod };
                    var month = group.First().Months;
                    calendarEntry.Months = month;
                    foreach (var item in group)
                    {
                        switch (item.TaskId)
                        {
                            case "SD":
                                calendarEntry.PeriodStartDate = item.Date;
                                break;
                            case "ED":
                                calendarEntry.PeriodEndDate = item.Date;
                                break;
                            case "OSD":
                                calendarEntry.OffSetStartDate = item.Date;
                                break;
                            case "OED":
                                calendarEntry.OffSetEndDate = item.Date;
                                break;
                            case "19":
                                calendarEntry.PayDate = item.Date;
                                break;
                        }
                    }

                    calendarDictionary[payPeriod] = calendarEntry;
                }

                DataTable dt = new DataTable();
                var headers = new List<string>
                {
                    "Period Start Date",
                    "Period End Date",
                    "Offset Start Date",
                    "Offset End Date",
                    "Pay Date"
                };
                var payperiodlist = results.Select(r => r.PayPeriod).Distinct().OrderBy(pp => pp).ToList();
                dt.Columns.Add("Pay Period", typeof(string));
                for (int i = 0; i < payperiodlist.Count; i++)
                {
                    dt.Columns.Add(payperiodlist[i].ToString(), (typeof(string)));
                }
                foreach (var header in headers)
                {
                    dt.Rows.Add(header, (typeof(string)));
                }

                int columnIndex = 1;
                foreach (var entry in calendarDictionary.Values)
                {
                    dt.Rows[0][columnIndex] = entry.PeriodStartDate.HasValue ? entry.PeriodStartDate.Value.ToString("yyyy-MM-dd") : "";
                    dt.Rows[1][columnIndex] = entry.PeriodEndDate.HasValue ? entry.PeriodEndDate.Value.ToString("yyyy-MM-dd") : "";
                    dt.Rows[2][columnIndex] = entry.OffSetStartDate.HasValue ? entry.OffSetStartDate.Value.ToString("yyyy-MM-dd") : "";
                    dt.Rows[3][columnIndex] = entry.OffSetEndDate.HasValue ? entry.OffSetEndDate.Value.ToString("yyyy-MM-dd") : "";
                    dt.Rows[4][columnIndex] = entry.PayDate.HasValue ? entry.PayDate.Value.ToString("yyyy-MM-dd") : "";

                    columnIndex++;
                }
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };
                string clientcode = await _reportServiceHelper.GetClientCodeByPayGroup(requestModel.paygroup);
                requestModel.clientname = clientcode;
                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {

                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;

                    using (var wb = new XLWorkbook(memStream))
                    {
                        if (wb.Worksheets.Count > 1)
                        {
                            var sheetToRemove = wb.Worksheet("sheet1");
                            if (sheetToRemove != null)
                            {
                                wb.Worksheets.Delete("sheet1");
                            }
                        }

                        var sheetname = _reportServiceHelper.GenerateSheetNameForPC(requestModel.paygroup, requestModel.year, "Calendar");
                        var sheet = wb.Worksheet(1);
                        sheet.Name = sheetname;

                        wb.Worksheet(1).Cell(7, 2).Value = requestModel != null ? requestModel.clientname : "";
                        wb.Worksheet(1).Cell(8, 2).Value = requestModel != null ? requestModel.paygroup : "";
                        wb.Worksheet(1).Cell(9, 2).Value = requestModel != null ? requestModel.year : "";

                        for (int i = 7; i <= 9; i++)
                        {
                            wb.Worksheet(1).Row(i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }
                        sheet.Cell(11, 1).Value = "Month";

                        int colIndex = 2;
                        foreach (var month in calendarDictionary.Values)
                        {
                            sheet.Cell(11, colIndex).Value = month.Months.ToString();
                            colIndex++;
                        }
                        var headerRange = sheet.Row(11).Cells(1, colIndex - 1);
                        headerRange.Style.Font.SetBold(true);
                        headerRange.Style.Fill.BackgroundColor = XLColor.Orange;
                        headerRange.Style.Font.SetFontColor(XLColor.White);
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        var tableStartRow = 12;
                        var table = sheet.Cell(tableStartRow, 1).InsertTable(dt);
                        table.HeadersRow().Style.Font.FontColor = XLColor.Black;
                        wb.Worksheet(1).Tables.FirstOrDefault().ShowAutoFilter = false;

                        table.Column(1).Style.Fill.SetBackgroundColor(XLColor.MediumElectricBlue).Font.SetFontColor(XLColor.White);

                        for (int i = tableStartRow; i <= (dt.Rows.Count) + tableStartRow; i++)
                        {
                            wb.Worksheet(1).Row(i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            var base64String = Convert.ToBase64String(ms.ToArray());
                            return base64String;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public virtual string GetPayFrequencyForPayGroup(LoggedInUser user, string PayGroupCode)
        {
            string strPayFrequency = string.Empty;
            try
            {
                var PayFrequency = (from PG in _appDbContext.paygroup
                                    join PF in _appDbContext.payfrequency on PG.payfrequencyid equals PF.id
                                    where PG.code == PayGroupCode

                                    select new
                                    {
                                        PF.code
                                    }
                                    ).Distinct().FirstOrDefault();

                strPayFrequency = PayFrequency.code.ToString();
            }
            catch (Exception ex)
            { }
            return strPayFrequency;

        }
        public async Task<(string?, string?)> GetStartAndEndTime(LoggedInUser loggedInUser, StartEndTimeRequestModel requestBody)
        {

            int paygroupid = GetPayGroupIDByCode(loggedInUser, requestBody.paygroup);
            var res = _appDbContext.paycalendar
                .Where(p => p.paygroupid == paygroupid
                    && p.year == requestBody.year
                    && p.payperiod == requestBody.payperiod
                    && (p.taskid == "SD" || p.taskid == "ED"))
                .Select(p => new
                {
                     p.taskid,
                     p.cutoffhours
                })
                .ToList();

            string? starttime = res.FirstOrDefault(p => p.taskid == "SD")?.cutoffhours?.Substring(0, 5);
            string? endtime = res.FirstOrDefault(p => p.taskid == "ED")?.cutoffhours?.Substring(0, 5);

            return (starttime, endtime);
        }

        protected virtual List<CalendarResponseModel> GetCalendarResponseModel(LoggedInUser user, CalendarRequestModel _data)
        {
            List<CalendarResponseModel> lstCalendarResponseModel = new List<CalendarResponseModel>();
            lstCalendarResponseModel = (from PC in _appDbContext.paycalendar
                                        join PG in _appDbContext.paygroup on PC.paygroupid equals PG.id
                                        where (PC.year == _data.year)
                                        //&& filterKey.Contains(PC.taskid.ToString()) 
                                        && PG.code == _data.paygroup

                                        select new CalendarResponseModel()
                                        {
                                            Month = PC.months,
                                            PayPeriod = PC.payperiod,
                                            PeriodStartDate = PC.date,
                                            TaskName = PC.taskname
                                        }).Distinct().ToList();
            return lstCalendarResponseModel;

        }
        public virtual async Task<string> GetHybridChageFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName)
        {
            var paygroupId = await _appDbContext.paygroup.Where(e => e.code == requestModel.PaygroupCode).Select(e => e.id).FirstOrDefaultAsync();
            requestModel.EndDate = requestModel.EndDate != null ? requestModel.EndDate.Value : _dateTimeHelper.GetDateTimeNow();
            var startTime = requestModel.StartDate.Value.TimeOfDay == TimeSpan.Zero ? TimeSpan.Zero : requestModel.StartDate.Value.TimeOfDay;
            var endTime = requestModel.EndDate.Value.TimeOfDay == TimeSpan.Zero ? new TimeSpan(23, 59, 59) : requestModel.EndDate.Value.TimeOfDay;
            try
            {
                if (requestModel.FilterOption.ToLower() == "pp" && paygroupId != 0 && requestModel.Year != null && requestModel.PayPeriod != null)
                {
                    var query = GetCutOffDateForPayPeriodAndPrevious(user, paygroupId, requestModel.Year.Value, requestModel.PayPeriod.Value);

                    var cutOffResult = await _appDbContext.payperiodcutoffs.FromSqlRaw(query).ToListAsync();

                    if (cutOffResult != null && cutOffResult.Count > 0)
                    {
                        if (cutOffResult[0].PreviousPayPeriod != null)
                        {
                            _logger.LogInformation("Pay period data has been retrieved as {pid}, {pcutoff}, {cid}, {ccutoff}", cutOffResult[0].PreviousId, cutOffResult[0].PreviousCutOffDate, cutOffResult[0].CurrentId, cutOffResult[0].CurrentCutOffDate);

                            if (cutOffResult[0].PreviousCutOffDate.HasValue && cutOffResult[0].CurrentCutOffDate.HasValue)
                            {
                                requestModel.StartDate = _dateTimeHelper.SetDateTimeWithTime(cutOffResult[0].PreviousCutOffDate, startTime);
                                requestModel.EndDate = _dateTimeHelper.SetDateTimeWithTime(cutOffResult[0].CurrentCutOffDate, endTime);
                            }
                            else
                            {
                                _logger.LogInformation("Current Payperiod Cutoff date is Empty");
                                return "Current Cutoff date doesnt exist";
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Previous Payperiod Cutoff date is Empty");
                            return "Previous Pay Period doesnt exist";
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Cutoff data is Empty");
                        var query2 = GetPreviouspayperioddata(user, paygroupId, requestModel.Year.Value, requestModel.PayPeriod.Value);
                        var cutOffResult2 = await _appDbContext.paycalendar.FromSqlRaw(query2).ToListAsync();

                        if (cutOffResult2.Count > 0)
                        {
                            _logger.LogInformation("prevexists");
                            return "prevexists";
                        }
                        else
                        {
                            _logger.LogInformation("GetPreviouspayperioddata Empty");
                            return "Previous Pay Period doesnt exist";
                        }
                    }
                }
                else
                {
                    requestModel.StartDate = _dateTimeHelper.SetDateTimeWithTime(requestModel.StartDate, startTime);
                    requestModel.EndDate = _dateTimeHelper.SetDateTimeWithTime(requestModel.EndDate, endTime);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred: {exceptionMessage}", ex.Message);
                throw ex;
            }
            var data = await GetHybridChangeFileData(user, paygroupId, requestModel.StartDate, requestModel.EndDate);

            var excelHelper = new ExportToExcelHelper();
            var interfacetype = _reportServiceHelper.GetInterfaceType(paygroupId);
            var result = await GetPeriodChangeFile(user, data, requestModel, Constants.hybridchangefilereportExcelTemplate, interfacetype, sheetName);
            return result ?? string.Empty;
        }
        public virtual async Task<string> GetPeriodChangeFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName)
        {
            var paygroupId = await _appDbContext.paygroup.Where(e => e.code == requestModel.PaygroupCode).Select(e => e.id).FirstOrDefaultAsync();
            requestModel.EndDate = requestModel.EndDate != null ? requestModel.EndDate.Value : _dateTimeHelper.GetDateTimeNow();
            var startTime = requestModel.StartDate.Value.TimeOfDay == TimeSpan.Zero ? TimeSpan.Zero : requestModel.StartDate.Value.TimeOfDay;
            var endTime = requestModel.EndDate.Value.TimeOfDay == TimeSpan.Zero ? new TimeSpan(23, 59, 59) : requestModel.EndDate.Value.TimeOfDay;
            try
            {
                if (requestModel.FilterOption.ToLower() == "pp" && paygroupId != 0 && requestModel.Year != null && requestModel.PayPeriod != null)
                {
                    var query = GetCutOffDateForPayPeriodAndPrevious(user, paygroupId, requestModel.Year.Value, requestModel.PayPeriod.Value);

                    var cutOffResult = await _appDbContext.payperiodcutoffs.FromSqlRaw(query).ToListAsync();

                    if (cutOffResult != null && cutOffResult.Count > 0)
                    {
                        if (cutOffResult[0].PreviousPayPeriod != null)
                        {
                            _logger.LogInformation("Pay period data has been retrieved as {pid}, {pcutoff}, {cid}, {ccutoff}", cutOffResult[0].PreviousId, cutOffResult[0].PreviousCutOffDate, cutOffResult[0].CurrentId, cutOffResult[0].CurrentCutOffDate);

                            if (cutOffResult[0].PreviousCutOffDate.HasValue && cutOffResult[0].CurrentCutOffDate.HasValue)
                            {
                                requestModel.StartDate = _dateTimeHelper.SetDateTimeWithTime(cutOffResult[0].PreviousCutOffDate, startTime);
                                requestModel.EndDate = _dateTimeHelper.SetDateTimeWithTime(cutOffResult[0].CurrentCutOffDate, endTime);

                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                    else
                    {
                        var query2 = GetPreviouspayperioddata(user, paygroupId, requestModel.Year.Value, requestModel.PayPeriod.Value);
                        var cutOffResult2 = await _appDbContext.paycalendar.FromSqlRaw(query2).ToListAsync();

                        if (cutOffResult2.Count > 0)
                        {
                            return "prevexists";
                        }
                        else
                        {
                            return "Previous Pay Period doesnt exist";
                        }
                    }
                }
                else
                {
                    requestModel.StartDate = _dateTimeHelper.SetDateTimeWithTime(requestModel.StartDate, startTime);
                    requestModel.EndDate = _dateTimeHelper.SetDateTimeWithTime(requestModel.EndDate, endTime);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred: {exceptionMessage}", ex.Message);
            }
            var data = await GetPeriodChangeFileData(user, paygroupId, requestModel.StartDate, requestModel.EndDate);

            var excelHelper = new ExportToExcelHelper();
            var interfacetype = _reportServiceHelper.GetInterfaceType(paygroupId);
            var result = await GetPeriodChangeFile(user, data, requestModel, Constants.periodchangefilereportExcelTemplate, interfacetype, sheetName);
            return result ?? string.Empty;
        }

        public virtual async Task<string> GetCenamPeriodChangeFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName)
        {
            var paygroup = await _appDbContext.paygroup.Where(e => e.code == requestModel.PaygroupCode).FirstOrDefaultAsync();
            var countrycode = await _appDbContext.country.Where(c => c.id == paygroup.countryid).Select(c => c.code).FirstOrDefaultAsync();
            requestModel.EndDate = requestModel.EndDate != null ? requestModel.EndDate.Value : _dateTimeHelper.GetDateTimeNow();
            var startTime = requestModel.StartDate.Value.TimeOfDay == TimeSpan.Zero ? TimeSpan.Zero : requestModel.StartDate.Value.TimeOfDay;
            var endTime = requestModel.EndDate.Value.TimeOfDay == TimeSpan.Zero ? new TimeSpan(23, 59, 59) : requestModel.EndDate.Value.TimeOfDay;
            try
            {
                List<string> validCountryCodes = new List<string> { "CRI", "DOM", "GTM", "HND", "NIC", "PAN", "SLV" };

                if (countrycode == null || !validCountryCodes.Contains(countrycode))
                {
                    return $"Pay Groups country code is neither CRI, DOM, GTM, HND, NIC, PAN, or SLV";
                }
                if (requestModel.FilterOption.ToLower() == "pp" && paygroup.id != 0 && requestModel.Year != null && requestModel.PayPeriod != null)
                {
                    var query = GetCutOffDateForPayPeriodAndPrevious(user, paygroup.id, requestModel.Year.Value, requestModel.PayPeriod.Value);

                    var cutOffResult = await _appDbContext.payperiodcutoffs.FromSqlRaw(query).ToListAsync();

                    if (cutOffResult != null && cutOffResult.Count > 0)
                    {
                        if (cutOffResult[0].PreviousPayPeriod != null)
                        {
                            _logger.LogInformation("Pay period data has been retrieved as {pid}, {pcutoff}, {cid}, {ccutoff}", cutOffResult[0].PreviousId, cutOffResult[0].PreviousCutOffDate, cutOffResult[0].CurrentId, cutOffResult[0].CurrentCutOffDate);

                            if (cutOffResult[0].PreviousCutOffDate.HasValue && cutOffResult[0].CurrentCutOffDate.HasValue)
                            {
                                requestModel.StartDate = _dateTimeHelper.SetDateTimeWithTime(cutOffResult[0].PreviousCutOffDate, startTime);
                                requestModel.EndDate = _dateTimeHelper.SetDateTimeWithTime(cutOffResult[0].CurrentCutOffDate, endTime);
                            }
                            else
                            {
                                return "Current Cutoff date doesnt exist";
                            }
                        }
                        else
                        {
                            return "Previous Pay Period doesnt exist";
                        }
                    }
                    else
                    {
                        var query2 = GetPreviouspayperioddata(user, paygroup.id, requestModel.Year.Value, requestModel.PayPeriod.Value);
                        var cutOffResult2 = await _appDbContext.paycalendar.FromSqlRaw(query2).ToListAsync();

                        if (cutOffResult2.Count > 0)
                        {
                            return "prevexists";
                        }
                        else
                        {
                            return "Previous Pay Period doesnt exist";
                        }
                    }
                }
                else
                {
                    requestModel.StartDate = _dateTimeHelper.SetDateTimeWithTime(requestModel.StartDate, startTime);
                    requestModel.EndDate = _dateTimeHelper.SetDateTimeWithTime(requestModel.EndDate, endTime);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred: {exceptionMessage}", ex.Message);
            }

            _logger.LogInformation("GetCenamPeriodChangeFileData will be executed after this.");

            var dataTables = await GetCenamPeriodChangeFileData(user, paygroup.id, requestModel.StartDate, requestModel.EndDate, countrycode);
            string reportTemplate = countrycode switch
            {
                "CRI" => Constants.CENAM_CRI_Template,
                "PAN" => Constants.CENAM_PAN_Template,
                "SLV" => Constants.CENAM_SLV_Template,
                "GTM" => Constants.CENAM_GTM_Template,
                "HND" => Constants.CENAM_HND_NIC_Template,
                "NIC" => Constants.CENAM_HND_NIC_Template,
                "DOM" => Constants.CENAM_DOM_Template,
            };

            var excelHelper = new ExportToExcelHelper();
            var interfacetype = _reportServiceHelper.GetInterfaceType(paygroup.id);
            var result = await ConvertDataTablesToExcelFile(dataTables, requestModel, interfacetype, reportTemplate, countrycode, sheetName, user);
            return result ?? string.Empty;
        }
        public virtual async Task<string> GetMexicoPeriodChangeFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName)
        {
            var paygroup = await _appDbContext.paygroup.Where(e => e.code == requestModel.PaygroupCode).FirstOrDefaultAsync();
            var countrycode = await _appDbContext.country.Where(c => c.id == paygroup.countryid).Select(c => c.code).FirstOrDefaultAsync();
            requestModel.EndDate = requestModel.EndDate != null ? requestModel.EndDate.Value : _dateTimeHelper.GetDateTimeNow();
            var startTime = requestModel.StartDate.Value.TimeOfDay == TimeSpan.Zero ? TimeSpan.Zero : requestModel.StartDate.Value.TimeOfDay;
            var endTime = requestModel.EndDate.Value.TimeOfDay == TimeSpan.Zero ? new TimeSpan(23, 59, 59) : requestModel.EndDate.Value.TimeOfDay;
            try
            {
                if (countrycode == null || (!countrycode.Equals("MEX")))
                {
                    return $"Pay Groups country code is not Mexico";
                }
                if (requestModel.FilterOption.ToLower() == "pp" && paygroup.id != 0 && requestModel.Year != null && requestModel.PayPeriod != null)
                {
                    var query = GetCutOffDateForPayPeriodAndPrevious(user, paygroup.id, requestModel.Year.Value, requestModel.PayPeriod.Value);

                    var cutOffResult = await _appDbContext.payperiodcutoffs.FromSqlRaw(query).ToListAsync();

                    if (cutOffResult != null && cutOffResult.Count > 0)
                    {
                        if (cutOffResult[0].PreviousPayPeriod != null)
                        {
                            _logger.LogInformation("Pay period data has been retrieved as {pid}, {pcutoff}, {cid}, {ccutoff}", cutOffResult[0].PreviousId, cutOffResult[0].PreviousCutOffDate, cutOffResult[0].CurrentId, cutOffResult[0].CurrentCutOffDate);

                            if (cutOffResult[0].PreviousCutOffDate.HasValue && cutOffResult[0].CurrentCutOffDate.HasValue)
                            {
                                requestModel.StartDate = _dateTimeHelper.SetDateTimeWithTime(cutOffResult[0].PreviousCutOffDate, startTime);
                                requestModel.EndDate = _dateTimeHelper.SetDateTimeWithTime(cutOffResult[0].CurrentCutOffDate, endTime);
                            }
                            else
                            {
                                return "Current Cutoff date doesnt exist";
                            }
                        }
                        else
                        {
                            return "Previous Pay Period doesnt exist";
                        }
                    }
                    else
                    {
                        var query2 = GetPreviouspayperioddata(user, paygroup.id, requestModel.Year.Value, requestModel.PayPeriod.Value);
                        var cutOffResult2 = await _appDbContext.paycalendar.FromSqlRaw(query2).ToListAsync();

                        if (cutOffResult2.Count > 0)
                        {
                            return "prevexists";
                        }
                        else
                        {
                            return "Previous Pay Period doesnt exist";
                        }
                    }
                }
                else
                {
                    requestModel.StartDate = _dateTimeHelper.SetDateTimeWithTime(requestModel.StartDate, startTime);
                    requestModel.EndDate = _dateTimeHelper.SetDateTimeWithTime(requestModel.EndDate, endTime);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred: {exceptionMessage}", ex.Message);
            }
            var dataTables = await GetMexicoPeriodChangeFileData(user, paygroup.id, requestModel.StartDate, requestModel.EndDate, countrycode);
            string reportTemplate = Constants.MEXICO_Template;
            var excelHelper = new ExportToExcelHelper();
            var interfacetype = _reportServiceHelper.GetInterfaceType(paygroup.id);
            var result = await ConvertDataTablesToExcelFile(dataTables, requestModel, interfacetype, reportTemplate, countrycode, sheetName, user);
            return result;
        }

        protected static string GetCutOffDateForPayPeriodAndPrevious(LoggedInUser user, int paygroupId, int year, int? payperiod)
        {
            return @$"with cte
                    as
                    (
	                    SELECT id, prev_id, payperiod, date as currentcutoff
	                    FROM (
  		                    SELECT *, LAG(id) OVER (ORDER BY year, payperiod) as prev_id
  		                    FROM dbo.paycalendar p where p.taskid = '3' and paygroupid = {paygroupId}
	                    ) as foo
                    WHERE year = {year} and payperiod = {payperiod}
                    )
                    SELECT C.ID C_ID, C.PAYPERIOD C_PAYPERIOD, C.currentcutoff C_CUTOFFDATE, PN.ID P_ID, PN.PAYPERIOD P_PAYPERIOD, PN.DATE AS P_CUTOFFDATE  
                    FROM CTE C 
                    left outer join dbo.paycalendar pn on C.prev_id = pn.id
                    LIMIT 1
                    ";
        }
        protected static string GetPreviouspayperioddata(LoggedInUser user, int paygroupId, int year, int? payperiod)
        {
            if (payperiod > 1)
            {
                return @$"select * from dbo.paycalendar p where (p.paygroupid={paygroupId}  and p.year={year} and p.payperiod={payperiod - 1} and p.taskid='3') order by p.payperiod desc limit 1";
            }
            return @$"select * from dbo.paycalendar p where (p.paygroupid={paygroupId}  and p.year={year - 1} and p.taskid='3') order by p.payperiod desc limit 1";
        }

        protected async Task<PeriodChangeFileDataModel> GetHybridChangeFileData(LoggedInUser user, int? paygroupId, DateTime? startDate, DateTime? endDate)
        {
            var periodChangeFileDataModel = new PeriodChangeFileDataModel();
            periodChangeFileDataModel.personal = new List<PeriodChangePERS>();
            periodChangeFileDataModel.jobs = new List<PeriodChangeJDET>();
            periodChangeFileDataModel.salary = new List<PeriodChangeSLRY>();
            periodChangeFileDataModel.address = new List<PeriodChangeADDR>();
            periodChangeFileDataModel.bank = new List<PeriodChangeBANK>();
            periodChangeFileDataModel.cspf = new List<PeriodChangeCSPF>();
            periodChangeFileDataModel.conf = new List<PeriodChangeCONF>();
            periodChangeFileDataModel.payD = new List<PeriodChangePAYD>();
            periodChangeFileDataModel.time = new List<PeriodChangeTIME>();
            periodChangeFileDataModel.starters = new List<PeriodChangeStarters>();
            periodChangeFileDataModel.leavers = new List<PeriodChangeLeavers>();
            if (paygroupId == null || startDate == null)
            {
                _logger.LogInformation("Invalid parameters sent to HybridChange file report, ignoring the request");
                return periodChangeFileDataModel;
            }
            if (endDate == null)
            {
                endDate = _dateTimeHelper.GetDateTimeNow();
            }
            var parameter = $"{paygroupId},@startDate,@endDate"; // Revisit use SQL Parameter instead of string params
            try
            {
                periodChangeFileDataModel.personal = await _appDbContext.PERS.FromSqlRaw("SELECT * FROM dbo.gethybridchangereport_pers(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                           .ToListAsync();                
                periodChangeFileDataModel.jobs = await _appDbContext.JDET.FromSqlRaw("SELECT * FROM dbo.gethybridchangereport_jdet(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                       .ToListAsync();                
                periodChangeFileDataModel.salary = await _appDbContext.SLRY.FromSqlRaw("SELECT * FROM dbo.gethybridchangereport_slry(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();                
                periodChangeFileDataModel.address = await _appDbContext.ADDR.FromSqlRaw("SELECT * FROM dbo.getperiodchangereport_addr(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();                
                periodChangeFileDataModel.bank = await _appDbContext.BANK.FromSqlRaw("SELECT * FROM dbo.gethybridchangereport_bank(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();
                periodChangeFileDataModel.cspf = await _appDbContext.CSPF.FromSqlRaw("SELECT * FROM dbo.gethybridchangereport_cspf(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();                
                periodChangeFileDataModel.conf = await _appDbContext.CONF.FromSqlRaw("SELECT * FROM dbo.gethybridchangereport_conf(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();               
                periodChangeFileDataModel.payD = await _appDbContext.PAYD.FromSqlRaw("SELECT * FROM dbo.gethybridchangereport_payd(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();
               periodChangeFileDataModel.time = await _appDbContext.TIME.FromSqlRaw("SELECT * FROM dbo.gethybridchangereport_time(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                          .ToListAsync();
               periodChangeFileDataModel.starters = await _appDbContext.STARTERS.FromSqlRaw("SELECT * FROM dbo.gethybridchangereport_starters(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                          .ToListAsync();
               periodChangeFileDataModel.leavers = await _appDbContext.Leavers.FromSqlRaw("SELECT * FROM dbo.gethybridchangereport_leavers(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                          .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while generating data for hybrid change file report, {ex}", ex);
                throw ex;
            }

            return periodChangeFileDataModel;
        }
        /// <summary>
        /// Gets the data by calling the respective pgsql functions
        /// </summary>
        /// <param name="_data"></param>
        /// <returns></returns>
        protected async Task<PeriodChangeFileDataModel> GetPeriodChangeFileData(LoggedInUser user, int? paygroupId, DateTime? startDate, DateTime? endDate)
        {

            var periodChangeFileDataModel = new PeriodChangeFileDataModel();
            periodChangeFileDataModel.personal = new List<PeriodChangePERS>();
            periodChangeFileDataModel.jobs = new List<PeriodChangeJDET>();
            periodChangeFileDataModel.salary = new List<PeriodChangeSLRY>();
            periodChangeFileDataModel.address = new List<PeriodChangeADDR>();
            periodChangeFileDataModel.bank = new List<PeriodChangeBANK>();
            periodChangeFileDataModel.cspf = new List<PeriodChangeCSPF>();
            periodChangeFileDataModel.conf = new List<PeriodChangeCONF>();
            periodChangeFileDataModel.payD = new List<PeriodChangePAYD>();
            periodChangeFileDataModel.time = new List<PeriodChangeTIME>();
            periodChangeFileDataModel.starters = new List<PeriodChangeStarters>();
            periodChangeFileDataModel.leavers = new List<PeriodChangeLeavers>();
            if (paygroupId == null || startDate == null)
            {
                _logger.LogInformation("Invalid parameters sent to Period change file report, ignoring the request");
                return periodChangeFileDataModel;
            }
            if (endDate == null)
            {
                endDate = _dateTimeHelper.GetDateTimeNow();
            }
            var parameter = $"{paygroupId},@startDate,@endDate"; // Revisit use SQL Parameter instead of string params
            try
            {
                periodChangeFileDataModel.personal = await _appDbContext.PERS.FromSqlRaw("SELECT * FROM dbo.GetPeriodChangeReport_Pers(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                           .ToListAsync();
                periodChangeFileDataModel.jobs = await _appDbContext.JDET.FromSqlRaw("SELECT * FROM dbo.GetPeriodChangeReport_JDET(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                       .ToListAsync();
                periodChangeFileDataModel.salary = await _appDbContext.SLRY.FromSqlRaw("SELECT * FROM dbo.GetPeriodChangeReport_SLRY(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();
                periodChangeFileDataModel.address = await _appDbContext.ADDR.FromSqlRaw("SELECT * FROM dbo.GetPeriodChangeReport_ADDR(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();
                periodChangeFileDataModel.bank = await _appDbContext.BANK.FromSqlRaw("SELECT * FROM dbo.GetPeriodChangeReport_BANK(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();
                periodChangeFileDataModel.cspf = await _appDbContext.CSPF.FromSqlRaw("SELECT * FROM dbo.GetPeriodChangeReport_CSPF(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();
                periodChangeFileDataModel.conf = await _appDbContext.CONF.FromSqlRaw("SELECT * FROM dbo.GetPeriodChangeReport_CONF(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();
                periodChangeFileDataModel.payD = await _appDbContext.PAYD.FromSqlRaw("SELECT * FROM dbo.GetPeriodChangeReport_PAYD(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();
                    periodChangeFileDataModel.time = await _appDbContext.TIME.FromSqlRaw("SELECT * FROM dbo.GetPeriodChangeReport_TIME(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();
                periodChangeFileDataModel.starters = await _appDbContext.STARTERS.FromSqlRaw("SELECT * FROM dbo.GetPeriodChangeReport_Starters(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();
                periodChangeFileDataModel.leavers = await _appDbContext.Leavers.FromSqlRaw("SELECT * FROM dbo.GetPeriodChangeReport_Leavers(" + parameter + ")", new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate }, new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate })
                                                                           .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while generating data for period change file report, {ex}", ex);
            }

            return periodChangeFileDataModel;
        }

        protected async Task<List<DataTable>> GetCenamPeriodChangeFileData(LoggedInUser user, int? paygroupId, DateTime? startDate, DateTime? endDate, string? countryCode)
        {
            var dataTables = new List<DataTable>();

            if (paygroupId == null || startDate == null || countryCode == null)
            {
                _logger.LogInformation("Invalid parameters sent to Cenam Period change file report, ignoring the request");
                return dataTables;
            }

            endDate ??= _dateTimeHelper.GetDateTimeNow();

            var parameter = $"@paygroupId,@startDate,@endDate";

            try
            {
                await _appDbContext.Database.OpenConnectionAsync();

                dataTables.Add(await ExecuteSqlCommandAsync(GetSqlQueryBasedOnCountryCode(countryCode, parameter), paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getcenamreport_payded({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getcenamreport_wfm({parameter})", paygroupId, startDate, endDate));
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while generating data for Cenam period change file report: {ex}", ex);
            }
            finally
            {
                await _appDbContext.Database.CloseConnectionAsync();
            }

            return dataTables;
        }


        protected async Task<List<DataTable>> GetMexicoPeriodChangeFileData(LoggedInUser user, int? paygroupId, DateTime? startDate, DateTime? endDate, string? countryCode)
        {
            var dataTables = new List<DataTable>();

            if (paygroupId == null || startDate == null || countryCode == null)
            {
                _logger.LogInformation("Invalid parameters sent to Mexico Period change file report, ignoring the request");
                return dataTables;
            }

            endDate ??= _dateTimeHelper.GetDateTimeNow();

            var parameter = $"@paygroupId,@startDate,@endDate";

            try
            {
                await _appDbContext.Database.OpenConnectionAsync();

                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_altas({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_reingresos({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_cambios({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_infonavit({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_cambio_banco({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_cambioturno({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_cambiomail({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_bajas({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_solicitud({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_ausentismos({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_movimientos({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_pers({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_cambio_sueldo({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_movimientos_programados({parameter})", paygroupId, startDate, endDate));
                dataTables.Add(await ExecuteSqlCommandAsync($"SELECT * FROM dbo.getmex_rpa({parameter})", paygroupId, startDate, endDate));
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while generating data for Mexico period change file report: {ex}", ex);
            }
            finally
            {
                await _appDbContext.Database.CloseConnectionAsync();
            }

            return dataTables;
        }
        private async Task<DataTable> ExecuteSqlCommandAsync(string query, int? paygroupId, DateTime? startDate, DateTime? endDate)
        {
            using var command = _appDbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;

            command.Parameters.AddRange(new Npgsql.NpgsqlParameter[]
            {
                new Npgsql.NpgsqlParameter("@paygroupId", NpgsqlTypes.NpgsqlDbType.Integer) { Value = paygroupId },
                new Npgsql.NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate },
                new Npgsql.NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate }
            });

            using var reader = await command.ExecuteReaderAsync();
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }
        private string GetSqlQueryBasedOnCountryCode(string countryCode, string parameter)
        {
            return countryCode switch
            {
                "CRI" => "SELECT * FROM dbo.getcenamreport_cr(" + parameter + ")",
                "DOM" => "SELECT * FROM dbo.getcenamreport_dr(" + parameter + ")",
                "GTM" => "SELECT * FROM dbo.getcenamreport_gt(" + parameter + ")",
                "PAN" => "SELECT * FROM dbo.getcenamreport_pa(" + parameter + ")",
                "SLV" => "SELECT * FROM dbo.getcenamreport_sv(" + parameter + ")",
                "HND" => "SELECT * FROM dbo.getcenamreport_hn_ni(" + parameter + ")",
                "NIC" => "SELECT * FROM dbo.getcenamreport_hn_ni(" + parameter + ")",
                _ => throw new InvalidOperationException("Invalid country code provided.")
            };
        }
        public virtual async Task<List<ReportServiceDetails>> GetReportServiceDetailsAsync(LoggedInUser user, string? paygroup)
        {
            var reportsdatalist = new List<ReportServiceDetails>();
            try
            {
                var query = (from q in _appDbContext.Set<ReportServiceDetails>()
                             where q.PayGroupCode == paygroup && user.UserName.ToLower() == q.CreatedBy.ToLower()
                             orderby q.CreatedAt descending
                             select new ReportServiceDetails()
                             {
                                 Id = q.Id,
                                 PayGroupCode = q.PayGroupCode,
                                 S3ObjectId = q.S3ObjectId,
                                 FilterOption = q.FilterOption,
                                 Year = q.Year,
                                 StartDate = q.StartDate,
                                 EndDate = q.EndDate,
                                 PayPeriod = q.PayPeriod,
                                 CreatedAt = _dateTimeHelper.GetDateTimeWithTimezone(q.CreatedAt).GetValueOrDefault(DateTime.MinValue),
                                 ModifiedAt = q.ModifiedAt,
                                 ModifiedBy = q.ModifiedBy
                             });

                return await query
                               .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }

            return reportsdatalist;
        }
        public virtual async Task<string> DownloadReportServiceDetails(LoggedInUser loggedInUser, int id)
        {
            try
            {
                string s3ObjectKey = await _appDbContext.reportservicedetails.Where(x => x.Id == id).Select(s => s.S3ObjectId).FirstOrDefaultAsync();

                if (s3ObjectKey == null)
                {
                    throw new Exception($"Unable to find Report file for the given id {id}");
                }

                _logger.LogInformation("DownloadReportServiceDetails : S3Bucket Name - {name}", _config.S3ReportOutputBucketName);
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = _config.S3ReportOutputBucketName,
                    Key = s3ObjectKey,
                    Expires = _dateTimeHelper.GetDateTimeNow().AddMinutes(5),
                };

                var url = _s3Client.GetPreSignedURL(request);
                return url;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "{ex}", ex);
                return null;
            }
        }
        public virtual void InsertError(LoggedInUser user, int iRequestID, string strProject, string strCode, string strShortDescription, string strLongDescription)
        {
            try
            {
                int id = 0;
                var errorDetails = new ErrorDetails
                {
                    requestid = iRequestID,
                    project = strProject,
                    code = strCode,
                    shortdescription = strShortDescription,
                    longdescription = strLongDescription
                };
                _appDbContext.errordetails.Add(errorDetails);
                _appDbContext.SaveChanges();
            }
            catch (NpgsqlException ex)
            {
                InsertError(user, iRequestID, "Application Service", "400", "", ex.ToString());
            }
            catch (Exception ex)
            {
                InsertError(user, iRequestID, "Application Service", "400", "", ex.ToString());

            }

        }

        #endregion Download Reports

        #region S3 download 

        public virtual async Task<string> GetPayElementReport(LoggedInUser user, List<PayElementResponseModel> results, PayElementRequestModel _mdl, string fileName)
        {
            try
            {
                string base64String;
                var earnings = results.Where(PE => PE.Format == "Quantity" || PE.Format == "Amount" || PE.Format == "Percentage").ToList();
                var time = results.Where(PE => PE.Format == "Hours" || PE.Format == "Days" || PE.Format == "Minutes").ToList();

                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                DataTable dtearnings = converter.ToDataTable(earnings);
                DataTable dttime = converter.ToDataTable(time);

                dtearnings.Columns.Remove("Clientname");
                dttime.Columns.Remove("Clientname");

                dtearnings = ProcesDTHeaders(dtearnings);
                if (dtearnings.Columns.Contains("Pay Element Code"))
                {
                    dtearnings.Columns["Pay Element Code"].ColumnName = "Local Pay Code";
                }
                if (dtearnings.Columns.Contains("Export Code"))
                {
                    dtearnings.Columns["Export Code"].ColumnName = "Partner Pay Code";
                }
                dtearnings.Columns["G L Credit Code"].ColumnName = "GL Credit Code";
                dtearnings.Columns["G L Debit Code"].ColumnName = "GL Debit Code";
                dtearnings.Columns["Is Employer Tax"].ColumnName = "IsEmployerTax";
                dtearnings.Columns["Is Employee Deduction"].ColumnName = "IsEmployeeDeduction";

                dttime = ProcesDTHeaders(dttime);
                if (dttime.Columns.Contains("Pay Element Code"))
                {
                    dttime.Columns["Pay Element Code"].ColumnName = "Local Pay Code";
                }
                if (dttime.Columns.Contains("Export Code"))
                {
                    dttime.Columns["Export Code"].ColumnName = "Partner Pay Code";
                }
                dttime.Columns["G L Credit Code"].ColumnName = "GL Credit Code";
                dttime.Columns["G L Debit Code"].ColumnName = "GL Debit Code";
                dttime.Columns["Is Employer Tax"].ColumnName = "IsEmployerTax";
                dttime.Columns["Is Employee Deduction"].ColumnName = "IsEmployeeDeduction";

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };
                string clientcode = await _reportServiceHelper.GetClientCodeByPayGroup(_mdl.paygroup);
                _mdl.clientname = clientcode;
                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    //return responseStream;

                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;
                    //return memStream;

                    using (var wb = new XLWorkbook(memStream))
                    {
                        var sheetname1 = "PAYCodes_" + DateTime.Now.ToString("yyyyMMdd");
                        var sheetname2 = "TIMECodes_" + DateTime.Now.ToString("yyyyMMdd");
                        var sheet = wb.Worksheet(1);
                        var sheet2 = wb.Worksheet(2);
                        var sheet3 = wb.Worksheet(3);
                        sheet.Name = "Summary";
                        sheet2.Name = sheetname1;
                        sheet3.Name = sheetname2;
                        string EmptySheet = "There is no data";

                        wb.Worksheet(1).Cell(9, 6).Value = _mdl != null ? _mdl.clientname : "";
                        wb.Worksheet(1).Cell(10, 6).Value = _mdl != null ? _mdl.paygroup : "";

                        if (dtearnings.Rows.Count > 0)
                        {
                            var table = wb.Worksheet(2).Cell(1, 1).InsertTable(dtearnings);
                            wb.Worksheet(2).Tables.FirstOrDefault().ShowAutoFilter = false;
                            table.HeadersRow().Style.Fill.BackgroundColor = XLColor.MediumElectricBlue;
                        }
                        else { var table = wb.Worksheet(2).Cell(4, 7).Value = EmptySheet.ToUpper(); }

                        if (dttime.Rows.Count > 0)
                        {
                            var table2 = wb.Worksheet(3).Cell(1, 1).InsertTable(dttime);
                            wb.Worksheet(3).Tables.FirstOrDefault().ShowAutoFilter = false;
                            table2.HeadersRow().Style.Fill.BackgroundColor = XLColor.MediumElectricBlue;
                        }
                        else { var table = wb.Worksheet(3).Cell(4, 7).Value = EmptySheet.ToUpper(); }

                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            base64String = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public virtual async Task<string> GetCSPFDatawarehouse(LoggedInUser user, CSAndCFDatawarehouseResponseModel results, CSPFDatawarehouseRequestModel _mdl, string fileName)
        {
            try
            {
                string base64String;

                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                DataTable dtcspf = converter.ToDataTable(results.cspfDatawarehouseResponse);
                DataTable dtcf = converter.ToDataTable(results.confidentialDatawarehouseResponse);
                dtcspf.Columns.Remove("startdate");
                dtcspf.Columns.Remove("clientname");
                dtcspf.Columns.Remove("includeterminated");
                dtcf = ProcesDTHeaders(dtcf);
                dtcspf = ProcesDTHeaders(dtcspf);
                dtcf.Columns["Employee I D"].ColumnName = "Employee ID";
                dtcspf.Columns["Employee I D"].ColumnName = "Employee ID";
                dtcspf.Columns["Cs F Id Name"].ColumnName = "Cs FldName";

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    //return responseStream;

                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;
                    //return memStream;

                    using (var wb = new XLWorkbook(memStream))
                    {
                        var Tabname1 = _reportServiceHelper.GenerateSheetName(_mdl.paygroup, "CSDW");
                        var Tabname2 = _reportServiceHelper.GenerateSheetName(_mdl.paygroup, "CFDW");
                        var sheet = wb.Worksheet(1);
                        var sheet2 = wb.Worksheet(2);
                        var sheet3 = wb.Worksheet(3);
                        sheet.Name = "Summary";
                        sheet2.Name = Tabname1;
                        sheet3.Name = Tabname2;
                        string EmptySheet = "There is no data";
                        //wb.Worksheet(1).Cell(2, 2).Value = _mdl != null ? _mdl.clientname : "";
                        wb.Worksheet(1).Cell(10, 6).Value = _mdl != null ? _mdl.paygroup : "";
                        wb.Worksheet(1).Cell(11, 6).Value = _mdl != null ? _mdl.startdate : "";
                        wb.Worksheet(1).Cell(12, 6).Value = _mdl != null ? _mdl.enddate : "";
                        wb.Worksheet(1).Cell(13, 6).Value = _mdl != null ? _mdl.includeterminated : "";

                        if (dtcspf.Rows.Count > 0)
                        {
                            var table = wb.Worksheet(2).Cell(1, 1).InsertTable(dtcspf);
                            wb.Worksheet(2).Tables.FirstOrDefault().ShowAutoFilter = false;
                            table.HeadersRow().Style.Fill.BackgroundColor = XLColor.FromHtml("#17479D");
                            table.HeadersRow().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            foreach (var row in table.DataRange.Rows())
                            {
                                row.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            }
                        }
                        else { var table = wb.Worksheet(2).Cell(4, 7).Value = EmptySheet.ToUpper(); }

                        if (dtcf.Rows.Count > 0)
                        {
                            var table = wb.Worksheet(3).Cell(1, 1).InsertTable(dtcf);
                            wb.Worksheet(3).Tables.FirstOrDefault().ShowAutoFilter = false;
                            table.HeadersRow().Style.Fill.BackgroundColor = XLColor.FromHtml("#17479D");
                            table.HeadersRow().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            foreach (var row in table.DataRange.Rows())
                            {
                                row.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            }
                        }
                        else { var table = wb.Worksheet(3).Cell(4, 7).Value = EmptySheet.ToUpper(); }
                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            base64String = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public virtual async Task<string> GetPAYDDatawarehouse(LoggedInUser user, List<PaydDatawarehouseResponseModel> results, PaydDatawarehouseRequestModel _mdl, string fileName)
        {
            try
            {
                string base64String;

                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                DataTable dt = converter.ToDataTable(results);
                dt.Columns.Remove("paygroup");
                dt.Columns.Remove("payperiod");
                dt.Columns.Remove("clientname");
                dt.Columns.Remove("includeterminated");

                dt = ProcesDTHeaders(dt);
                dt.Columns["Employee I D"].ColumnName = "Employee ID";
                if (dt.Columns.Contains("Pay Element Code"))
                {
                    dt.Columns["Pay Element Code"].ColumnName = "Local Pay Code";
                }
                if (dt.Columns.Contains("Export Code"))
                {
                    dt.Columns["Export Code"].ColumnName = "Partner Pay Code";
                }


                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    //return responseStream;

                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;
                    //return memStream;

                    using (var wb = new XLWorkbook(memStream))
                    {
                        var Tabname = _reportServiceHelper.GenerateSheetName(_mdl.paygroup, "PDW");
                        var sheet = wb.Worksheet(1);
                        var sheet2 = wb.Worksheet(2);
                        sheet.Name = "Summary";
                        sheet2.Name = Tabname;
                        string EmptySheet = "There is no data";
                        wb.Worksheet(1).Cell(9, 6).Value = _mdl != null ? _mdl.paygroup : "";
                        wb.Worksheet(1).Cell(10, 6).Value = _mdl != null ? _mdl.payperiod : "";
                        wb.Worksheet(1).Cell(11, 6).Value = _mdl != null ? _mdl.includeterminated : "";

                        if (dt.Rows.Count > 0)
                        {
                            var table = wb.Worksheet(2).Cell(1, 1).InsertTable(dt);
                            wb.Worksheet(2).Tables.FirstOrDefault().ShowAutoFilter = false;
                            table.HeadersRow().Style.Fill.BackgroundColor = XLColor.MediumElectricBlue;
                        }
                        else { var table = wb.Worksheet(2).Cell(4, 7).Value = EmptySheet.ToUpper(); }

                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            base64String = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public virtual async Task<string> GetSystemUserReport(LoggedInUser user, List<SystemUserResponseModel> results, SystemUserRequestModel _mdl, string fileName)
        {
            try
            {
                string base64String;

                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                DataTable dt = converter.ToDataTable(results);

                dt = ProcesDTHeaders(dt);
                dt.Columns["Clientname"].ColumnName = "Client Name";
                dt.Columns["Paygroup"].ColumnName = "Pay Group";
                dt.Columns["Userid"].ColumnName = "User ID";

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };
                string clientcode = await _reportServiceHelper.GetClientCodeByPayGroup(_mdl.paygroup);
                _mdl.clientname = clientcode;
                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    //return responseStream;

                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;
                    //return memStream;

                    using (var wb = new XLWorkbook(memStream))
                    {
                        var Tabname = _reportServiceHelper.GenerateSheetName(_mdl.paygroup, "SysUsr");
                        var sheet = wb.Worksheet(1);
                        var sheet2 = wb.Worksheet(2);
                        sheet.Name = "Summary";
                        sheet2.Name = Tabname;
                        wb.Worksheet(1).Cell(9, 6).Value = _mdl != null ? _mdl.clientname : "";
                        wb.Worksheet(1).Cell(10, 6).Value = _mdl != null ? _mdl.paygroup : "";
                        wb.Worksheet(2).Cell(1, 1).InsertTable(dt);

                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            base64String = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
                return "Error in generating Sytem User report";
            }
        }

        public virtual async Task<string> GetTransactionByPayGroupReport(LoggedInUser user, List<TransactionResponseModel> results, TransactionRequestModel _mdl, string fileName)
        {
            try
            {
                string base64String;
                var numberOfPPs = CalculatePayPeriods(_mdl.startpp, _mdl.endpp);

                DataTable dt = new DataTable();
                dt.Columns.Add("Pay Group");
                dt.Columns.Add("Payroll Year");
                dt.Columns.Add("Pay Period");
                dt.Columns.Add("Pay Date");
                dt.Columns.Add("Month");
                dt.Columns.Add("Employee");
                dt.Columns.Add("Employee ID");
                dt.Columns.Add("Pay Element Name Local");
                dt.Columns.Add("Pay Element Name");
                dt.Columns.Add("Local Pay Code");
                dt.Columns.Add("Partner Pay Code");
                dt.Columns.Add("Element Type");
                dt.Columns.Add("GL Credit Code");
                dt.Columns.Add("GL Debit Code");
                dt.Columns.Add("From");
                dt.Columns.Add("To");
                dt.Columns.Add("Amount");
                dt.Columns.Add("Currency");
                dt.Columns.Add("Cost Center");
                dt.Columns.Add("Department");
                dt.Columns.Add("Org Unit 1");
                dt.Columns.Add("Org Unit 2");
                dt.Columns.Add("Org Unit 3");
                dt.Columns.Add("Org Unit 4");

                foreach (var item in results)
                {
                    dt.Rows.Add(
                        item.PayGroup,
                        item.PayrollYear,
                        item.PayPeriod,
                        GetFormattedDateString(item.PayDate),
                        item.Months,
                        item.EmployeeID + " " + item.LastName + " " + item.FirstName,
                        item.EmployeeID,
                        item.PayElementNameLocal,
                        item.PayElementName,
                        item.PayElementCode,
                        item.ExportCode,
                        item.ElementType,
                        item.GLCreditCode,
                        item.GLDebitCode,
                        item.From,
                        item.To,
                        item.Amount,
                        item.TransactionCurrency,
                        item.CostCenter,
                        item.Department,
                        item.Orglevel1,
                        item.Orglevel2,
                        item.Orglevel3,
                        item.Orglevel4
                    );
                }

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };
                string clientcode = await _reportServiceHelper.GetClientCodeByPayGroup(_mdl.paygroup);
                _mdl.clientname = clientcode;

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;

                    using (var wb = new XLWorkbook(memStream))
                    {
                        var templateSheet = wb.Worksheet(1);
                        int startpp = int.TryParse(_mdl.startpp, out var result) ? result : 0;
                        int endpp = int.TryParse(_mdl.endpp, out var res) ? res : 0;
                        if (numberOfPPs <= 8)
                        {
                            for (int i = startpp; i <= endpp; i++)
                            {
                                var payPeriodSheet = templateSheet.CopyTo($"Transac {(i).ToString("D2")}");

                                var filteredData = FilterDataTableByPayPeriod(dt, i);
                                var filteredResults = results.Where(r => r.PayPeriod == i.ToString()).FirstOrDefault();
                                PopulateTransactionByPaygroupData(payPeriodSheet, filteredData, _mdl, i, filteredResults);
                            }
                            wb.Worksheet(1).Delete();
                        }
                        else
                        {
                            var sheet = wb.Worksheet(1);
                            sheet.Name = "Transac " + _mdl.startpp + "-" + _mdl.endpp;
                            var payPeriodStart = (from pc in _appDbContext.paycalendar
                                                  join pg in _appDbContext.paygroup on pc.paygroupid equals pg.id
                                                  where pc.taskid == "SD" && _mdl.paygroup == pg.code && pc.payperiod.ToString() == _mdl.startpp
                                                  select pc.date).FirstOrDefault();

                            var payPeriodEnd = (from pc in _appDbContext.paycalendar
                                                join pg in _appDbContext.paygroup on pc.paygroupid equals pg.id
                                                where pc.taskid == "ED" && _mdl.paygroup == pg.code && pc.payperiod.ToString() == _mdl.endpp
                                                select pc.date).FirstOrDefault();
                            PopulateTransactionByPaygroupData(sheet, dt, _mdl, null,
                                new TransactionResponseModel { PPStartDate = payPeriodStart.Value.ToString("dd-MM-yyyy") },
                                new TransactionResponseModel { PPEndDate = payPeriodEnd.HasValue ? payPeriodEnd.Value.ToString("dd-MM-yyyy") : "" });
                        }

                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            ms.Position = 0; // Reset the stream position to the beginning
                            var status = await UploadtoS3andUpdatereportservicedetails(ms, fileName, JsonConvert.SerializeObject(_mdl), user, _mdl.paygroup);

                            base64String = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return "Error in generating Transaction By PayGroup report";
            }
        }

        public void PopulateTransactionByPaygroupData(IXLWorksheet sheet, DataTable dt, TransactionRequestModel _mdl, int? i, TransactionResponseModel startResult, TransactionResponseModel endResult = null)
        {
            string EmptySheet = "There is no data";
            sheet.ShowGridLines = false;
            sheet.Cell(9, 6).Value = _mdl != null ? _mdl.clientname : "";
            sheet.Cell(10, 6).Value = _mdl != null ? _mdl.paygroup : "";

            if (endResult == null || startResult == endResult)
            {

                sheet.Cell(11, 6).Value = i.ToString();
                sheet.Cell(12, 6).Value = dt.Rows.Count > 0 ? DateTime.Parse(startResult.PPStartDate).ToString("dd-MM-yyyy") : null;
                sheet.Cell(13, 6).Value = dt.Rows.Count > 0 ? DateTime.Parse(startResult.PPEndDate).ToString("dd-MM-yyyy") : null;
            }
            else
            {

                sheet.Cell(11, 6).Value = _mdl.startpp + " - " + _mdl.endpp;
                sheet.Cell(12, 6).Value = (dt.Rows.Count > 0 && startResult != null && !string.IsNullOrEmpty(startResult.PPStartDate)) ? DateTime.ParseExact(startResult.PPStartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy") : null;
                sheet.Cell(13, 6).Value = (dt.Rows.Count > 0 && endResult != null && !string.IsNullOrEmpty(endResult.PPEndDate)) ? DateTime.ParseExact(endResult.PPEndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy") : null;
            }

            if (dt.Rows.Count > 0)
            {
                var table = sheet.Cell(16, 1).InsertTable(dt);
                sheet.Tables.FirstOrDefault().ShowAutoFilter = false;
                var range = sheet.RangeUsed();
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                table.HeadersRow().Style.Fill.BackgroundColor = XLColor.MediumElectricBlue;
            }
            else
            {
                sheet.Cell(18, 5).Value = EmptySheet.ToUpper();
            }
        }

        public DataTable FilterDataTableByPayPeriod(DataTable dt, int payPeriod)
        {
            DataTable filteredDataTable = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                if (int.Parse(row["Pay Period"].ToString()) == payPeriod)
                {
                    filteredDataTable.ImportRow(row);
                }
            }
            return filteredDataTable;
        }

        private int CalculatePayPeriods(string startPP, string endPP)
        {
            int start = int.Parse(startPP);
            int end = int.Parse(endPP);
            return end - start + 1;
        }

        public virtual async Task<string> GetTransactionByCountryReport(LoggedInUser user, List<TransactionCountryResponseModel> results, TransactionCountryRequestModel _mdl, string fileName)
        {
            try
            {
                if (results.Count == 0) { return null; }
                string base64String;

                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                DataTable dt = converter.ToDataTable(results);
                dt.Columns.Remove("Country");
                dt.Columns.Remove("Countryid");
                dt.Columns.Remove("ClientName");
                dt = ProcesDTHeaders(dt);
                if (dt.Columns.Contains("Pay Element Code"))
                {
                    dt.Columns["Pay Element Code"].ColumnName = "Local Pay Code";
                }
                if (dt.Columns.Contains("Export Code"))
                {
                    dt.Columns["Export Code"].ColumnName = "Partner Pay Code";
                }

                dt.Columns["P P Start Date"].ColumnName = "PP Start Date";
                dt.Columns["P P End Date"].ColumnName = "PP End Date";
                dt.Columns["G L Credit Code"].ColumnName = "GL Credit Code";
                dt.Columns["G L Debit Code"].ColumnName = "GL Debit Code";
                dt.Columns["Employee I D"].ColumnName = "Employee ID";

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    //return responseStream;

                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;
                    //return memStream;

                    using (var wb = new XLWorkbook(memStream))
                    {
                        var sheet = wb.Worksheet(1);
                        var sheet2 = wb.Worksheet(2);
                        sheet.Name = "Summary";
                        sheet2.Name = "TransactionByCountryR" + _mdl.startpp + "-" + _mdl.endpp;
                        string EmptySheet = "There is no data";

                        wb.Worksheet(1).Cell(9, 6).Value = _mdl != null ? _mdl.clientname : "";
                        wb.Worksheet(1).Cell(10, 6).Value = _mdl != null ? _mdl.country : "";
                        wb.Worksheet(1).Cell(11, 6).Value = _mdl != null ? _mdl.startpp : "";
                        wb.Worksheet(1).Cell(12, 6).Value = _mdl != null ? _mdl.endpp : "";

                        if (dt.Rows.Count > 0)
                        {
                            var table = wb.Worksheet(2).Cell(1, 1).InsertTable(dt);
                            wb.Worksheet(2).Tables.FirstOrDefault().ShowAutoFilter = false;
                            table.HeadersRow().Style.Fill.BackgroundColor = XLColor.MediumElectricBlue;
                        }
                        else { var table = wb.Worksheet(2).Cell(4, 7).Value = EmptySheet.ToUpper(); }

                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            base64String = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return null;
            }
        }

        public virtual async Task<string> GetHrDatawarehouse(LoggedInUser user, List<HrDatawarehouseResponseModel> results, HrDatawarehouseRequestModel _mdl, string fileName)
        {
            try
            {
                string base64String;

                var converter = new ListtoDataTableConverter();
                DataTable dt = converter.ToDataTable(results);
                dt.Columns.Remove("ClientName");
                dt.Columns.Remove("IncludeTerminated");
                dt.Columns.Remove("payperiod");
                dt.Columns.Remove("year");
                dt = ProcesDTHeaders(dt);

                dt.Columns["Employee I D"].ColumnName = "Employee ID";
                dt.Columns["Dateof Birth"].ColumnName = "Date of Birth";
                dt.Columns["Typeof Salary"].ColumnName = "Type of Salary";
                dt.Columns["Noof Installments"].ColumnName = "No of Installments";
                dt.Columns["Org Unit1"].ColumnName = "Org Unit 1";
                dt.Columns["Org Unit2"].ColumnName = "Org Unit 2";
                dt.Columns["Org Unit3"].ColumnName = "Org Unit 3";
                dt.Columns["Org Unit4"].ColumnName = "Org Unit 4";
                dt.Columns["Street Address1"].ColumnName = "Street Address 1";
                dt.Columns["Street Address2"].ColumnName = "Street Address 2";
                dt.Columns["Street Address3"].ColumnName = "Street Address 3";
                dt.Columns["Street Address4"].ColumnName = "Street Address 4";
                dt.Columns["Street Address5"].ColumnName = "Street Address 5";
                dt.Columns["Street Address6"].ColumnName = "Street Address 6";
                dt.Columns["Iban Code"].ColumnName = "IBAN Code";
                dt.Columns["country"].ColumnName = "Country";
                dt.Columns["Salary Effective Date"].ColumnName = "Effective Date";
                dt.Columns["Work Contract Effective Date"].ColumnName = " Effective Date ";
                dt.Columns["Work Contract Enddate"].ColumnName = " End Date ";
                dt.Columns["Salary End Date"].ColumnName = "End Date";


                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;

                    using (var wb = new XLWorkbook(memStream))
                    {
                        var Tabname = _reportServiceHelper.GenerateSheetName(_mdl.paygroup, "DW");
                        int nextrownumber = 0;
                        var sheet = wb.Worksheet(1);
                        var sheet2 = wb.Worksheet(2);
                        sheet.Name = "Summary";
                        sheet2.Name = Tabname;
                        string EmptySheet = "There is no data";
                        wb.Worksheet(1).Cell(9, 6).Value = _mdl != null ? _mdl.paygroup : "";
                        wb.Worksheet(1).Cell(10, 6).Value = _mdl != null ? _mdl.payperiod : "";
                        wb.Worksheet(1).Cell(11, 6).Value = _mdl != null ? _mdl.includeterminated : "";

                        if (dt.Rows.Count > 0)
                        {
                            var table = wb.Worksheet(2).Cell(1, 1).InsertTable(dt);
                            wb.Worksheet(2).Tables.FirstOrDefault().ShowAutoFilter = false;
                            table.HeadersRow().Style.Fill.BackgroundColor = XLColor.MediumElectricBlue;
                        }
                        else
                        {
                            var table = wb.Worksheet(2).Cell(4, 7).Value = EmptySheet.ToUpper();
                        }

                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            base64String = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return null;
            }
        }

        public virtual async Task<string> GetPayPeriodRegisters(LoggedInUser user, List<PayPeriodRegisterResponseModel> results, List<PayPeriodRegisterDetailResponseModel> detailResults, PayPeriodRegisterRequestModel _mdl, List<PayrollElementsModel> peData, string fileName)
        {
            try
            {
                string base64String;

                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                DataTable summarydata = converter.ToDataTable(results);
                int iEMPRtoEMP = 0;
                const string ErToEeKeyPrefix = "eree_";
                int iEMPtoEMPR = 0;
                const string EeToErKeyPrefix = "eeer_";
                int iEMPRtoOT = 0;
                const string ErToOtKeyPrefix = "erot_";
                int iEMPtoOT = 0;
                const string EeToOtKeyPrefix = "eeot_";

                DataTable dtDetailData = new DataTable("DetailResult");
                DataColumn dt = new DataColumn(); dt.DataType = typeof(string); dt.ColumnName = "PayGroup"; dt.Caption = "Pay Group"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(string); dt.ColumnName = "EmployeeID"; dt.Caption = "Employee ID"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(string); dt.ColumnName = "EmployeeNumber"; dt.Caption = "Employee Number"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(int); dt.ColumnName = "PayPeriod"; dt.Caption = "Pay Period"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(string); dt.ColumnName = "Offcycle"; dt.Caption = "Offcycle"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(string); dt.ColumnName = "HireDate"; dt.Caption = "Hire Date"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(string); dt.ColumnName = "TerminationDate"; dt.Caption = "Termination Date"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(string); dt.ColumnName = "LastName"; dt.Caption = "Last Name"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(string); dt.ColumnName = "SecondLastName"; dt.Caption = "Second Last Name"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(string); dt.ColumnName = "FirstName"; dt.Caption = "First Name"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(string); dt.ColumnName = "MiddleNames"; dt.Caption = "Middle Names"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);


                #region 
                var columnDictionary = new Dictionary<string, int>();

                var columnLoop = (from PE in peData
                                  where PE.froms == "Employer" && PE.tos == "Employee"
                                  select new { PE.code, PE.name, }).Distinct().OrderBy(e => e.code).ToList();
                iEMPRtoEMP = columnLoop.Count();
                for (int i = 0; i < columnLoop.Count(); i++)
                {
                    var columnName = columnLoop[i].code + "-" + columnLoop[i].name;
                    dt = new DataColumn
                    {
                        DataType = typeof(decimal),
                        ColumnName = columnName,
                        Caption = columnName,
                        AutoIncrement = false,
                        ReadOnly = false,
                        Unique = false
                    };

                    _ = columnDictionary.TryAdd($"{ErToEeKeyPrefix}{columnName}", 0);
                    dtDetailData.Columns.Add(dt);
                }

                columnLoop = (from PE in peData
                              where PE.froms == "Employee" && PE.tos == "Employer"
                              select new { PE.code, PE.name, }).Distinct().OrderBy(e => e.code).ToList();
                iEMPtoEMPR = columnLoop.Count();
                for (int i = 0; i < columnLoop.Count(); i++)
                {
                    var columnName = columnLoop[i].code + "-" + columnLoop[i].name;
                    dt = new DataColumn
                    {
                        DataType = typeof(decimal),
                        ColumnName = columnName,
                        Caption = columnName,
                        AutoIncrement = false,
                        ReadOnly = false,
                        Unique = false
                    };

                    _ = columnDictionary.TryAdd($"{EeToErKeyPrefix}{columnName}", 0);
                    dtDetailData.Columns.Add(dt);
                }


                columnLoop = (from PE in peData
                              where PE.froms == "Employer" && PE.tos == "Other"
                              select new { PE.code, PE.name, }).Distinct().OrderBy(e => e.code).ToList();
                iEMPRtoOT = columnLoop.Count();
                for (int i = 0; i < columnLoop.Count(); i++)
                {
                    var columnName = columnLoop[i].code + "-" + columnLoop[i].name;
                    dt = new DataColumn
                    {
                        DataType = typeof(decimal),
                        ColumnName = columnName,
                        Caption = columnName,
                        AutoIncrement = false,
                        ReadOnly = false,
                        Unique = false
                    };

                    _ = columnDictionary.TryAdd($"{ErToOtKeyPrefix}{columnName}", 0);
                    dtDetailData.Columns.Add(dt);
                }

                columnLoop = (from PE in peData
                              where PE.froms == "Employee" && PE.tos == "Other"
                              select new { PE.code, PE.name, }).Distinct().OrderBy(e => e.code).ToList();
                iEMPtoOT = columnLoop.Count();
                for (int i = 0; i < columnLoop.Count(); i++)
                {
                    var columnName = columnLoop[i].code + "-" + columnLoop[i].name;
                    dt = new DataColumn
                    {
                        DataType = typeof(decimal),
                        ColumnName = columnName,
                        Caption = columnName,
                        AutoIncrement = false,
                        ReadOnly = false,
                        Unique = false
                    };

                    _ = columnDictionary.TryAdd($"{EeToOtKeyPrefix}{columnName}", 0);
                    dtDetailData.Columns.Add(dt);
                }

                #endregion

                dt = new DataColumn(); dt.DataType = typeof(decimal); dt.ColumnName = "NetPay"; dt.Caption = "Net Pay"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(decimal); dt.ColumnName = "TotalDisbursement"; dt.Caption = "Total Disbursement"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(decimal); dt.ColumnName = "EmployerCost"; dt.Caption = "Employer Cost"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(decimal); dt.ColumnName = "TotalDeductions"; dt.Caption = "Total Deductions"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);
                dt = new DataColumn(); dt.DataType = typeof(decimal); dt.ColumnName = "TotalGrossPay"; dt.Caption = "Total Gross Pay"; dt.AutoIncrement = false; dt.ReadOnly = false; dt.Unique = false; dtDetailData.Columns.Add(dt);

                DataRow dr;
                for (int j = 0; j < detailResults.Count(); j++)
                {

                    dr = dtDetailData.NewRow();
                    dr["PayGroup"] = detailResults[j].PayGroup;
                    dr["EmployeeID"] = detailResults[j].EmployeeID;
                    dr["EmployeeNumber"] = detailResults[j].EmployeeNumber;
                    dr["PayPeriod"] = detailResults[j].PayPeriod;
                    dr["Offcycle"] = detailResults[j].Offcycle;
                    dr["HireDate"] = detailResults[j].HireDate;
                    dr["TerminationDate"] = detailResults[j].TerminationDate;
                    dr["LastName"] = detailResults[j].LastName;
                    dr["SecondLastName"] = detailResults[j].SecondLastName;
                    dr["FirstName"] = detailResults[j].FirstName;
                    dr["MiddleNames"] = detailResults[j].MiddleNames;

                    for (int i = 0; i < detailResults[j].EmployerToEmployee.Count(); i++)
                    {
                        var columnName = detailResults[j].EmployerToEmployee[i].code + "-" + detailResults[j].EmployerToEmployee[i].name;
                        var value = detailResults[j].EmployerToEmployee[i].value;

                        var dictKey = $"{ErToEeKeyPrefix}{columnName}";
                        if (value != null && value != 0 && columnDictionary.ContainsKey(dictKey))
                        {
                            columnDictionary[dictKey] = columnDictionary[dictKey] + 1;
                        }
                        dr[columnName] = value;
                    }

                    for (int i = 0; i < detailResults[j].EmployeeToEmployer.Count(); i++)
                    {
                        var columnName = detailResults[j].EmployeeToEmployer[i].code + "-" + detailResults[j].EmployeeToEmployer[i].name;
                        var value = detailResults[j].EmployeeToEmployer[i].value;

                        var dictKey = $"{EeToErKeyPrefix}{columnName}";
                        if (value != null && value != 0 && columnDictionary.ContainsKey(dictKey))
                        {
                            columnDictionary[dictKey] = columnDictionary[dictKey] + 1;
                        }
                        dr[columnName] = value;
                    }

                    for (int i = 0; i < detailResults[j].EmployerToOther.Count(); i++)
                    {
                        var columnName = detailResults[j].EmployerToOther[i].code + "-" + detailResults[j].EmployerToOther[i].name;
                        var value = detailResults[j].EmployerToOther[i].value;
                        var dictKey = $"{ErToOtKeyPrefix}{columnName}";
                        if (value != null && value != 0 && columnDictionary.ContainsKey(dictKey))
                        {
                            columnDictionary[dictKey] = columnDictionary[dictKey] + 1;
                        }
                        dr[columnName] = value;
                    }

                    for (int i = 0; i < detailResults[j].EmployeeToOther.Count(); i++)
                    {
                        var columnName = detailResults[j].EmployeeToOther[i].code + "-" + detailResults[j].EmployeeToOther[i].name;
                        var value = detailResults[j].EmployeeToOther[i].value;
                        var dictKey = $"{EeToOtKeyPrefix}{columnName}";
                        if (value != null && value != 0 && columnDictionary.ContainsKey(dictKey))
                        {
                            columnDictionary[dictKey] = columnDictionary[dictKey] + 1;
                        }
                        dr[columnName] = value;
                    }

                    dr["NetPay"] = detailResults[j].NetPay;
                    dr["TotalDisbursement"] = detailResults[j].TotalDisbursement;
                    dr["EmployerCost"] = detailResults[j].EmployerCost;
                    dr["TotalDeductions"] = detailResults[j].TotalDeductions;
                    dr["TotalGrossPay"] = detailResults[j].TotalGrossPay;

                    dtDetailData.Rows.Add(dr);
                }

                foreach (KeyValuePair<string, int> entry in columnDictionary)
                {
                    if (entry.Value <= 0)
                    {
                        try
                        {
                            string dtColumnName;

                            if (entry.Key.StartsWith(ErToEeKeyPrefix))
                            {
                                dtColumnName = entry.Key.Replace(ErToEeKeyPrefix, "");
                                iEMPRtoEMP--;
                            }
                            else if (entry.Key.StartsWith(EeToErKeyPrefix))
                            {
                                dtColumnName = entry.Key.Replace(EeToErKeyPrefix, "");
                                iEMPtoEMPR--;
                            }
                            else if (entry.Key.StartsWith(ErToOtKeyPrefix))
                            {
                                dtColumnName = entry.Key.Replace(ErToOtKeyPrefix, "");
                                iEMPRtoOT--;
                            }
                            else
                            {
                                dtColumnName = entry.Key.Replace(EeToOtKeyPrefix, "");
                                iEMPtoOT--;
                            }

                            _logger.LogInformation("Removed the column {columnName} from the datatable because value is zero or null", dtColumnName);
                            dtDetailData.Columns.Remove(dtColumnName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Exception occurred while removing the column from datatable in period regiter report genration, columnName {columnName}, {ex}", entry.Key, ex);
                        }
                    }
                }

                summarydata = ProcesDTHeaders(summarydata);
                summarydata.Columns["Employee I D"].ColumnName = "Employee ID";
                summarydata.Columns["E R E E Earnings"].ColumnName = "ER-EE Earnings";
                summarydata.Columns["E E E R Deductions"].ColumnName = "EE-ER Deductions";
                summarydata.Columns["E E O T E E Contributions"].ColumnName = "EE-OT EE Contributions";
                summarydata.Columns["E R O T E R Contributions"].ColumnName = "ER-OT ER Contributions";

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    //return responseStream;

                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;
                    //return memStream;



                    using (var wb = new XLWorkbook(memStream))
                    {

                        var summarysheet = wb.Worksheet(1);
                        var detailsheet = wb.Worksheet(2);
                        var sumdatasheet = wb.Worksheet(3);
                        summarysheet.Name = "Summary";
                        detailsheet.Name = "PRegDet" + "" + _mdl.startpp + "-" + _mdl.endpp;
                        sumdatasheet.Name = "PRegSum" + "" + _mdl.startpp + " - " + _mdl.endpp;
                        string EmptySheet = "There is no data";
                        wb.Worksheet(1).Cell(9, 6).Value = _mdl != null ? _mdl.paygroup : "";
                        wb.Worksheet(1).Cell(10, 6).Value = _mdl != null ? _mdl.startpp : "";
                        wb.Worksheet(1).Cell(11, 6).Value = _mdl != null ? _mdl.endpp : "";


                        if (dtDetailData.Rows.Count > 0)
                        {
                            var dtdetail = wb.Worksheet(2).Cell(7, 1).InsertTable(dtDetailData);
                            wb.Worksheet(2).Tables.FirstOrDefault().ShowAutoFilter = false;

                            dtdetail.HeadersRow().Cells(1, 11).Style.Fill.BackgroundColor = XLColor.MediumElectricBlue;
                            dtdetail.HeadersRow().Cells(12, iEMPRtoEMP + 12).Style.Fill.BackgroundColor = XLColor.MayaBlue;
                            dtdetail.HeadersRow().Cells(12 + iEMPRtoEMP, iEMPtoEMPR + iEMPRtoEMP + 12).Style.Fill.BackgroundColor = XLColor.DodgerBlue;
                            dtdetail.HeadersRow().Cells(12 + iEMPRtoEMP + iEMPtoEMPR, iEMPRtoOT + iEMPtoEMPR + iEMPRtoEMP + 12).Style.Fill.BackgroundColor = XLColor.LightSlateGray;
                            dtdetail.HeadersRow().Cells(12 + iEMPRtoEMP + iEMPtoEMPR + iEMPRtoOT, iEMPtoOT + iEMPRtoOT + iEMPtoEMPR + iEMPRtoEMP + 12).Style.Fill.BackgroundColor = XLColor.Teal;
                            dtdetail.HeadersRow().Cells(12 + iEMPRtoEMP + iEMPtoEMPR + iEMPRtoOT + iEMPtoOT, 5 + iEMPtoOT + iEMPRtoOT + iEMPtoEMPR + iEMPRtoEMP + 11).Style.Fill.BackgroundColor = XLColor.DarkBrown;
                        }
                        else { var table = wb.Worksheet(2).Cell(4, 7).Value = EmptySheet.ToUpper(); }

                        if (summarydata.Rows.Count > 0)
                        {
                            var dtsumdata = wb.Worksheet(3).Cell(1, 1).InsertTable(summarydata);
                            wb.Worksheet(3).Tables.FirstOrDefault().ShowAutoFilter = false;
                            dtsumdata.HeadersRow().Style.Fill.BackgroundColor = XLColor.MediumElectricBlue;
                        }
                        else { var table = wb.Worksheet(2).Cell(4, 7).Value = EmptySheet.ToUpper(); }

                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            base64String = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public virtual async Task<string> GetVarianceReport(LoggedInUser user, List<VarianceResponseModel> results, VarianceRequestModel _mdl, string fileName)
        {
            try
            {
                string base64String;

                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                DataTable dt = converter.ToDataTable(results);
                dt.Columns.Remove("PayPeriod");
                dt = ProcesDTHeaders(dt);
                dt.Columns["Empolyee I D"].ColumnName = "Empolyee ID";
                if (dt.Columns.Contains("Pay Element Code"))
                {
                    dt.Columns["Pay Element Code"].ColumnName = "Local Pay Code";
                }
                if (dt.Columns.Contains("Export Code"))
                {
                    dt.Columns["Export Code"].ColumnName = "Partner Pay Code";
                }


                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };
                string clientcode = await _reportServiceHelper.GetClientCodeByPayGroup(_mdl.paygroup);
                _mdl.clientname = clientcode;
                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    //return responseStream;

                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;
                    //return memStream;

                    using (var wb = new XLWorkbook(memStream))
                    {

                        var sheet = wb.Worksheet(1);
                        var sheet2 = wb.Worksheet(2);
                        sheet.Name = "Summary";
                        sheet2.Name = "ULTE_CO_SM_VarDetEE";
                        string EmptySheet = "There is no data";
                        wb.Worksheet(1).Cell(9, 6).Value = _mdl != null ? _mdl.clientname : "";
                        wb.Worksheet(1).Cell(10, 6).Value = _mdl != null ? _mdl.paygroup : "";
                        wb.Worksheet(1).Cell(11, 6).Value = _mdl != null ? _mdl.payperiod : "";

                        if (dt.Rows.Count > 0)
                        {
                            var table = wb.Worksheet(2).Cell(1, 1).InsertTable(dt);
                            wb.Worksheet(2).Tables.FirstOrDefault().ShowAutoFilter = false;
                            table.HeadersRow().Style.Fill.BackgroundColor = XLColor.MediumElectricBlue;
                        }
                        else
                        {
                            var table = wb.Worksheet(2).Cell(4, 7).Value = EmptySheet.ToUpper();
                        }

                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            base64String = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public virtual async Task<string> GetErrorLogReport(LoggedInUser user, List<ErrorLogResponseModel> results, ErrorLogRequestModel _mdl, string fileName)
        {
            try
            {
                string base64String;

                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                DataTable dt = converter.ToDataTable(results);
                DateTime? startdate = _mdl.startdate.Value.TimeOfDay == TimeSpan.Zero ? _mdl.startdate : _dateTimeHelper.GetDateTimeWithTimezone(_mdl.startdate);
                DateTime? enddate = _mdl.enddate.Value.TimeOfDay == TimeSpan.Zero ? _mdl.enddate : _dateTimeHelper.GetDateTimeWithTimezone(_mdl.enddate);

                dt = ProcesDTHeaders(dt);
                dt.Columns["Employee I D"].ColumnName = "Employee ID";
                dt.Columns["Request Details Id"].ColumnName = "Request ID";

                dt.Columns.Remove("id");
                dt.Columns.Remove("Client Name");
                dt.Columns.Remove("Start Date");
                dt.Columns.Remove("End Date");
                dt.Columns.Remove("Line");

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;

                    using (var wb = new XLWorkbook(memStream))
                    {
                        //Todo :Report / sheetname should not exceed 31 chars Issue create  Name Generic / common function - temp fix applied for time being
                        var Tabname = _reportServiceHelper.GenerateSheetName(_mdl.paygroup, "EL");
                        var sheet = wb.Worksheet(1);
                        var sheet2 = wb.Worksheet(2);
                        sheet.Name = "Summary";
                        sheet2.Name = Tabname.Trim();
                        string EmptySheet = "There is no data";

                        wb.Worksheet(1).Cell(8, 6).Value = _mdl != null ? _mdl.paygroup : "";
                        wb.Worksheet(1).Cell(9, 6).Value = startdate != null ? startdate?.ToString("yyyy-MM-dd") : "";
                        wb.Worksheet(1).Cell(10, 6).Value = enddate != null ? enddate?.ToString("yyyy-MM-dd") : "";

                        if (dt.Rows.Count > 0)
                        {
                            var table = wb.Worksheet(2).Cell(1, 1).InsertTable(dt);
                            wb.Worksheet(2).Tables.FirstOrDefault().ShowAutoFilter = false;
                            table.HeadersRow().Style.Fill.BackgroundColor = XLColor.MediumElectricBlue;
                        }
                        else { var table = wb.Worksheet(2).Cell(4, 7).Value = EmptySheet.ToUpper(); }

                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            base64String = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                InsertError(user, 0, "Application Service", "400", "GetErrorLogReport-1958", ex.ToString());
                return null;
            }
        }

        public virtual List<DataTable> ProcesPeriodChangeDT(List<DataTable> dts)
        {
            try
            {
                //For having space inbetween header texts
                foreach (DataTable dt in dts)
                {
                    var columnCount = dt.Columns.Count;
                    for (int i = 0; i < columnCount; i++)
                    {
                        dt.Columns[i].ColumnName = Regex.Replace(dt.Columns[i].ColumnName, "(\\B[A-Z])", " $1"); ;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return dts;
        }

        /// <summary>
        /// For Iterating the columns present in DT and modify the Headers as per the requirement
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public virtual DataTable ProcesDTHeaders(DataTable dt)
        {
            try
            {
                //For having space inbetween header texts

                var columnCount = dt.Columns.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    dt.Columns[i].ColumnName = Regex.Replace(dt.Columns[i].ColumnName, "(\\B[A-Z])", " $1"); ;
                }


            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        /// <summary>
        /// Converts the lists to DataTables then DT gets inserted into Excel Sheets
        /// </summary>
        /// <param name="data"></param>
        /// <param name="requestModel"></param>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public virtual async Task<string> GetPeriodChangeFile(LoggedInUser user, PeriodChangeFileDataModel data, PcFileRequestModel requestModel, string fileName, string interfacetype, string sheetName)
        {
            try
            {
                var dts = new List<DataTable>();
                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                DataTable persDT = converter.ToDataTable(data.personal); dts.Add(persDT);
                DataTable jdetDT = converter.ToDataTable(data.jobs); dts.Add(jdetDT);
                DataTable slryDT = converter.ToDataTable(data.salary); dts.Add(slryDT);
                DataTable addrDT = converter.ToDataTable(data.address); dts.Add(addrDT);
                DataTable bankDT = converter.ToDataTable(data.bank); dts.Add(bankDT);
                DataTable cspfDT = converter.ToDataTable(data.cspf); dts.Add(cspfDT);
                DataTable confDT = converter.ToDataTable(data.conf); dts.Add(confDT);
                DataTable paydDT = converter.ToDataTable(data.payD); dts.Add(paydDT);
                DataTable timeDT = converter.ToDataTable(data.time); dts.Add(timeDT);
                DataTable starterDT = converter.ToDataTable(data.starters); dts.Add(starterDT);
                DataTable leaverDT = converter.ToDataTable(data.leavers); dts.Add(leaverDT);

                dts = ProcesPeriodChangeDT(dts);
                if (paydDT.Columns.Contains("Pay Element Code"))
                {
                    paydDT.Columns["Pay Element Code"].ColumnName = "Local Pay Code";
                }
                if (paydDT.Columns.Contains("Export Code"))
                {
                    paydDT.Columns["Export Code"].ColumnName = "Partner Pay Code";
                }
                persDT.Columns["Employee I D"].ColumnName = "Employee ID";
                jdetDT.Columns["Employee I D"].ColumnName = "Employee ID";
                slryDT.Columns["Employee I D"].ColumnName = "Employee ID";
                addrDT.Columns["Employee I D"].ColumnName = "Employee ID";
                bankDT.Columns["Employee I D"].ColumnName = "Employee ID";
                cspfDT.Columns["Employee I D"].ColumnName = "Employee ID";
                confDT.Columns["Employee I D"].ColumnName = "Employee ID";
                paydDT.Columns["Employee I D"].ColumnName = "Employee ID";
                timeDT.Columns["Employee I D"].ColumnName = "Employee ID";
                starterDT.Columns["Employee I D"].ColumnName = "Employee ID";
                leaverDT.Columns["Employee I D"].ColumnName = "Employee ID";

                persDT.Columns["P P N"].ColumnName = "PPN";
                jdetDT.Columns["P P N"].ColumnName = "PPN";
                slryDT.Columns["P P N"].ColumnName = "PPN";
                addrDT.Columns["P P N"].ColumnName = "PPN";
                bankDT.Columns["P P N"].ColumnName = "PPN";
                cspfDT.Columns["P P N"].ColumnName = "PPN";
                paydDT.Columns["P P N"].ColumnName = "PPN";
                timeDT.Columns["P P N"].ColumnName = "PPN";

                bankDT.Columns["I B A N Code"].ColumnName = "IBAN Code";
                jdetDT.Columns["Positions"].ColumnName = "Position";

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;

                    using (var wb = new XLWorkbook(memStream))
                    {

                        wb.Worksheet(1).Cell(9, 6).Value = requestModel != null ? requestModel.PaygroupCode : "";
                        wb.Worksheet(1).Cell(10, 6).Value = interfacetype != null ? interfacetype.ToLower() == "xml" ? "ConnectedPay" : "DPI" : "DPI";
                        wb.Worksheet(1).Cell(11, 6).Value = requestModel != null ? requestModel.PayPeriod != null ? "By Pay Period" : "Changed Date" : "";

                        if (requestModel.PayPeriod != null)
                        {
                            wb.Worksheet(1).Cell(12, 6).Value = requestModel != null ? requestModel.PayPeriod : "";
                            wb.Worksheet(1).Cell(13, 6).Value = "";
                            wb.Worksheet(1).Cell(14, 6).Value = "";
                        }
                        else
                        {
                            if (requestModel != null)
                            {
                                var startDateCell = wb.Worksheet(1).Cell(13, 6);
                                var endDateCell = wb.Worksheet(1).Cell(14, 6);

                                if (requestModel.StartDate.HasValue)
                                {
                                    startDateCell.Value = requestModel.StartDate.Value.TimeOfDay != TimeSpan.Zero ? _dateTimeHelper.GetDateTimeWithTimezone(requestModel.StartDate).Value.Date : requestModel.StartDate.Value.Date;
                                    startDateCell.Style.DateFormat.Format = "dd-MM-yyyy"; 
                                }
                                else
                                {
                                    startDateCell.Value = "";
                                }

                                if (requestModel.EndDate.HasValue)
                                {
                                    endDateCell.Value = requestModel.EndDate.Value.TimeOfDay != TimeSpan.Zero ? _dateTimeHelper.GetDateTimeWithTimezone(requestModel.EndDate).Value.Date : requestModel.EndDate.Value.Date;
                                    endDateCell.Style.DateFormat.Format = "dd-MM-yyyy"; 
                                }
                                else
                                {
                                    endDateCell.Value = "";
                                }
                            }
                            else
                            {
                                wb.Worksheet(1).Cell(13, 6).Value = "";
                                wb.Worksheet(1).Cell(14, 6).Value = "";
                            }
                        }

                        for (int k = 2; k <= 13; k++)
                        {
                            if (k == 13)
                            {
                                continue;
                            }

                            var table = wb.Worksheet(k).Cell(1, 1).InsertTable(dts[k - 2]);
                            wb.Worksheet(k).Tables.FirstOrDefault().ShowAutoFilter = false;
                            table.HeadersRow().Style.Fill.BackgroundColor = XLColor.MediumElectricBlue;
                        }

                        using var ms = new MemoryStream();
                        wb.SaveAs(ms);

                        ms.Position = 0; // Reset the stream position to the beginning

                        var status = await UploadtoS3andUpdatereportservicedetails(ms, sheetName, Newtonsoft.Json.JsonConvert.SerializeObject(requestModel), user, requestModel.PaygroupCode); // Hve to implement error handling based on return value

                        var result = Convert.ToBase64String(ms.ToArray());
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                throw ex;
            }
        }
        private async Task<string> ConvertDataTablesToExcelFile(List<DataTable> dataTables, PcFileRequestModel requestModel, string interfacetype, string fileName, string countryCode, string sheetName, LoggedInUser user)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);

                    using (var wb = new XLWorkbook(memStream))
                    {
                        wb.Worksheet(1).Cell(9, 6).Value = requestModel?.PaygroupCode ?? "";
                        wb.Worksheet(1).Cell(10, 6).Value = interfacetype?.ToLower() == "xml" ? "ConnectedPay" : "DPI";
                        wb.Worksheet(1).Cell(11, 6).Value = requestModel?.PayPeriod != null ? "By Pay Period" : "Changed Date";

                        if (requestModel?.PayPeriod != null)
                        {
                            wb.Worksheet(1).Cell(12, 6).Value = requestModel.PayPeriod;
                            wb.Worksheet(1).Cell(13, 6).Value = "";
                            wb.Worksheet(1).Cell(14, 6).Value = "";
                        }
                        else
                        {
                            var startDateCell = wb.Worksheet(1).Cell(13, 6);
                            var endDateCell = wb.Worksheet(1).Cell(14, 6);

                            startDateCell.Value = requestModel.StartDate != null ? (requestModel.StartDate.Value.TimeOfDay != TimeSpan.Zero ? _dateTimeHelper.GetDateTimeWithTimezone(requestModel.StartDate).Value.Date : requestModel.StartDate.Value.Date) : "";
                            endDateCell.Value = requestModel.EndDate != null ? (requestModel.EndDate.Value.TimeOfDay != TimeSpan.Zero ? _dateTimeHelper.GetDateTimeWithTimezone(requestModel.EndDate).Value.Date : requestModel.EndDate.Value.Date) : "";


                            startDateCell.Style.DateFormat.Format = "dd-MM-yyyy";
                            endDateCell.Style.DateFormat.Format = "dd-MM-yyyy";
                        }

                        if (countryCode.Equals("MEX"))
                        {
                            for (int i = 0; i < dataTables.Count; i++)
                            {
                                var dataTable = dataTables[i];
                                if (dataTable != null && dataTable.Rows.Count > 0)
                                {
                                    wb.Worksheet(i + 2).Cell(2, 1).InsertData(dataTable.AsEnumerable());
                                }
                            }
                        }
                        else
                        {
                            var tabname2 = _reportServiceHelper.GenerateSheetName(requestModel?.PaygroupCode, "HR");
                            var tabname3 = _reportServiceHelper.GenerateSheetName(requestModel?.PaygroupCode, "PAYD");
                            var tabname4 = _reportServiceHelper.GenerateSheetName(requestModel?.PaygroupCode, "WFM");

                            wb.Worksheet(2).Name = tabname2;
                            wb.Worksheet(3).Name = tabname3;
                            wb.Worksheet(4).Name = tabname4;

                            for (int k = 2; k <= 5; k++)
                            {
                                if (k == 5)
                                {
                                    continue;
                                }
                                var sheet = wb.Worksheet(k);
                                var dataTable = dataTables[k - 2];
                                int rowStart = 2;
                                int colStart = 1;

                                for (int i = 0; i < dataTable.Rows.Count; i++)
                                {
                                    for (int j = 0; j < dataTable.Columns.Count; j++)
                                    {
                                        sheet.Cell(rowStart + i, colStart + j).Value = dataTable.Rows[i][j].ToString();
                                    }
                                }
                            }
                        }

                        using var ms = new MemoryStream();
                        wb.SaveAs(ms);

                        ms.Position = 0; // Reset the stream position to the beginning

                        var status = await UploadtoS3andUpdatereportservicedetails(ms, sheetName, Newtonsoft.Json.JsonConvert.SerializeObject(requestModel), user, requestModel.PaygroupCode); // Hve to implement error handling based on return value

                        var result = Convert.ToBase64String(ms.ToArray());
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while generating excel for Mexico report: {exMessage}", ex.Message);
                return null;
            }
        }

        private async Task<bool> UploadtoS3andUpdatereportservicedetails(MemoryStream ms, string sheetName, string? reportFilter, LoggedInUser user, string paygrpCode)
        {
            try
            {
                // Create a PutObjectRequest with the memory stream directly
                PutObjectRequest s3request = new PutObjectRequest
                {
                    BucketName = _reportOutputBucketName,
                    Key = sheetName, // File name in S3
                    InputStream = ms, // Use the memory stream directly (not base64)
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" // Correct content type for Excel
                };

                // Upload to S3
                PutObjectResponse s3response = await _s3Client.PutObjectAsync(s3request);
                if (s3response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("Upload to S3 successful!.fileName -  {sheetName}", sheetName);
                }
                else
                {
                    _logger.LogError("Error in uploading to S3");
                    return false;
                }

                //write to Db the uploaded report details reportServiceDetails
                var newEntry = new ReportServiceDetails
                {
                    PayGroupCode = paygrpCode,
                    S3ObjectId = sheetName,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedBy = user.UserName.ToLower(),
                    ModifiedBy = user.UserName.ToLower(),
                    ReportFilter = reportFilter
                };

                await _appDbContext.reportservicedetails.AddAsync(newEntry);
                await _appDbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Occured in UploadtoS3andUpdatereportservicedetails... fileName -  {ex}", ex);
                return false;
            }
        }

        #endregion S3
        public virtual List<int> GetPayPeriodsByPayGroupID(LoggedInUser user, int payGroupID)
        {
            try
            {
                var _payPeriods = (from p in _appDbContext.Set<PayCalendar>()
                                   where p.paygroupid == payGroupID
                                   select p.payperiod).Distinct().ToList();
                return _payPeriods;

            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public virtual List<int> GetPayPeriodsByPayGroupCode(LoggedInUser user, string strPayGroupCode)
        {
            try
            {
                var _payPeriods = (from p in _appDbContext.Set<PayGroup>()
                                   join pc in _appDbContext.Set<PayCalendar>() on p.id equals pc.paygroupid
                                   where p.code == strPayGroupCode
                                   select pc.payperiod).Distinct().ToList();
                return _payPeriods;

            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public virtual string GetCountryByClientCode(LoggedInUser user, string paygroup)
        {
            try
            {
                var _country = (from pg in _appDbContext.Set<PayGroup>()
                                join CO in _appDbContext.Set<Country>() on pg.countryid equals CO.id
                                where pg.code == paygroup
                                select CO.code).FirstOrDefault();
                return _country;

            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public virtual List<int> GetPayPeriodsByCountryCode(LoggedInUser user, int? iCountryID)
        {
            try
            {
                var _payPeriods = (from C in _appDbContext.Set<Client>()
                                   join LE in _appDbContext.Set<LegalEntity>() on C.id equals LE.clientid
                                   join pg in _appDbContext.Set<PayGroup>() on LE.id equals pg.legalentityid
                                   join pc in _appDbContext.Set<PayCalendar>() on pg.id equals pc.paygroupid
                                   where pg.countryid == iCountryID
                                   select pc.payperiod).Distinct().ToList();
                return _payPeriods;

            }
            catch (Exception ex)
            {

            }
            return null;
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

                var res = _payPeriods.Where(t => t.date?.Year.ToString() == year.ToString()).Select(t => t.payperiod).ToList();
                return res;

            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public virtual int GetPayGroupIDByCode(LoggedInUser user, string strCode)
        {
            var response = (from p in _appDbContext.Set<PayGroup>()
                            where p.code == strCode
                            select p.id).FirstOrDefault();

            return response;

        }
        public virtual string GetFormattedDateString(string sDate)
        {
            DateTime temp;
            if (DateTime.TryParse(sDate, out temp))
            {
                if (temp != DateTime.MinValue)
                {
                    return temp.ToString("yyyy-MM-dd");
                }
                else
                {
                    return "";
                }

            }
            return "";

        }

        public virtual PeriodChangeFileRequestModel SetStartandEndDateBasedonPayperiod(LoggedInUser user, PeriodChangeFileRequestModel _req)
        {
            int paygID = GetPayGroupIDByCode(user, _req.paygroup);
            _req.paygroupid = paygID;
            if (_req != null)
            {
                if (_req.payperiod != null)
                {
                    _req.enddate = GetPayCalendarByPayPeriod(user, Convert.ToInt32(_req.payperiod), paygID, Convert.ToInt32(_req.year))?.date;
                    if (_req.payperiod > 1)
                    {
                        _req.startdate = GetPayCalendarByPayPeriod(user, Convert.ToInt32(_req.payperiod) - 1, paygID, Convert.ToInt32(_req.year))?.date;
                    }
                    else
                    {
                        var year = _req.enddate?.Year;
                        var payperiod = GetPayPeriodsByYear(user, paygID, (Convert.ToInt32(year)) - 1).Distinct().OrderByDescending(t => t).FirstOrDefault();
                        if (payperiod == 0)
                        {
                            _req.startdate = null;
                        }
                        else
                        {
                            var _date = _appDbContext.paycalendar.Where(t => t.paygroupid == paygID && t.payperiod == payperiod && t.taskid == "3" && t.year == year).OrderByDescending(t => t.date).LastOrDefault();
                            _req.startdate = _date?.date;
                        }
                    }

                }
            }
            return _req;
        }

        public virtual PayCalendar GetPayCalendarByPayPeriod(LoggedInUser user, int payperiod, int paygroupid, int year)
        {
            PayCalendar _payC = new PayCalendar();
            try
            {
                _payC = _appDbContext.paycalendar.Where(t => t.paygroupid == paygroupid && t.payperiod == payperiod && t.taskid == "3" && t.year == year).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _payC;
        }

        public virtual DataTable ChangeDateFormatColumn(DataTable dt)
        {
            //foreach (DataRow item in dt.Rows)
            //{

            //}


            //dt.Columns["DateOfOrder"]. Convert(val => DateTime.Parse(val.ToString()).ToString("dd/MMM/yyyy"));

            return dt;
        }

        public virtual bool CanView(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return false;
        }
    }


    public class ListtoDataTableConverter
    {
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows

                    values[i] = Props[i].GetValue(item, null);
                    if (values[i] != null ? values[i].GetType() == typeof(DateTime) : false)
                    {
                        //var b = DateTime.ParseExact(values[i].ToString(), "yyyy-MM-dd", null);

                        values[i] = DateTime.Parse(Props[i].GetValue(item, null).ToString()).ToString("yyyy-MM-dd");
                    }

                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
