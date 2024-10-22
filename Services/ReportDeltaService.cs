using Services.Abstractions;
using Microsoft.Extensions.Logging;
using Persistence;
using Domain.Models;
using Amazon.S3;
using Amazon.S3.Model;
using ClosedXML.Excel;
using System.Data;
using Services.Helpers;
using Domain.Models.Users;
using Domain.Enums;
using System.Text.RegularExpressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Services
{

    public class ReportDeltaService : IReportDeltaService
    {
        protected readonly ILogger<ReportDeltaService> _logger;
        protected readonly ISelectListHelper _selectListHelper;
        protected readonly IReportServiceHelper _reportServiceHelper;
        protected IAmazonS3 _s3Client { get; set; }
        protected readonly AppDbContext _appDbContext;
        protected readonly IDateTimeHelper _dateTimeHelper;
        private readonly string _reportTemplateBucketName = "ultipay-report-template";

        public ReportDeltaService(AppDbContext appDbContext, IAmazonS3 s3Client, ILogger<ReportDeltaService> logger, ISelectListHelper selectListHelper, IReportServiceHelper reportServiceHelper, IDateTimeHelper dateTimeHelper)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _s3Client = s3Client;
            _selectListHelper = selectListHelper;
            _reportTemplateBucketName = Environment.GetEnvironmentVariable("UltipayReportTemplateBucket") ?? "ultipay-report-template";
            _reportServiceHelper = reportServiceHelper;
            _dateTimeHelper = dateTimeHelper;
        }

        public bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public bool CanView(LoggedInUser user)
        {
            return true;
        }

        public virtual List<DeltaResponseModel> GetDeltaReport(LoggedInUser user, DeltaRequestModel _requestModel)
        {
            List<DeltaResponseModel> lstDeltaResponseModel = new List<DeltaResponseModel>();
            try
            {
                if (IsAuthorisedtoAccess(user, _requestModel))
                {
                    _requestModel.enddate = _requestModel.enddate != null ? _requestModel.enddate.Value : _dateTimeHelper.GetDateTimeNow();
                    var startTime = _requestModel.startdate.Value.TimeOfDay == TimeSpan.Zero ? TimeSpan.Zero : _requestModel.startdate.Value.TimeOfDay;
                    var endTime = _requestModel.enddate.Value.TimeOfDay == TimeSpan.Zero ? new TimeSpan(23, 59, 59) : _requestModel.enddate.Value.TimeOfDay;
                    if (_requestModel.filterby.ToLower() == "pp" && _requestModel.year != null && _requestModel.payperiod != null)
                    {
                        var paygroup = _appDbContext.paygroup.Where(x => x.code == _requestModel.paygroup).ToList().First();
                        var payPeriods = _appDbContext.paycalendar
                                        .Where(pc => pc.taskid == "3" &&
                                                     pc.paygroupid == 38 &&
                                                     string.Compare(pc.year.ToString() + pc.payperiod.ToString().PadLeft(2, '0'), _requestModel.year.ToString() + _requestModel.payperiod.ToString().PadLeft(2, '0')) <= 0)
                                        .OrderByDescending(pc => pc.year)
                                        .ThenByDescending(pc => pc.payperiod)
                                        .Take(2)
                                        .ToList();

                        var payStartDate = payPeriods.Count() == 1 ? payPeriods[0].date : payPeriods[1].date;
                        var payEndDate = payPeriods[0].date;
                        if (payStartDate.HasValue)
                        {
                            payStartDate = _dateTimeHelper.SetDateTimeWithTime(payStartDate,startTime).Value.AddSeconds(1);  
                        }
                        if (payEndDate.HasValue)
                        {
                            payEndDate = _dateTimeHelper.SetDateTimeWithTime(payEndDate,endTime);
                        }
                        lstDeltaResponseModel = GetChangelogEntries(_requestModel.paygroup, payStartDate.Value, payEndDate.Value);
                    }
                    else if (_requestModel.filterby.ToLower() == "date" && _requestModel.startdate != null && _requestModel.enddate != null)
                    {
                        _requestModel.startdate = _dateTimeHelper.SetDateTimeWithTime(_requestModel.startdate, startTime);
                        _requestModel.enddate = _dateTimeHelper.SetDateTimeWithTime(_requestModel.enddate, endTime);
                        lstDeltaResponseModel = GetChangelogEntries(_requestModel.paygroup, _requestModel.startdate, _requestModel.enddate);
                    }
                }
                else
                {
                    throw new UnauthorizedAccessException();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return lstDeltaResponseModel;
        }

        public virtual List<DeltaResponseModel> GetChangelogEntries(string paygroupId, DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                return new List<DeltaResponseModel>();
            }

            var personalEntries = _appDbContext.changelog
           .Where(x => x.tablename == "Personal" &&
               x.paygroupcode == paygroupId &&
               x.modifiedat >= startDate.Value &&
               x.modifiedat <= endDate.Value)
           .OrderBy(x => x.employeeid)
           .ThenBy(x => x.tablename)
           .ThenBy(x => x.fieldname)
           .GroupBy(x => new { x.employeeid, x.tablename, x.fieldname })
           .Select(group => new
           {
               OldestChange = group.OrderBy(x => x.modifiedat).FirstOrDefault(),
               NewestChange = group.OrderByDescending(x => x.modifiedat).FirstOrDefault()
           })
           .ToList()
           .Where(result => result.OldestChange == result.NewestChange || result.OldestChange.oldvalue != result.NewestChange.newvalue)
           .Select(result => new Domain.Entities.ChangeLog
           {
               paygroupcode = result.NewestChange.paygroupcode,
               employeeid = result.NewestChange.employeeid,
               tablename = result.NewestChange.tablename,
               fieldname = result.NewestChange.fieldname,
               recordtype = result.NewestChange.recordtype,
               oldvalue = result.OldestChange.oldvalue,
               oldeffectivedate = String.IsNullOrEmpty(result.OldestChange.oldvalue) ? null : startDate.Value.AddDays(-1),
               newvalue = result.NewestChange.newvalue,
               neweffectivedate = result.NewestChange.neweffectivedate,
               modifieddate = result.NewestChange.modifieddate,
               recordid = result.NewestChange.recordid,
               createdby = result.NewestChange.createdby,
               createdat = result.NewestChange.createdat,
               modifiedby = result.NewestChange.modifiedby,
               modifiedat = result.NewestChange.modifiedat
           })
           .ToList();

            var addressChanges = (from changelog in _appDbContext.changelog
                                  join historyAddress in _appDbContext.historyemployeeaddress
                                  on new { RecordId = changelog.recordid } equals new { RecordId = historyAddress.id.ToString() }
                                  where changelog.tablename == "Address" &&
                                        changelog.paygroupcode == paygroupId &&
                                        changelog.modifiedat >= startDate.Value &&
                                        changelog.modifiedat <= endDate.Value
                                  select new
                                  {
                                      // Select properties you need from both tables
                                      ChangeLog = changelog,
                                      AddressHistory = historyAddress
                                  }).ToList()
                                   .OrderBy(x => x.ChangeLog.employeeid)
                 .ThenBy(x => x.AddressHistory.addresstype)
                 .ThenBy(x => x.ChangeLog.fieldname)
                 .GroupBy(x => new { x.ChangeLog.employeeid, x.AddressHistory.addresstype, x.ChangeLog.fieldname })
                 .Select(group => new
                 {
                     OldestChange = group.OrderBy(x => x.ChangeLog.modifiedat).FirstOrDefault().ChangeLog,
                     NewestChange = group.OrderByDescending(x => x.ChangeLog.modifiedat).FirstOrDefault().ChangeLog
                 })
                 .Where(result => result.OldestChange == result.NewestChange || result.OldestChange.oldvalue != result.NewestChange.newvalue)
                 .Select(result => new Domain.Entities.ChangeLog
                 {
                     paygroupcode = result.NewestChange.paygroupcode,
                     employeeid = result.NewestChange.employeeid,
                     tablename = result.NewestChange.tablename,
                     fieldname = result.NewestChange.fieldname,
                     recordtype = result.NewestChange.recordtype,
                     oldvalue = result.OldestChange.oldvalue,
                     oldeffectivedate = result.OldestChange.oldeffectivedate,
                     newvalue = result.NewestChange.newvalue,
                     neweffectivedate = result.NewestChange.neweffectivedate,
                     modifieddate = result.NewestChange.modifieddate,
                     recordid = result.NewestChange.recordid,
                     createdby = result.NewestChange.createdby,
                     createdat = result.NewestChange.createdat,
                     modifiedby = result.NewestChange.modifiedby,
                     modifiedat = result.NewestChange.modifiedat
                 })
                 .ToList();

            var bankchanges = (from changelog in _appDbContext.changelog
                               join historyAddress in _appDbContext.historyemployeebank
                               on new { RecordId = changelog.recordid } equals new { RecordId = historyAddress.id.ToString() }
                               where changelog.tablename == "Bank" &&
                                     changelog.paygroupcode == paygroupId &&
                                     changelog.modifiedat >= startDate.Value &&
                                     changelog.modifiedat <= endDate.Value
                               select new
                               {
                                   // Select properties you need from both tables
                                   ChangeLog = changelog,
                                   AddressHistory = historyAddress
                               }).ToList()
                                   .OrderBy(x => x.ChangeLog.employeeid)
                 .ThenBy(x => x.AddressHistory.accounttype)
                 .ThenBy(x => x.ChangeLog.fieldname)
                 .GroupBy(x => new { x.ChangeLog.employeeid, x.AddressHistory.accounttype, x.ChangeLog.fieldname })
                 .Select(group => new
                 {
                     OldestChange = group.OrderBy(x => x.ChangeLog.modifiedat).FirstOrDefault().ChangeLog,
                     NewestChange = group.OrderByDescending(x => x.ChangeLog.modifiedat).FirstOrDefault().ChangeLog
                 })
                 .Where(result => result.OldestChange == result.NewestChange || result.OldestChange.oldvalue != result.NewestChange.newvalue)
                 .Select(result => new Domain.Entities.ChangeLog
                 {
                     paygroupcode = result.NewestChange.paygroupcode,
                     employeeid = result.NewestChange.employeeid,
                     tablename = result.NewestChange.tablename,
                     fieldname = result.NewestChange.fieldname,
                     recordtype = result.NewestChange.recordtype,
                     oldvalue = result.OldestChange.oldvalue,
                     oldeffectivedate = result.OldestChange.oldeffectivedate,
                     newvalue = result.NewestChange.newvalue,
                     neweffectivedate = result.NewestChange.neweffectivedate,
                     modifieddate = result.NewestChange.modifieddate,
                     recordid = result.NewestChange.recordid,
                     createdby = result.NewestChange.createdby,
                     createdat = result.NewestChange.createdat,
                     modifiedby = result.NewestChange.modifiedby,
                     modifiedat = result.NewestChange.modifiedat
                 })
                 .ToList();

            var confChanges = (from changelog in _appDbContext.changelog
                               join historyAddress in _appDbContext.historyemployeeconf
                               on new { RecordId = changelog.recordid } equals new { RecordId = historyAddress.id.ToString() }
                               where changelog.tablename == "Conf" &&
                                     changelog.paygroupcode == paygroupId &&
                                     changelog.modifiedat >= startDate.Value &&
                                     changelog.modifiedat <= endDate.Value
                               select new
                               {
                                   // Select properties you need from both tables
                                   ChangeLog = changelog,
                                   AddressHistory = historyAddress
                               }).ToList()
                                   .OrderBy(x => x.ChangeLog.employeeid)
                 .ThenBy(x => x.AddressHistory.documenttype)
                 .ThenBy(x => x.ChangeLog.fieldname)
                 .GroupBy(x => new { x.ChangeLog.employeeid, x.AddressHistory.documenttype, x.ChangeLog.fieldname })
                 .Select(group => new
                 {
                     OldestChange = group.OrderBy(x => x.ChangeLog.modifiedat).FirstOrDefault().ChangeLog,
                     NewestChange = group.OrderByDescending(x => x.ChangeLog.modifiedat).FirstOrDefault().ChangeLog
                 })
                 .Where(result => result.OldestChange == result.NewestChange || result.OldestChange.oldvalue != result.NewestChange.newvalue)
                 .Select(result => new Domain.Entities.ChangeLog
                 {
                     paygroupcode = result.NewestChange.paygroupcode,
                     employeeid = result.NewestChange.employeeid,
                     tablename = result.NewestChange.tablename,
                     fieldname = result.NewestChange.fieldname,
                     recordtype = result.NewestChange.recordtype,
                     oldvalue = result.OldestChange.oldvalue,
                     oldeffectivedate = result.OldestChange.oldeffectivedate,
                     newvalue = result.NewestChange.newvalue,
                     neweffectivedate = result.NewestChange.neweffectivedate,
                     modifieddate = result.NewestChange.modifieddate,
                     recordid = result.NewestChange.recordid,
                     createdby = result.NewestChange.createdby,
                     createdat = result.NewestChange.createdat,
                     modifiedby = result.NewestChange.modifiedby,
                     modifiedat = result.NewestChange.modifiedat
                 })
                 .ToList();

            var otherEntries = _appDbContext.changelog
                .Where(x =>
                    (new[] { "Job", "Pay Deduction", "Country Specific", "Salary" }).Contains(x.tablename) &&
                    x.paygroupcode == paygroupId &&
                    x.modifiedat >= startDate.Value &&
                    x.modifiedat <= endDate.Value)
                .OrderByDescending(x => x.modifiedat)
                .ToList();

            // Concatenate all the entries into a single list
            var changelogEntries = addressChanges.Concat(otherEntries).ToList().Concat(confChanges).Concat(bankchanges).Concat(personalEntries).ToList();

            return changelogEntries.Select(x => new DeltaResponseModel
            {
                Paygroup = x.paygroupcode,
                EmployeeID = x.employeeid,
                TableName = x.tablename,
                FieldName = x.fieldname,
                RecordType = x.recordtype,
                OldValue = x.oldvalue,
                OldEffectiveDate = x.oldeffectivedate,
                NewValue = x.newvalue,
                NewEffectiveDate = x.neweffectivedate,
                ModifiedDate = x.modifieddate,
                Type = GetType(x.neweffectivedate, startDate, endDate)
            }).OrderBy(x => x.EmployeeID).
            ThenBy(x => x.GetTablePriority(x.TableName)).
            ThenBy(x => x.NewEffectiveDate).
            ThenBy(x => x.FieldName).
            ToList();
        }

        ChangeEffectiveType GetType(DateTime? newEffectiveDate, DateTime? startDate, DateTime? endDate)
        {
            if (newEffectiveDate.HasValue && startDate.HasValue && endDate.HasValue)
            {
                if (newEffectiveDate >= startDate && newEffectiveDate <= endDate)
                {
                    return ChangeEffectiveType.CurrentPeriod;
                }
                else if (newEffectiveDate < startDate)
                {
                    return ChangeEffectiveType.Retrospect;
                }
                else
                {
                    return ChangeEffectiveType.FutureDated;
                }
            }
            else
            {
                return ChangeEffectiveType.Retrospect;
            }
        }

        public virtual bool IsAuthorisedtoAccess(LoggedInUser user, DeltaRequestModel _requestModel)
        {
            return true;
        }

        public virtual async Task<string> GetDeltaReport(LoggedInUser user, List<DeltaResponseModel> results, DeltaRequestModel _requestModel, string fileName)
        {
            try
            {
                string base64String;
                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                DataTable dt = converter.ToDataTable(results);
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _reportTemplateBucketName,
                    Key = fileName
                };
                string clientcode = await _reportServiceHelper.GetClientCodeByPayGroup(_requestModel.paygroup);
                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream memStream = new MemoryStream())
                {
                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;
                    using (var wb = new XLWorkbook(memStream))
                    {
                        var sheetname = _reportServiceHelper.GenerateSheetName(_requestModel.paygroup, "DeltRep");
                        var sheet = wb.Worksheet(1);

                        sheet.Name = sheetname;

                        sheet.Cell(6, 1).Value = "Delta Report";

                        sheet.Cell(8, 1).Value = "Client Name";
                        sheet.Cell(9, 1).Value = "Paygroup";
                        sheet.Cell(10, 1).Value = "Filter By";
                        sheet.Cell(9, 2).Value = _requestModel != null ? _requestModel.paygroup : "";
                        sheet.Cell(8, 2).Value = string.IsNullOrEmpty(clientcode) ? "" : clientcode;
                        if (_requestModel.filterby.ToLower() == "pp" && _requestModel.year != null && _requestModel.payperiod != null)
                        {
                            sheet.Cell(10, 2).Value = "Pay Period";

                            sheet.Cell(10, 1).Value = "Pay Period";
                            sheet.Cell(10, 2).Value = _requestModel != null ? _requestModel.payperiod : "";

                        }
                        else if (_requestModel.filterby.ToLower() == "date" && _requestModel.startdate != null && _requestModel.enddate != null)
                        {
                            sheet.Cell(10, 2).Value = "Date";
                            sheet.Cell(11, 1).Value = "Start Date";
                            sheet.Cell(12, 1).Value = "End Date";
                            sheet.Cell(11, 2).Value = _requestModel.startdate != null ? (_requestModel.startdate.Value.TimeOfDay != TimeSpan.Zero ? _dateTimeHelper.GetDateTimeWithTimezone(_requestModel.startdate).Value.ToString("yyyy-MM-dd") : _requestModel.startdate.Value.ToString("yyyy-MM-dd")) : "";
                            sheet.Cell(12, 2).Value = _requestModel.enddate != null ? (_requestModel.enddate.Value.TimeOfDay != new TimeSpan(23, 59, 59) ? _dateTimeHelper.GetDateTimeWithTimezone(_requestModel.enddate).Value.ToString("yyyy-MM-dd") : _requestModel.enddate.Value.ToString("yyyy-MM-dd")) : "";
                        }

                        var table = sheet.Cell(14, 1).InsertTable(dt);

                        sheet.Tables.FirstOrDefault().ShowAutoFilter = false;
                        table.HeadersRow().Style.Fill.BackgroundColor = XLColor.SteelBlue;
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
    }
}
