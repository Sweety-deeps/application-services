using Amazon.SQS;
using ClosedXML.Excel;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Persistence;
using Services.Abstractions;
using Services.Dayforce;
using Services.Helpers;

namespace Services
{
    public class GPRIService : IGPRIService
    {
        private readonly ILogger<GPRIService> _logger;
        protected readonly AppDbContext _appDbContext;
        private readonly IDayforceApiClient _dayforceApiClient;
        private readonly IS3Handling _S3Handling;
        private readonly Dictionary<string, IDataImportService> _dataImportServices;
       


        private readonly Config _config;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;
        private readonly IDayforceSftpClient _dayforceSftpClient;
        private readonly IDateTimeHelper _dateTimeHelper;

        public GPRIService(AppDbContext appDbContext, IDayforceApiClient dayforceApiClient, IS3Handling s3Handling,
            ILogger<GPRIService> logger, IEnumerable<IDataImportService> dataImportServices, Config config,
            ILoggedInUserRoleService loggedInUserRoleService, IDayforceSftpClient dayforceSftpClient, IDateTimeHelper dateTimeHelper)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _dayforceApiClient = dayforceApiClient;
            _S3Handling = s3Handling;
            _dataImportServices = dataImportServices.ToDictionary(service => service.GetType().Namespace);
            _config = config;
            _loggedInUserRoleService = loggedInUserRoleService;
            _dayforceSftpClient = dayforceSftpClient;
            _dateTimeHelper = dateTimeHelper;
        }

