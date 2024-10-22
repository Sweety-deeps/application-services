using Domain.Entities;
using Domain.Models;
using Domain.Models.Dayforce;
using Domain.Models.Payslips;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Renci.SshNet;
using Services.Abstractions;
using Services.Helpers;
using System.Text;

namespace Services.Dayforce
{
    public class DayforceSftpClient : IDayforceSftpClient
    {
        private readonly AppDbContext _appDbContext;
        private readonly SftpDayforceConfig _sftpDayforceConfig;
        private readonly ILogger<DayforceSftpClient> _logger;
        private readonly IDateTimeHelper _dateTimeHelper;

        public DayforceSftpClient(AppDbContext appDbContext, SftpDayforceConfig sftpDayforceConfig, ILogger<DayforceSftpClient> logger,IDateTimeHelper dateTimeHelper)
        {
            _appDbContext = appDbContext;
            _sftpDayforceConfig = sftpDayforceConfig;
            _logger = logger;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<BaseResponseModel<string>> PostGpriSftpAsync(DayforceGpriRequestModel requestModel)
        {
            var result = new BaseResponseModel<string>();

            var sftpRequest = await (from g in _appDbContext.gpri where g.fileid == requestModel.FileId
                                join p in _appDbContext.paygroup on g.paygroupid equals p.id
                                join l in _appDbContext.legalentity on p.legalentityid equals l.id
                                select new DayforcePostSftpRequest
                                {
                                    PaygroupCode = p.code,
                                    LegalentityCode = l.code,
                                    SftpFolder = p.GpriSftpFolder
                                }).SingleOrDefaultAsync();

            if (sftpRequest == null)
            {
                result.Status = false;
                result.Message = "Paygroup not found";
                return result;
            }

            if (sftpRequest.SftpFolder == null)
            {
                result.Status = false;
                result.Message = "Gpri Sftp folder not configured for the paygroup";
                return result;
            }

            result = UploadFileToSftp(sftpRequest, requestModel.GpriFilePayload);      

            return result;
        }

        public async Task<BaseResponseModel<PostPayslipResult>> PostPayslipSftpAsync(DayforcePostPayslipListRequest requestModel)
        {
            var postPayslipResult = new PostPayslipResult();
            var result = new BaseResponseModel<PostPayslipResult>()
            {
                Data = postPayslipResult,
                Errors = new List<string>()
            };

            if (string.IsNullOrEmpty(requestModel.PayslipSftpFolder))
            {
                result.Status = false;
                result.Message = "Gpri Sftp folder not configured for the paygroup";
                return result;
            }

            result = await BulkUploadFileSftp(requestModel.PayslipSftpFolder, requestModel.FileData);

            return result;
        }

        private async Task<BaseResponseModel<PostPayslipResult>> BulkUploadFileSftp(string sftpFolder, IList<DayforcePostPayslipRequest> fileData)
        {
            var result = new BaseResponseModel<PostPayslipResult>();
            var postPaySlipResultList = new List<PostPayslipFilesProcessed>();
            var resData = new PostPayslipResult();

            string host = _sftpDayforceConfig.SftpUploadUrl;
            string username = _sftpDayforceConfig.SftpUsername;
            string password = _sftpDayforceConfig.SftpPassword;
            string port = _sftpDayforceConfig.SftpPort;

            try
            {
                using (var sftp = new SftpClient(host, int.Parse(port), username, password))
                {
                    sftp.Connect();
                    if (sftp.IsConnected)
                    {
                        foreach (var (item, index) in fileData.Select((value, i) => (value, i)))
                        {
                            var dateTimeNow =_dateTimeHelper.GetDateTimeNow();
                            if (!sftp.Exists(sftpFolder))
                            {
                                result.Status = false;
                                result.Message = "Sftp folder does not exist";
                                result.Errors.Add("Sftp folder does not exist");
                                return result;
                            }

                            using var ms = new MemoryStream();
                            item.File.Position = 0;
                            item.File.CopyTo(ms);
                            ms.Position = 0;

                            sftp.UploadFile(ms, $"{sftpFolder}/{item.FileName}");

                            var filesProcessed = new PostPayslipFilesProcessed
                            {
                                Index = index,
                                UploadStatus = "UPLOADED",
                                Message = "File successfully uploaded to SFTP",
                                Status = "Success"
                            };

                            postPaySlipResultList.Add(filesProcessed);
                        }

                        resData.FilesProcessed = postPaySlipResultList;
                        result.Data = resData;
                        result.Status = true;
                        result.Message = "Files data is successfully sent to Dayforce sftp";
                    }
                    else
                    { 
                        result.Status = false;
                        result.Message = "Unable to connect to Sftp";
                        result.Errors.Add("Unable to connect to Sftp");
                    }

                    sftp.Disconnect();
                    sftp.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while sending data to Dayforce sftp.");
                result.Status = false;
                result.Message = ex.Message;
            }
            finally
            {
                foreach(var item in fileData)
                {
                    await item.DisposeAsync();
                }
            }
            return result;
        }

        private BaseResponseModel<string> UploadFileToSftp(DayforcePostSftpRequest sftpRequest, string fileData)
        {
            var result = new BaseResponseModel<string>();

            string host = _sftpDayforceConfig.SftpUploadUrl;
            string username = _sftpDayforceConfig.SftpUsername;
            string password = _sftpDayforceConfig.SftpPassword;
            string port = _sftpDayforceConfig.SftpPort;

            try
            {
                using (var sftp = new SftpClient(host, int.Parse(port), username, password))
                {
                    sftp.Connect();
                    if (sftp.IsConnected)
                    {
                        var file = Encoding.UTF8.GetBytes(fileData);
                        var fileName = $"{sftpRequest.CountryCode}_{sftpRequest.LegalentityCode}_{sftpRequest.PaygroupCode}_{DateTime.Now:yyyyMMdd}_{DateTime.Now:HHmmss}.xml";
                        if (!sftp.Exists(sftpRequest.SftpFolder))
                        {
                            result.Status = false;
                            result.Message = "Sftp folder does not exist";
                            return result;
                        }

                        using (var stream = new MemoryStream(file))
                        {
                            sftp.UploadFile(stream, $"{sftpRequest.SftpFolder}" + $"{fileName}");
                        }

                        result.Status = true;
                        result.Message = "Files data is successfully sent to Dayforce sftp";
                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "Unable to connect to Sftp";
                    }

                    sftp.Disconnect();
                    sftp.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while sending data to Dayforce sftp.");
                result.Status = false;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
