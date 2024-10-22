using Amazon.S3;
using Amazon.S3.Model;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Payslips;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Renci.SshNet;
using Services.Abstractions;
using Services.Dayforce;
using Services.Helpers;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Services
{
    public class PayslipsService : IPayslipsService
    {
        protected readonly ILogger<PayslipsService> _logger;
        protected readonly IDayforceApiClient _dayforceApiClient;
        private readonly IDayforceSftpClient _dayforceSftpClient;
        protected readonly AppDbContext _appDbContext;
        protected readonly IAmazonS3 _awsS3Client;
        protected readonly Config _config;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly Dictionary<string, IDataImportService> _dataImportServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;
        private readonly IAmazonS3 _s3Client;
        private readonly string _reportTemplateBucketName = "ultipay-report-template";

        public PayslipsService(AppDbContext appDbContext, IAmazonS3 awsS3Client,
            IDayforceApiClient dayforceApiClient, IDayforceSftpClient dayforceSftpClient,
            ILogger<PayslipsService> logger, Config config, IDateTimeHelper dateTimeHelper,
            ILoggedInUserRoleService loggedInUserRoleService,
            IEnumerable<IDataImportService> dataImportServiceCollection, IAmazonS3 s3client)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _awsS3Client = awsS3Client;
            _dayforceApiClient = dayforceApiClient;
            _dayforceSftpClient = dayforceSftpClient;
            _config = config;
            _dateTimeHelper = dateTimeHelper;
            _loggedInUserRoleService = loggedInUserRoleService;
            _dataImportServices = dataImportServiceCollection.ToDictionary(service => service.GetType().Namespace);
            _s3Client = s3client;
            _reportTemplateBucketName = Environment.GetEnvironmentVariable("UltipayReportTemplateBucket") ?? "ultipay-report-template";
        }

        public virtual async Task<DatabaseResponse> UploadPaySlips(LoggedInUser user, PaySlipsUploadModel model)
        {
            var response = new DatabaseResponse();
            try
            {
                if (model != null)
                {
                    _ = await UpdateGPRITable(user, model.fileid, Domain.Constants.UPLOADPROCESSING, Domain.Constants.UPLOADPROCESSINGNEXT);
                    var additionalData = new { gpriFileId = model.fileid };
                    var additionalJson = JsonSerializer.Serialize(additionalData);
                    var dataImportService = _loggedInUserRoleService.GetServiceForController(user, _dataImportServices);
                    await dataImportService.PublishToDataImportAsync(user, 0, "PayslipUpload", model.paygroup, model.s3objectid, additionalJson, 0, model.filesize);

                    response.status = true;
                    response.message = "Upload Success";
                    return response;
                }
                else
                {
                    var _ress = await UpdateGPRITable(user, model.fileid, Domain.Constants.UPLOADFAILED, Domain.Constants.UPLOADFAILEDNEXT);
                    response.status = false;
                    response.message = "Received empty details";
                    return response;
                }

            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError("Exception occurred in upload payslips {ex}", ex);
                _ = await UpdateGPRITable(user, model.fileid, Domain.Constants.UPLOADFAILED, Domain.Constants.UPLOADFAILEDNEXT);
                response.status = false;
                response.message = ex.Message;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred in upload payslips {ex}", ex);
                _ = await UpdateGPRITable(user, model.fileid, Domain.Constants.UPLOADFAILED, Domain.Constants.UPLOADFAILEDNEXT);
                response.status = false;
                response.message = "Something went wrong, please check with adminitrator";
                return response;
            }
        }

        public virtual async Task<BaseResponseModel<string>> GetPayslipUploadUrl(LoggedInUser user, string paygroup, string fileName)
        {
            var response = new BaseResponseModel<string>()
            {
                Status = true,
                Data = string.Empty,
                Message = "S3 SignedUrl Generated Successfully"
            };
            
            var request = new GetPreSignedUrlRequest()
            {
                Verb = HttpVerb.PUT,
                BucketName = _config.S3TempPayslipBucketName,
                Key = fileName,
                Expires = _dateTimeHelper.GetDateTimeNow().AddMinutes(5),
                ContentType = "application/zip",
            };

            response.Data = _awsS3Client.GetPreSignedURL(request);

            return response;
        }

        public virtual async Task<BaseResponseModel<string>> SendPayslip(LoggedInUser user, string paygroupCode, string fileId)
        {
            var response = new BaseResponseModel<string>()
            {
                Status = true,
                Data = string.Empty,
            };

            try
            {
                await UpdateGPRITable(user, fileId, Domain.Constants.PAYSLIPSPROCESSING.ToString(), Domain.Constants.PAYSLIPSPROCESSINGNEXT.ToString());

                var additionalData = new { gpriFileId = fileId };
                var additionalJson = JsonSerializer.Serialize(additionalData);
                var dataImportService = _loggedInUserRoleService.GetServiceForController(user, _dataImportServices);
                await dataImportService.PublishToDataImportAsync(user, 0, "SendPayslip", paygroupCode, null, additionalJson, 0, 0);

                response.Status = true;
                response.Message = "Send payslip process is initiated.";
                return response;

                /*
                var query = from g in _appDbContext.gpri
                            join p in _appDbContext.paygroup on g.paygroupid equals p.id
                            join l in _appDbContext.legalentity on p.legalentityid equals l.id
                            join c in _appDbContext.country on p.countryid equals c.id
                            where g.fileid == fileId
                            select new
                            {
                                GpriId = g.id,
                                OutboundFormat = p.outboundformat,
                                SftpFolder = p.PayslipSftpFolder,
                                PaygroupCode = p.code,
                                LegalEntityCode = l.code,
                                CountryCode = c.code,
                                p.ApiClientId,
                                p.ApiUserName,
                                p.ApiPassword,
                                p.urlPrefix
                            };

                var payslipResult = await query.FirstOrDefaultAsync() ?? throw new InvalidDataException($"Unable find payslip result for file id, {fileId}");

                var payslipItems = await GetPostPayslipItems(user, fileId);

                var postPayslipResults = new List<PostPayslipFilesProcessed>();
                int i = 0;
                foreach (var item in payslipItems)
                {
                    var postPayslipResult = new PostPayslipFilesProcessed();
                    var metadata = new List<PostPayslipMetadataItem>() { item.PostPayslipMetadataItem };
                    var serialisedMetadata = JsonSerializer.Serialize(metadata);
                    var file = await _awsS3Client.GetObjectAsync(_config.S3PayslipBucketName, item.S3ObjectKey);
                    if (file.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // Todo: any memory leaks?
                        var ms = new MemoryStream();
                        file.ResponseStream.CopyTo(ms);

                        var fileData = new DayforcePostPayslipRequest()
                        {
                            File = ms,
                            FileKey = item.PostPayslipMetadataItem.FileKey,
                            FileName = item.PostPayslipMetadataItem.FileName,
                            S3ObjectKey = item.S3ObjectKey,
                        };
                        
                        var payslipRequest = new DayforcePostPayslipListRequest()
                        {
                            FileData = new List<DayforcePostPayslipRequest>() { fileData },
                            Metadata = serialisedMetadata,
                            FileId = fileId,
                            GpriId = payslipResult.GpriId,
                            CountryCode = payslipResult.CountryCode,
                            LegalEntityCode = payslipResult.LegalEntityCode,
                            PaygroupCode = payslipResult.PaygroupCode,
                            OutboundFormat = payslipResult.OutboundFormat,
                            PayslipSftpFolder = payslipResult.SftpFolder,
                            ApiClientId = payslipResult.ApiClientId,
                            EncryptedApiUserName = payslipResult.ApiUserName,
                            EncryptedApiPassword = payslipResult.ApiPassword,
                            urlPrefix = payslipResult.urlPrefix
                        };

                        var apiResponse = new BaseResponseModel<PostPayslipResult>();

                        if (payslipResult.OutboundFormat == Domain.Constants.POSTJSON)
                        {
                            apiResponse = await _dayforceApiClient.PostPayslipAsync(payslipRequest);
                        }
                        else if (payslipResult.OutboundFormat == Domain.Constants.POSTXML)
                        {
                            apiResponse = await _dayforceSftpClient.PostPayslipSftpAsync(payslipRequest);
                        }

                        if (apiResponse?.Data?.FilesProcessed != null && apiResponse.Data.FilesProcessed.Count > 0)
                        {
                            var fileProcessed = apiResponse.Data.FilesProcessed.First();
                            postPayslipResult.PayslipId = item.PayslipId;
                            postPayslipResult.Status = apiResponse.Data.Status;
                            postPayslipResult.Index = i;
                            postPayslipResult.DocumentGUID = fileProcessed.DocumentGUID;
                            postPayslipResult.UploadStatus = fileProcessed.UploadStatus;
                            postPayslipResult.Message = fileProcessed.Message;
                        }
                        else
                        {
                            postPayslipResult.PayslipId = item.PayslipId;
                            postPayslipResult.Status = "FAILED";
                            postPayslipResult.Index = i;
                            postPayslipResult.DocumentGUID = null;
                            postPayslipResult.UploadStatus = "FAILED";
                            postPayslipResult.Message = apiResponse?.Errors.FirstOrDefault() ?? "Failed";
                        }
                    }
                    else
                    {
                        postPayslipResult.PayslipId = item.PayslipId;
                        postPayslipResult.Status = "FAILED";
                        postPayslipResult.Index = i;
                        postPayslipResult.DocumentGUID = null;
                        postPayslipResult.UploadStatus = "FAILED";
                        postPayslipResult.Message = "Unable to find payslip from storage";
                    }

                    postPayslipResult.Metadata = serialisedMetadata;
                    postPayslipResults.Add(postPayslipResult);
                    i++;
                }

                var status = Domain.Constants.PAYSLIPSCOMPLETE;
                var next = string.Empty;

                if (postPayslipResults.Any(e => e.UploadStatus == "UPLOADED") && postPayslipResults.Any(e => e.UploadStatus == "FAILED"))
                {
                    status = Domain.Constants.PAYSLIPSPARTIAL;
                    next = Domain.Constants.PAYSLIPSPARTIALNEXT;
                }

                foreach (var item in postPayslipResults)
                {
                    // Not at all good, find a way to optimise it
                    var record = _appDbContext.gpripayslip
                                        .Where(p => p.id == item.PayslipId);
                    var updateQuery = await record
                                        .ExecuteUpdateAsync(s =>
                                                            s.SetProperty(g => g.partneruploadstatus, item.UploadStatus)
                                                             .SetProperty(g => g.message, item.Message)
                                                             .SetProperty(g => g.documentid, item.DocumentGUID)
                                                             .SetProperty(g => g.MetadataJson, item.Metadata));
                }

                if (!postPayslipResults.Any(e => e.UploadStatus == "UPLOADED"))
                {
                    var json = JsonSerializer.Serialize(postPayslipResults);
                    _logger.LogError("Post payslip results after failure in partner, {json}", json);

                    throw new Exception($"Payslip upload failed in partner.");
                }
                else if (payslipResult.OutboundFormat == Domain.Constants.POSTXML)
                {
                    _logger.LogError("Files are uploaded to SFTP and metadata will be sent to dayforce in 65 seconds");
                    var dataImportService = _loggedInUserRoleService.GetServiceForController(user, _dataImportServices);
                    await dataImportService.PublishToDataImportAsync(payslipResult.GpriId, Domain.Constants.CONNECTEDPAYPAYSLIP, payslipResult.PaygroupCode, null, null, 65);
                }

                await UpdateGPRITable(user, fileId, status, next);
                response.Status = true;
                response.Data = "Upload success";

                */
            }
            catch (Exception ex)
            {
                await UpdateGPRITable(user, fileId,Domain.Constants.PAYSLIPSFAILED.ToString(), Domain.Constants.PAYSLIPSFAILED.ToString());
                _logger.Log(LogLevel.Error, "{ex}", ex);
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public virtual List<PaySlipsModel> GetPayslipData(LoggedInUser user, string fileID)
        {
            var lstPaySlipsModel = new List<PaySlipsModel>();
            try
            {

                lstPaySlipsModel = (from P in _appDbContext.Set<PaySlips>().Where(t => t.fileid == fileID)

                                    select new PaySlipsModel()
                                    {
                                        id = P.id,
                                        fileid = P.fileid,
                                        legalentityxrefcode = P.legalentityxrefcode,
                                        paygroupxrefcode = P.paygroupxrefcode,
                                        payperiod = P.payperiod,
                                        employeexrefcode = P.employeexrefcode,
                                        filename = P.filename,
                                        paydate = P.paydate,
                                        payperiodstart = P.payperiodstart,
                                        payperiodend = P.payperiodend,
                                        contributetonetpay = P.contributetonetpay,
                                        itemamount = P.itemamount,
                                        requestid = P.requestid,
                                        level1 = P.documentid,
                                        message = P.message,
                                        ftpsstatus = P.partneruploadstatus,
                                        createdat = P.createdat,
                                        createdby = P.createdby,
                                        modifiedat = P.modifiedat,
                                        modifiedby = P.modifiedby,
                                        status = P.status,
                                        queuetimestamp = P.queuetimestamp,
                                        MetadataExists = P.MetadataJson != null,
                                        Context = P.Context,
                                        ApiErrorMessage = P.ApiErrorMessage,
                                    }).OrderByDescending(a => a.id).ToList();

                return lstPaySlipsModel;

            }
            catch (Exception ex)
            {
                return lstPaySlipsModel;
            }
        }

        public virtual List<PaySlipsDownloadModel> DownloadPayslipData(LoggedInUser user, string fileID)
        {
            var lstPaySlipsModel = new List<PaySlipsDownloadModel>();
            try
            {

                lstPaySlipsModel = (from P in _appDbContext.Set<PaySlips>().Where(t => t.fileid == fileID)

                                    select new PaySlipsDownloadModel()
                                    {
                                        id = P.id,
        
                                        FileId = P.fileid,
                                        LegalEntityCode = P.legalentityxrefcode,
                                        PayGroupCode = P.paygroupxrefcode,
                                        PayPeriod = P.payperiod,
                                        EmpId = P.employeexrefcode,
                                        FileName = P.filename,
                                        PayDate = P.paydate.HasValue ? P.paydate.Value.ToString("yyyy-MM-dd") : "",
                                        PeriodStartDate = P.payperiodstart.HasValue ? P.payperiodstart.Value.ToString("yyyy-MM-dd") : "",
                                        PeriodEndDate = P.payperiodend.HasValue ? P.payperiodend.Value.ToString("yyyy-MM-dd") : "",
                                        NetPay = P.contributetonetpay,
                                        GrossPay = P.itemamount,
                                        Message = P.message,
                                        UploadStatus = P.partneruploadstatus,
                                        DocumentID = P.documentid,                                      
                                        Status= P.status,
                                        Queuetimestamp= P.queuetimestamp,
                                       
                                    }).OrderByDescending(a => a.id).ToList();

                return lstPaySlipsModel;

            }
            catch (Exception ex)
            {
                return lstPaySlipsModel;
            }
        }

        public virtual async Task<bool> UpdateGpriPaysllipStatusAndCount(LoggedInUser user, string fileId, string status, string next, List<PayslipFiles>? payslipFiles)
        {
            var record = _appDbContext.gpri.Where(g => g.fileid == fileId);
            int updateResult;
            if (payslipFiles != null && payslipFiles.Count > 0)
            {
                var employeeCount = payslipFiles.Select(e => e.empid).Distinct().Count();

                updateResult = await record.ExecuteUpdateAsync(b =>
                                            b.SetProperty(g => g.payslipstatus, status)
                                             .SetProperty(g => g.nextstep, next)
                                             .SetProperty(g => g.noofees, employeeCount)
                                             .SetProperty(g => g.noofpayslips, payslipFiles.Count())
                                             .SetProperty(g => g.modifiedat, _dateTimeHelper.GetDateTimeNow())
                                             .SetProperty(g => g.modifiedby, user.UserName)
                                        );
            }
            else
            {
                updateResult = await record.ExecuteUpdateAsync(b =>
                                            b.SetProperty(g => g.payslipstatus, status)
                                             .SetProperty(g => g.nextstep, next)
                                             .SetProperty(g => g.modifiedat,_dateTimeHelper.GetDateTimeNow())
                                             .SetProperty(g => g.modifiedby, user.UserName)
                                        );
            }

            await _appDbContext.SaveChangesAsync();

            return updateResult > 0;
        }

        public virtual async Task<bool> UpdateGPRITable(LoggedInUser user, string fileId, string status, string next)
        {
            try
            {
                await UpdateGpriPaysllipStatusAndCount(user, fileId, status, next, null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while update status in gpri table {ex}", ex);
                return false;
            }
            return true;
        }

        public virtual async Task<List<PostPayslipItem>> GetPostPayslipItems(LoggedInUser user, string fileId)
        {
            var items = new List<PostPayslipItem>();

            var payslips = await _appDbContext.gpripayslip.Where(p => p.fileid == fileId).ToListAsync();

            if (payslips == null)
                return items;

            int i = 0;
            foreach (var payslip in payslips)
            {
                var fileKey = $"file{i}";
                var paygroupCode = payslip.paygroupxrefcode ?? string.Empty;
                var periodStartDate = payslip.payperiodstart?.ToString("yyyyMMdd") ?? string.Empty;

                var additionalData = new PostPayslipAdditionalData()
                {
                    PayGroupXRefCode = paygroupCode,
                    LegalEntity = payslip.legalentityxrefcode ?? string.Empty,
                    PayDate = payslip.paydate?.ToString("yyyyMMdd") ?? string.Empty,
                    PeriodStartDate = periodStartDate,
                    PeriodEndDate = payslip.payperiodend?.ToString("yyyyMMdd") ?? string.Empty,
                    NetPay = payslip.contributetonetpay?.ToString() ?? string.Empty,
                    GrossPay = payslip.itemamount?.ToString() ?? string.Empty,
                    Type = "Additional",
                };

                var additionalDataJson = JsonSerializer.Serialize(additionalData);

                var metadataItem = new PostPayslipMetadataItem()
                {
                    FileName = payslip.filename ?? string.Empty,
                    DocumentTypeXRefCode = "DF_DOC_PAY_SLIP",
                    EntityTypeXRefCode = "EMPLOYEE_FILE",
                    Entity = paygroupCode + "|" + periodStartDate,
                    EmployeeXRefCode = payslip.employeexrefcode ?? string.Empty,
                    FileKey = fileKey,
                    AdditionalData = additionalDataJson,
                };

                var postPayslipItem = new PostPayslipItem()
                {
                    PayslipId = payslip.id,
                    S3ObjectKey = payslip.s3objectkey,
                    PostPayslipMetadataItem = metadataItem,
                };

                items.Add(postPayslipItem);

                i++;
            }

            return items;
        }

        public virtual async Task<string> GetGpriPayslipMetadata(LoggedInUser user, string fileId, int gpriPayslipId)
        {
            try
            {
                var result = await _appDbContext.gpripayslip.Where(e => e.id == gpriPayslipId).Select(e => e.MetadataJson).FirstOrDefaultAsync();

                return result ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while retrieving Gpri Payslip metadata for fileId {fileId} and PayslipId {pId}, {ex}", fileId, gpriPayslipId, ex);
                return string.Empty;
            }
        }

        public virtual async Task<string> DownloadPayslips(LoggedInUser user, List<PaySlipsDownloadModel> results, string fileName)
        {
            try
            {
                string base64String;
                DataTable dt = new DataTable();
                dt.Columns.Add("File ID", typeof(string));
                dt.Columns.Add("Legal Entity Code", typeof(string));
                dt.Columns.Add("Pay Group XRefCode", typeof(string));
                dt.Columns.Add("Pay Period", typeof(int));
                dt.Columns.Add("Emp ID", typeof(string));
                dt.Columns.Add("File Name", typeof(string));
                dt.Columns.Add("Pay Date", typeof(string));
                dt.Columns.Add("Period Start Date", typeof(string));
                dt.Columns.Add("Period End Date", typeof(string));
                dt.Columns.Add("Net Pay", typeof(decimal));
                dt.Columns.Add("Gross Pay", typeof(decimal));
                dt.Columns.Add("Message", typeof(string));
                dt.Columns.Add("Upload Status", typeof(string));
                dt.Columns.Add("Document ID", typeof(string));
                dt.Columns.Add("Status", typeof(string));
                dt.Columns.Add("Queue Time Stamp", typeof(string));

                foreach (var item in results)
                {
                    dt.Rows.Add(
                        item.FileId,
                        item.LegalEntityCode,
                        item.PayGroupCode,
                        item.PayPeriod,
                        item.EmpId,
                        item.FileName,
                        item.PayDate,
                        item.PeriodStartDate,
                        item.PeriodEndDate,
                        item.NetPay,
                        item.GrossPay,
                        item.Message,
                        item.UploadStatus,
                        item.DocumentID,
                        item.Status,
                        item.Queuetimestamp
                    );
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

                    responseStream.CopyTo(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Position = 0;

                    using (var wb = new XLWorkbook(memStream))
                    {

                        var sheet = wb.Worksheet(1);
                        sheet.Name = fileName;

                        var tableStartRow = 8;
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
                            base64String = Convert.ToBase64String(ms.ToArray());
                            return base64String;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public virtual async Task<bool> UploadFileToS3(LoggedInUser user, MemoryStream ms, string key)
        {
            try
            {
                var request = new PutObjectRequest()
                {
                    Key = key,
                    InputStream = ms,
                    BucketName = _config.S3PayslipBucketName,
                    StorageClass = S3StorageClass.Standard,
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.None,
                };
                var response = await _awsS3Client.PutObjectAsync(request);
                _logger.LogInformation("Upload payslip to s3 responded with status code {statusCode}", response.HttpStatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError("Upload payslip to s3 failed, {ex}", ex);
                return false;
            }
            return true;
        }

        private async Task<string> GetLegalEntityCodeByPaygroupCode(LoggedInUser user, string paygroupCode)
        {
            var query = from l in _appDbContext.Set<LegalEntity>()
                        join p in _appDbContext.Set<PayGroup>() on l.id equals p.legalentityid
                        where p.code == paygroupCode
                        select l.code;
            var legalEntityCode = await query.FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(legalEntityCode))
                throw new InvalidDataException($"No legal entity found for paygroup code {paygroupCode}");

            return legalEntityCode;
        }

        /// <summary>
        /// For iterating the Datatables and upding the column headers with space
        /// </summary>
        /// <param name="dts"></param>
        /// <returns></returns>
        public DataTable ProcesPeriodChangeDT(LoggedInUser user, DataTable dt)
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