        public virtual string GenerateRandomNumber()
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < 13; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }
        public async Task<bool> ValidateGPRIFormatAsync(GPRIModel model)
        {
            if (string.IsNullOrEmpty(model.payrollresults))
            {
                return false;
            }

            try
            {
                string convertBase64 = model.payrollresults.Replace("data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,", string.Empty);
                byte[] excelBytes = Convert.FromBase64String(convertBase64);

                using var ms = new MemoryStream(excelBytes);
                using var workbook = new XLWorkbook(ms);
                var worksheet = workbook.Worksheets.First();

                string firstCellValue = worksheet.Cell(1,1).GetString().Trim();

                return (firstCellValue == "Payroll Name" && model.payrollformat.Equals("Gross to Net")) ||
                       (firstCellValue == "NIT" && model.payrollformat.Equals("Local"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating GPRI");
                return false;
            }
        }
        public virtual async Task<StatusTypes> AddGPRI(LoggedInUser user, GPRIModel model)
        {
            try
            {
                bool valid = await ValidateGPRIFormatAsync(model);
                if (!valid)
                {
                    _logger.LogError("The selected GPRI payroll format does not match the uploaded file type.");
                    return StatusTypes.Invalid;
                }
                var gpri = new GPRI();

                gpri = new GPRI
                {
                    paygroupid = model.paygroupid,
                    fileid = GenerateRandomNumber(),
                    payperiod = model.payperiod,
                    isoffcycle = model.isoffcycle,
                    offcycletype = model.offcycletype,
                    offcyclesuffix = model.offcyclesuffix,
                    overwritepaydate = !string.IsNullOrEmpty(model.overwritepaydate)? DateTime.Parse(model.overwritepaydate): null,
                    isoverride = model.isoverride,
                    sheetnumber = model.sheetnumber,
                    employeerow = string.IsNullOrEmpty(model.employeerow) ? 0 : Convert.ToInt32(model.employeerow),
                    payelementrow = string.IsNullOrEmpty(model.payelementrow) ? 0 : Convert.ToInt32(model.payelementrow),
                    payelementcolumn = string.IsNullOrEmpty(model.payelementcolumn) ? 0 : Convert.ToInt32(model.payelementcolumn),
                    employeecolumn = string.IsNullOrEmpty(model.employeecolumn) ? 0 : Convert.ToInt32(model.employeecolumn),
                    payslipstatus = model.payslipstatus,
                    noofees = model.noofees,
                    noofpayslips = model.noofpayslips,
                    receivedtime = _dateTimeHelper.GetDateTimeNow(),
                    processedtime = null,
                    createdby = user.UserName,
                    createdat = _dateTimeHelper.GetDateTimeNow(),
                    modifiedby = model.modifiedby,
                    modifiedat = null,
                    status = Domain.Constants.PAYROLLPROCESSING,
                    nextstep = "",
                    payperiodyear = model.year

                };

                _appDbContext.gpri.Add(gpri);
                _appDbContext.SaveChanges();
                _appDbContext.Entry(gpri).State = EntityState.Detached;

                string _payGroup = _appDbContext.paygroup.Where(p => p.id == model.paygroupid).Select(p => p.code).FirstOrDefault();

                DataImportRequestModel dataImportRequestModel = new DataImportRequestModel
                {
                    file = model.payrollresults,
                    entityName = "GPRI",
                    entityID = gpri.id,
                    template = model.payrollformat,
                    payGroup = _payGroup
                };

                var dataImportSerivce = _loggedInUserRoleService.GetServiceForController(user, _dataImportServices);
                await dataImportSerivce.UploadDataImport(user, dataImportRequestModel);
                return StatusTypes.Success;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "{ex}", ex);
                return StatusTypes.Failure;
            }
        }

        public virtual async Task UpdateGPRI(LoggedInUser user, string fileId, string status, string next, string sendGpriResult)
        {
            try
            {
                if (status == "payrollapproved")
                {
                    status = Domain.Constants.PAYROLLAPPROVED.ToString();
                    next = Domain.Constants.PAYROLLAPPROVEDNEXT.ToString();
                }
                if (status == "payrollrejected")
                {
                    status = Domain.Constants.PAYROLLREJECTED.ToString();
                    next = Domain.Constants.REJECTEDNEXT.ToString();
                }
                else if (status == "sendgpri")
                {
                    status = Domain.Constants.GPRIPROCESSING.ToString();
                    next = Domain.Constants.GPRIPROCESSINGNEXT.ToString();
                }
                else if (status == "approved")
                {
                    status = Domain.Constants.GPRIAPPROVED.ToString();
                    next = Domain.Constants.GPRIAPPROVEDNEXT.ToString();
                }
                else if(status =="rejected")
                {
                    status = Domain.Constants.GPRIREJECTED.ToString();
                    next = Domain.Constants.REJECTEDNEXT.ToString();
                }
                var record = _appDbContext.gpri.Where(g => g.fileid == fileId);
                var updateRecord = record.FirstOrDefault();
                if (sendGpriResult == string.Empty)
                {
                    sendGpriResult = JsonConvert.SerializeObject(sendGpriResult);
                }

                if (updateRecord != null)
                {
                    if(status == "GPRI - Complete")
                    {
                        updateRecord.completiontime = _dateTimeHelper.GetDateTimeNow();
                    }
                    updateRecord.status = status;
                    updateRecord.nextstep = next;
                    updateRecord.sendgpriresult = sendGpriResult;
                    updateRecord.modifiedby = user.UserName;
                    updateRecord.modifiedat = _dateTimeHelper.GetDateTimeNow();
                    _appDbContext.gpri.Update(updateRecord);
                    _appDbContext.SaveChanges();
                }

                _logger.LogInformation("GPRI file {fileId} is updated with status {status} and nextstep {next}", fileId, status, next);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "{ex}", ex);
            }
        }

        public virtual async Task SendGPRI(LoggedInUser user, string fileId, string status)
        {
            try
            {
                var result = await _appDbContext.Set<GPRI>()
                            .Join( _appDbContext.Set<GPRIXML>(),
                                g => g.id,
                                gx => gx.gpritableid,
                                (g, gx) => new
                                {
                                    g.id,
                                    g.fileid,
                                    g.paygroupid,
                                    gx.s3objectid,
                                })
                                .Where(x => x.fileid == fileId)
                                .Select(x => new
                                {
                                    x.id,
                                    x.paygroupid,
                                    x.fileid,
                                    x.s3objectid,
                                })
                                .Take(1)
                                .FirstOrDefaultAsync();

                if (result?.s3objectid == null)
                {
                    throw new Exception($"Unable to find Json file for the given file id {fileId}");
                }

                var gpriFile =  await _S3Handling.DownloadFromS3(user, _config.S3SerializedGpriBucketName, result.s3objectid);

                var dayforceGpriRequestModel = new DayforceGpriRequestModel()
                {
                    GpriFilePayload = gpriFile,
                    FileId = result.fileid
                };

                var paygroupDetailsQuery = from p in _appDbContext.paygroup
                                           join l in _appDbContext.legalentity on p.legalentityid equals l.id
                                           join c in _appDbContext.country on p.countryid equals c.id
                                           where p.id == result.paygroupid
                                           select new PaygroupBaseGpriModel
                                           {
                                               PaygroupId = p.id,
                                               OutboundFormat = p.outboundformat ?? "JSON",
                                               GpriSftpFolder = p.GpriSftpFolder,
                                               CountryCode = c.code,
                                               LegalEntityCode = l.code,
                                               PaygroupCode = p.code,
                                               ApiClientId = p.ApiClientId,
                                               EncryptedApiUserName = p.ApiUserName,
                                               EncryptedApiPassword = p.ApiPassword,
                                               UrlPrefix = p.urlPrefix,
                                               IsTestSftp = p.OutboundSftpServer != null && p.OutboundSftpServer.Equals("test", StringComparison.OrdinalIgnoreCase),
                                           };

                var paygroupDetails = await paygroupDetailsQuery.FirstOrDefaultAsync() ?? throw new InvalidDataException($"Unable to generate basic details for given paygroup id {result.paygroupid}");

                var apiResponse = await _dayforceApiClient.PostGpriAsync(dayforceGpriRequestModel, paygroupDetails);
                var sendGpriResult = JsonConvert.SerializeObject(apiResponse);

                if (apiResponse.Status)
                {
                    await UpdateGPRI(user, fileId, Domain.Constants.GPRICOMPLETED.ToString(), Domain.Constants.GPRICOMPLETEDNEXT.ToString(), sendGpriResult);
                }
                else
                {
                    _logger.LogError("GPRI API request failed, {apiResponse}", apiResponse);
                    await UpdateGPRI(user, fileId, Domain.Constants.GPRIFAILED.ToString(), Domain.Constants.GPRIFAILEDNEXT.ToString(), sendGpriResult);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "{ex}", ex);
                await UpdateGPRI(user, fileId, Domain.Constants.GPRIFAILED.ToString(), Domain.Constants.GPRIFAILEDNEXT.ToString(), string.Empty);
            }
        }


        public virtual async Task<List<GPRIModel>> GetGPRI(LoggedInUser user, bool isPayslip, string paygroupCode)
        {
            var gpriList = new List<GPRIModel>();
            try
            {
                var query = (from q in _appDbContext.Set<GPRI>()
                             join y in _appDbContext.Set<PayGroup>() on q.paygroupid equals y.id
                             join p in _appDbContext.Set<GPRIXML>() on q.id equals p.gpritableid into ps_jointable
                             from x in ps_jointable.DefaultIfEmpty()
                             select new GPRIModel()
                             {
                                 id = q.id,
                                 fileid = q.fileid,
                                 paygroupid = q.paygroupid,
                                 paygroup = y.code,
                                 payperiod = q.payperiod,
                                 isoffcycle = q.isoffcycle,
                                 offcycletype = q.offcycletype,
                                 offcyclesuffix = q.offcyclesuffix,
                                 isoverride = q.isoverride,
                                 sheetnumber = q.sheetnumber,
                                 employeerow = Convert.ToString(q.employeerow),
                                 payelementrow = Convert.ToString(q.payelementrow),
                                 payelementcolumn = Convert.ToString(q.payelementcolumn),
                                 employeecolumn = Convert.ToString(q.employeecolumn),
                                 payslipstatus = q.payslipstatus,
                                 noofees = q.noofees,
                                 noofpayslips = q.noofpayslips,
                                 receivedtime = q.receivedtime.HasValue ? _dateTimeHelper.GetDateTimeWithTimezone(q.receivedtime).ToString() : null,
                                 processedtime = q.receivedtime.HasValue ? _dateTimeHelper.GetDateTimeWithTimezone(q.processedtime).ToString() : null,
                                 completiontime = q.completiontime.HasValue ? _dateTimeHelper.GetDateTimeWithTimezone(q.completiontime).ToString() : null,
                                 createdby = q.createdby,
                                 createdat = q.createdat.HasValue ? (q.createdat.Value).ToString() : null,
                                 modifiedby = q.modifiedby,
                                 modifiedat = q.modifiedat.HasValue ? (q.modifiedat.Value).ToString() : null,
                                 status = q.status,
                                 nextstep = q.nextstep == null ? "" : q.nextstep,
                                 year = q.payperiodyear,
                                 sendgpriresult = q.sendgpriresult,
                                 outboundformat = y.outboundformat,
                                 inboundformat = y.inboundformat,
                                 s3objectid = x.s3objectid
                             });

                if (isPayslip)
                {
                    return await query
                                    .Where(e => e.paygroup.ToLower() == paygroupCode.ToLower() && e.status != null && e.status.ToLower() == "approved")
                                    .OrderByDescending(t => t.id).ToListAsync();
                }

                return await query
                                .Where(e => e.paygroup.ToLower() == paygroupCode.ToLower())
                                .OrderByDescending(t => t.id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }

            return gpriList;
        }





        public virtual void DeleteGPRI(LoggedInUser user, int id)
        {
            try
            {
                var gpri = new GPRI();
                var _gpri = _appDbContext.gpri.Where(s => s.id == id).AsNoTracking().FirstOrDefault();

                if (_gpri != null)
                {
                    _gpri.modifiedby = user.UserName;
                    _gpri.modifiedat = _dateTimeHelper.GetDateTimeNow();
                    _gpri.status = "InActive";

                    _appDbContext.gpri.Update(_gpri);
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "{ex}", ex);
            }
        }

        public async Task<string> DownloadGpri(LoggedInUser user, string fileId)
        {
            try
            {
                string s3ObjectKey = await (from g in _appDbContext.gpri
                                      where g.fileid == fileId
                                      join gx in _appDbContext.gprixml on g.id equals gx.gpritableid
                                      select gx.s3objectid).SingleOrDefaultAsync();

                if (s3ObjectKey == null)
                {
                    throw new Exception($"Unable to find Gpri file for the given file id {fileId}");
                }

                _logger.LogInformation("GPRI Json bucket name - {jsonName}", _config.S3SerializedGpriBucketName);

                string url = _S3Handling.GeneratePreSignedUrl(user, _config.S3SerializedGpriBucketName, s3ObjectKey);
                return url;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "{ex}", ex);
                return null;
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
