using Amazon.S3;
using Amazon.S3.Model;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Domain;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Renci.SshNet;
using Services.Abstractions;
using Services.Helpers;
using System.Data;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace Services
{
    public class DataImportService : IDataImportService
    {
        protected readonly ILogger<DataImportService> _logger;
        protected readonly IS3Handling _S3Handling;
        protected readonly ISQSHandling _SQSHandling;
        protected readonly AppDbContext _appDbContext;
        private readonly Config _config;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly HttpClient _httpClient;
        private readonly IAmazonS3 _s3Client;
        private readonly string _reportTemplateBucketName;

        public DataImportService(ILogger<DataImportService> logger, IS3Handling s3Handling,
            ISQSHandling sQSHandling, AppDbContext appDbContext,
            Config config,IDateTimeHelper dateTimeHelper, HttpClient httpClient, IAmazonS3 s3Client)
        {
            _logger = logger;
            _S3Handling = s3Handling;
            _SQSHandling = sQSHandling;
            _appDbContext = appDbContext;
            _config = config;
            _dateTimeHelper = dateTimeHelper;
            _httpClient = httpClient;
            _s3Client = s3Client;
            _reportTemplateBucketName = Environment.GetEnvironmentVariable("UltipayReportTemplateBucket") ?? "ultipay-report-template";
        }

        public virtual async Task<string> DownloadErrorReport(LoggedInUser user, Guid? id)
        {
            string errorDetailKey = await _appDbContext.dataimport.Where(x => x.id == id).Select(x => x.errordetails).FirstOrDefaultAsync() ?? string.Empty;

            if (string.IsNullOrEmpty(errorDetailKey))
            {
                return string.Empty;
            }

            string url = _S3Handling.GeneratePreSignedUrl(user, _config.S3ErrorDetailsBucketName, errorDetailKey);
            return url;
        }
        public virtual async Task<string> DownloadErrorReportByEntityId(LoggedInUser user, int? id)
        {
            string errorDetailKey = await _appDbContext.dataimport.Where(x => x.entityid == id).Select(x => x.errordetails).FirstOrDefaultAsync() ?? string.Empty;

            if (string.IsNullOrEmpty(errorDetailKey))
            {
                return string.Empty;
            }

            string url = _S3Handling.GeneratePreSignedUrl(user, _config.S3ErrorDetailsBucketName, errorDetailKey);
            return url;
        }
        public virtual async Task<string> GetFileIdByEntityIdOrGuid(LoggedInUser user, int? entityId, Guid? guid)
        {
            if (entityId.HasValue)
            {
                return await _appDbContext.gpri
                    .Where(g => g.id == entityId.Value)
                    .Select(g => g.fileid)
                    .FirstOrDefaultAsync() ?? string.Empty;
            }
            else if (guid.HasValue)
            {
                var result = await _appDbContext.dataimport
                    .Where(d => d.id == guid.Value)
                    .Join(_appDbContext.gpri,
                        d => d.entityid,
                        g => g.id,
                        (d, g) => new { g.fileid })
                    .Select(x => x.fileid)
                    .FirstOrDefaultAsync();

                return result ?? string.Empty;
            }
            return string.Empty;
            
        }
        private void PopulateSummaryWorksheet(IXLWorksheet sheet, dynamic detailsQuery)
        {
            if (detailsQuery != null)
            {
                sheet.Cell(8, 6).Value = detailsQuery.PayGroup;
                sheet.Cell(9, 6).Value = detailsQuery.Year;
                sheet.Cell(10, 6).Value = detailsQuery.PayPeriod;
                sheet.Cell(11, 6).Value = detailsQuery.IsOffCycle;
                sheet.Cell(12, 6).Value = detailsQuery.OffCycleSuffix;
                sheet.Cell(13, 6).Value = detailsQuery.ProcessedTime;

                for (int i = 8; i <= 13; i++)
                {
                    sheet.Cell(i, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
            }
        }

        private void PopulateErrorDetailsWorksheet(IXLWorksheet sheet, dynamic errorResults, string entityName)
        {
            const string EmptySheet = "There is no data";

            if (errorResults.Count > 0)
            {
                var dataTable = new DataTable();

                var columns = entityName switch
                {
                    "GPRI" => new[] { "Pay Group", "Status", "Log Type", "Message", "Column", "Row Number", "Error Reported On", "File ID" },
                    _ => new[] { "Pay Group", "Row", "Column", "Log Type", "Message", "Error Reported On" }
                };

                foreach (var column in columns)
                {
                    dataTable.Columns.Add(column);
                }

                foreach (var error in errorResults)
                {
                    var rowValues = entityName == "GPRI"
                        ? new object[] { error.PayGroup, error.Status, error.LogType, error.Message, error.ColumnName, error.RowNumber, error.ErrorReportedOn, error.FileId }
                        : new object[] { error.Paygroup, error.Row, error.Column, error.LogType, error.Message, error.ErrorReportedOn };

                    dataTable.Rows.Add(rowValues);
                }

                var table = sheet.Cell(1, 1).InsertTable(dataTable);
                table.HeadersRow().Style.Font.FontColor = XLColor.Black;
                table.ShowAutoFilter = false;
                table.Row(1).Style.Fill.SetBackgroundColor(XLColor.MediumElectricBlue).Font.SetFontColor(XLColor.White);
                table.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            else
            {
                sheet.Cell(4, 7).Value = EmptySheet.ToUpper();
            }
        }
        public virtual async Task<string> DownloadErrorReportAsync(LoggedInUser user, string? url, ErrorDetailsRequestModel requestModel, string? filename)
        {
            string base64string = "";
            int? entityId = requestModel.entityId;
            try
            {
                var responses = await _httpClient.GetAsync(url);
                responses.EnsureSuccessStatusCode();
                var content = await responses.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new SingleOrArrayConverter<ErrorDetailsResponseModel>());
                var errorDetails = JsonSerializer.Deserialize<List<ErrorDetailsResponseModel>>(content, options);

                dynamic errorResults = null;
                dynamic detailsQuery = null;

                if (requestModel.entityname == "GPRI")
                {
                    if (entityId == null)
                    {
                        entityId = (from d in _appDbContext.dataimport
                                    join g in _appDbContext.gpri on d.entityid equals g.id
                                    where d.id == requestModel.guid
                                    select d.entityid).FirstOrDefault();
                    }

                    detailsQuery = (from g in _appDbContext.gpri
                                    join d in _appDbContext.dataimport on g.id equals d.entityid
                                    where g.id == entityId
                                    select new
                                    {
                                        ProcessedTime = g.processedtime.Value.ToString("HH:mm:ss"),
                                        IsOffCycle = g.isoffcycle,
                                        OffCycleSuffix = g.offcyclesuffix,
                                        PayGroup = d.paygroupcode,
                                        PayPeriod = g.payperiod,
                                        Year = g.payperiodyear,
                                        Status = d.status,
                                        FileId = g.fileid,
                                        CreatedAt = d.createdat
                                    }).FirstOrDefault();

                    errorResults = errorDetails.Select(e => new ErrorDetailsResponseModel
                    {
                        LogType = e.LogType,
                        Message = e.Message,
                        ColumnName = e.ColumnName,
                        RowNumber = e.RowNumber,
                        Status = detailsQuery.Status,
                        FileId = detailsQuery.FileId,
                        PayGroup = detailsQuery.PayGroup,
                        ErrorReportedOn = detailsQuery.CreatedAt
                    }).ToList();
                }
                else if (requestModel.entityname == "PayrollElement" || requestModel.entityname == "PayCalendar" || requestModel.entityname == "SelectListValues" || requestModel.entityname == "PayslipUpload" || requestModel.entityname == "SendPayslip")
                {
                    var createdAt = await _appDbContext.dataimport
                    .Where(d => d.id == requestModel.guid)
                    .Select(d => d.createdat)
                    .FirstOrDefaultAsync();
                    errorResults = errorDetails.Select(e => new
                    {
                        Paygroup = requestModel.paygroupcode,
                        Row = e.RowNumber,
                        Column = e.ColumnName,
                        LogType = e.LogType,
                        Message = e.Message,
                        ErrorReportedOn = createdAt
                    }).ToList();
                }
                else
                {
                    throw new ArgumentException("Invalid entityname");
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
                        if (requestModel.entityname == "GPRI" && detailsQuery != null)
                        {
                            PopulateSummaryWorksheet(wb.Worksheet(1), detailsQuery);
                            PopulateErrorDetailsWorksheet(wb.Worksheet(2), errorResults,requestModel.entityname);
                        }
                        else
                        {
                            PopulateErrorDetailsWorksheet(wb.Worksheet(1), errorResults,requestModel.entityname);
                        }
                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            base64string = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Failed to fetch data from the provided URL. {ex}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred.{ex}", ex);
            }
            return base64string;
        }


        public virtual async Task<List<DataImport>> GetDataImportAsync(LoggedInUser user, string paygroupCode)
        {
            bool isSuperUser = user.Role == Role.superuser;
            var query = _appDbContext.dataimport.AsQueryable();
            if (isSuperUser)
            {
                query = query.Where(x => x.paygroupcode == paygroupCode || x.paygroupcode == null);
            }
            else
            {
                query = query.Where(x => x.paygroupcode == paygroupCode);
            }
            var result = await query
           .OrderByDescending(x => x.createdat)
           .Select(x => new DataImport
           {
               id = x.id,
               entityname = x.entityname,
               s3objectid = x.s3objectid,
               paygroupcode = x.paygroupcode,
               status = x.status,
               errordetails = x.errordetails != null ? "NOT NULL" : "NULL",
               createdat = x.createdat,
               createdby = x.createdby,
               filesize = x.filesize,
           })
           .ToListAsync();
            result.ForEach(x => x.createdat = _dateTimeHelper.GetDateTimeWithTimezone(x.createdat));
            return result;
        }

        public virtual async Task PublishToDataImportAsync(LoggedInUser user, int entityId, string entityName, string paygroupCode, string? s3ObjectId, string? additionalJson, int delay, long filesize)
        {
            var newDataImport = new DataImport()
            {
                entityid = entityId,
                entityname = entityName,
                paygroupcode = paygroupCode,
                status = Domain.Constants.UPLOADQUEUED,
                createdat = _dateTimeHelper.GetDateTimeNow(),
                s3objectid = s3ObjectId,
                AdditionalInfo = additionalJson,
                createdby = user.UserName,
                filesize = filesize
            };
            _appDbContext.dataimport.Add(newDataImport);
            await _appDbContext.SaveChangesAsync();

            _ = await _SQSHandling.PublishMessageToPublisher(newDataImport.id, delay);
        }

        public virtual async Task<DatabaseResponse> UploadDataImport(LoggedInUser user, DataImportRequestModel dataImportRequestModel)
        {
            DatabaseResponse response = new DatabaseResponse();
            try
            {
                string file = dataImportRequestModel.file;
                string entity = dataImportRequestModel.entityName;
                int entityID = dataImportRequestModel.entityID;
                string template = dataImportRequestModel.template;
                string? payGroup = dataImportRequestModel.entityName == "SelectListValues" ? null : dataImportRequestModel.payGroup;

                string s3BucketName = _config.S3DataImportBucketName;

                string s3ObjectKey = string.Empty;
                string timestamp = _dateTimeHelper.GetDateTimeNow().ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

                string file1 = file.Replace("data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,", string.Empty);

                if (entity == "GPRI")
                {
                    s3ObjectKey = $"{entity}_{entityID}_{template}_{timestamp}.xlsx";
                }
                else if (entity == "SelectListValues")
                {
                    s3ObjectKey = $"{entity}_{timestamp}.xlsx";
                }
                else
                {
                    s3ObjectKey = $"{entity}_{payGroup}_{timestamp}.xlsx";
                }

                var s3response = await _S3Handling.UploadToS3(user, s3BucketName, s3ObjectKey, file1);
                if (!s3response.success)
                {
                    response.message = "Error in uploading to S3";
                    response.status = false;
                    return response;
                }

                DataImport newDataImport = new DataImport
                {
                    entityid = entityID,
                    entityname = entity,
                    template = template,
                    s3objectid = s3ObjectKey,
                    paygroupcode = payGroup,
                    status = Domain.Constants.UPLOADQUEUED,
                    createdat = _dateTimeHelper.GetDateTimeNow(),
                    createdby = user.UserName,
                    filesize = s3response.fileSizeBytes,
                };
                _appDbContext.dataimport.Add(newDataImport);
                await _appDbContext.SaveChangesAsync();

                bool isPublished = await _SQSHandling.PublishMessageToPublisher(newDataImport.id, 0);
                if (!isPublished)
                {
                    response.message = "Error in publishing to SQS";
                    response.status = false;
                    return response;
                }

                response.message = "DataImport is successful";
                response.status = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DataImport");
                response.message = ex.Message;
                response.status = false;
                return response;
            }
        }

        public virtual async Task<BaseResponseModel<string>> DownloadDataImport(LoggedInUser user, Guid Id)
        {
            var response = new BaseResponseModel<string>();
            try
            {
                var dataImportDetails = await _appDbContext.dataimport.Where(t => t.id == Id).FirstOrDefaultAsync();

                if (dataImportDetails == null)
                {
                    response.Message = "Request not found";
                    return response;
                }

                string? s3Key = dataImportDetails.s3objectid;
                if (s3Key == null)
                {
                    response.Message = "S3 Key not found";
                    return response;
                }
                
               

                var DataImport_S3BucketName = dataImportDetails.entityname == "PayslipUpload" ? _config.S3TempPayslipBucketName : _config.S3DataImportBucketName;

                var data = await _S3Handling.DownloadDataImportFromS3(user, DataImport_S3BucketName, s3Key);
                string base64String = Convert.ToBase64String(data);
                if (data == null)
                {
                    response.Status = false;    
                    response.Message = "File not found";
                    return response;
                }

                response.Status = true;
                response.Data = base64String;
                return response;

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
                response.Message = ex.Message;
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
