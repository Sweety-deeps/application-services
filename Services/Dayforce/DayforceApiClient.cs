using Domain.Models;
using Domain.Models.Dayforce;
using Domain.Models.Payslips;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Services.Helpers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Services.Dayforce
{
    public class DayforceApiClient : IDayforceApiClient
	{
		private const string LegacyPostGpriUrlPath = "/GLOBALPayRunImport?isValidateOnly=true";
		private const string PostPayslipUrlPath = "/Documents/Upload";

        private readonly HttpClient _httpClient;
        private readonly LegacyDayforceConfig _legacyDayforceConfig;
        private readonly SftpDayforceConfig _sftpDayforceConfig;
        private readonly TestSftpDayforceConfig _testSftpDayforceConfig;
        private readonly ILogger<DayforceApiClient> _logger;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEncrytionHelper _encrytionHelper;

		public DayforceApiClient(HttpClient httpClient, LegacyDayforceConfig legacyDayforceConfig,
            SftpDayforceConfig sftpDayforceConfig,
            ILogger<DayforceApiClient> logger, IDateTimeHelper dateTimeHelper,
            IEncrytionHelper encrytionHelper, TestSftpDayforceConfig testSftpDayforceConfig)
		{
			_httpClient = httpClient;
            _legacyDayforceConfig = legacyDayforceConfig;
            _sftpDayforceConfig = sftpDayforceConfig;
			_logger = logger;
            _dateTimeHelper = dateTimeHelper;
            _encrytionHelper = encrytionHelper;
            _testSftpDayforceConfig = testSftpDayforceConfig;
		}

		public async Task<BaseResponseModel<string>> PostGpriAsync(DayforceGpriRequestModel requestModel, PaygroupBaseGpriModel paygroupBaseGpriModel)
		{
			try
			{
				if (paygroupBaseGpriModel.OutboundFormat == Domain.Constants.POSTJSON)
				{
                    var userName = _encrytionHelper.Decrypt(paygroupBaseGpriModel.EncryptedApiUserName);
                    var password = _encrytionHelper.Decrypt(paygroupBaseGpriModel.EncryptedApiPassword);

                    if (string.IsNullOrEmpty(paygroupBaseGpriModel.ApiClientId) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                    {
                        return new BaseResponseModel<string>()
                        {
                            Status = false,
                            Message = "Paygroup has invalid API configuration",
                            Errors = new List<string>()
                            {
                                "Paygroup has invalid API configuration, please check the following API Client ID, API user name, API password",
                            },
                        };
                    }

                    return await PostSendGpriJson(requestModel, paygroupBaseGpriModel.ApiClientId, userName, password, paygroupBaseGpriModel.UrlPrefix);
                }
				else if (paygroupBaseGpriModel.OutboundFormat == Domain.Constants.POSTXML)
				{
                    return PostSendGpriXml(requestModel, paygroupBaseGpriModel);
				}
				else
				{
                    return new BaseResponseModel<string>()
					{
                        Status = false,
                        Message = "Invalid Outbound Format",
                        Errors = new List<string>()
						{
                            "Invalid Outbound Format",
                        },
                    };
                }
			}
            catch (Exception ex)
			{
                _logger.LogError("Error while sending GPRI data to Dayforce. {ex}", ex);
                return new BaseResponseModel<string>()
				{
                    Status = false,
                    Message = "Error while sending GPRI data to Dayforce.",
                    Errors = new List<string>()
					{
                        ex.Message,
                    },
                };
            }
		}

		private async Task<BaseResponseModel<string>> PostSendGpriJson(DayforceGpriRequestModel requestModel, string clientId, string userName, string password, string? urlPrefix)
		{
            var payload = new StringContent(requestModel.GpriFilePayload, System.Text.Encoding.UTF8, "application/json");
            var url = GetRequestUrl(_httpClient.BaseAddress, clientId, LegacyPostGpriUrlPath, urlPrefix);
            var result = new BaseResponseModel<string>();

            int maxRedirects = 3;
            while (maxRedirects >= 0)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(userName, password);
                var response = await _httpClient.PostAsync(url, payload);

                _logger.LogInformation("Dayforce - GPRI POST request ({maxRedirects}) responded with status {status}", 3 - maxRedirects, response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    result.Status = true;
                    result.Message = "GPRI data is successfully sent to Dayforce.";
                    result.Data = responseContent;
                    break;
                }
                else if (isRedirectResponse(response.StatusCode))
                {
                    url = response.Headers.Location;
                    maxRedirects--;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    result.Status = false;
                    result.Message = $"GPRI API call failed with status {response.StatusCode}";
                    result.Errors = new List<string>()
                    {
                        responseContent,
                    };
                    break;
                }
            }

            return result;
        }

        private static bool isRedirectResponse(HttpStatusCode statusCode)
        {
            return statusCode == System.Net.HttpStatusCode.Redirect ||
                        statusCode == System.Net.HttpStatusCode.MovedPermanently ||
                        statusCode == System.Net.HttpStatusCode.Found ||
                        statusCode == System.Net.HttpStatusCode.SeeOther ||
                        statusCode == System.Net.HttpStatusCode.TemporaryRedirect;
        }

        private BaseResponseModel<string> PostSendGpriXml(DayforceGpriRequestModel requestModel, PaygroupBaseGpriModel paygroupBaseGpriModel)
		{
            var result = new BaseResponseModel<string>();

            if (string.IsNullOrEmpty(paygroupBaseGpriModel?.GpriSftpFolder))
            {
                result.Status = false;
                result.Message = "GPRI SFTP Folder is blank in paygroup.";

                return result;
            }

            try
            {
                string host = _sftpDayforceConfig.SftpUploadUrl;
                string username = _sftpDayforceConfig.SftpUsername;
                string password = _sftpDayforceConfig.SftpPassword;
                string port = _sftpDayforceConfig.SftpPort;
                _logger.LogInformation("IsTestSftp : {paygroupBaseGpriModel.IsTestSftp}", paygroupBaseGpriModel.IsTestSftp);
                if (paygroupBaseGpriModel.IsTestSftp)
                {
                    host = _testSftpDayforceConfig.SftpUploadUrl;
                    username = _testSftpDayforceConfig.SftpUsername;
                    password = _testSftpDayforceConfig.SftpPassword;
                    port = _testSftpDayforceConfig.SftpPort;
                }
                _logger.LogInformation("host : {host}", host);
                _logger.LogInformation("username : {username}", username);
                _logger.LogInformation("password : {password}", password);
                _logger.LogInformation("port : {port}", port);
                using (var sftp = new SftpClient(host, int.Parse(port), username, password))
                {
                    sftp.Connect();
                    if (sftp.IsConnected)
                    {
                        var file = Encoding.UTF8.GetBytes(requestModel.GpriFilePayload);
                        var fileName = GetGpriXmlFileName(paygroupBaseGpriModel.CountryCode, paygroupBaseGpriModel.LegalEntityCode, paygroupBaseGpriModel.PaygroupCode);
                        var folderPath = $"{paygroupBaseGpriModel.GpriSftpFolder}/";
                        _logger.LogInformation("folderPath : {folderPath}", folderPath);
                        _logger.LogInformation("fileName : {fileName}", fileName);

                        if (sftp.Exists(folderPath))
                        {
                            using (var stream = new MemoryStream(file))
                            {
                                sftp.UploadFile(stream, $"{folderPath}{fileName}");
                            }

                            result.Status = true;
                            result.Message = "GPRI data is successfully sent to Dayforce sftp";
                        }
                        else
                        {
                            result.Status = false;
                            result.Message = $"GPRI SFTP Folder {folderPath} not found in the destination.";
                        }
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
            catch(Exception ex)
            {
                _logger.LogError("Exception occurred while uploading the GPRI to SFTP, {ex}", ex);
                result.Status = false;
                result.Message = "Error while sending GPRI data to Dayforce.";
            }
			return result;
        }
        /* DPI Send Payslip */
        public async Task<BaseResponseModel<PostPayslipResult>> PostPayslipAsync(DayforcePostPayslipListRequest requestModel)
		{
            var postPayslipResult = new PostPayslipResult();
            var result = new BaseResponseModel<PostPayslipResult>()
            {
                Data = postPayslipResult,
                Errors = new List<string>()
            };
            
            try
            {
                var userName = _encrytionHelper.Decrypt(requestModel.EncryptedApiUserName);
                var password = _encrytionHelper.Decrypt(requestModel.EncryptedApiPassword);

                if (string.IsNullOrEmpty(requestModel.ApiClientId) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    return new BaseResponseModel<PostPayslipResult>()
                    {
                        Status = false,
                        Message = "Paygroup has invalid API configuration",
                        Errors = new List<string>()
                            {
                                "Paygroup has invalid API configuration, please check the following API Client ID, API user name, API password",
                            },
                    };
                }

                var url = GetRequestUrl(_httpClient.BaseAddress, requestModel.ApiClientId, PostPayslipUrlPath, requestModel.urlPrefix);
                _logger.LogDebug("[PostPayslipAsync] GetRequestUrl -  {url}", url);

                int maxRedirect = 3;
                while (maxRedirect >= 0)
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, url);
                    using var ms = new MemoryStream();
                    using var content = new MultipartFormDataContent
                    {
                        { new StringContent(requestModel.Metadata), "metadata" },
                    };

                    foreach (var item in requestModel.FileData)
                    {
                        // This resets the position so that it can be copied to another memory stream
                        item.File.Position = 0;
                        item.File.CopyTo(ms);                        
                        ms.Position = 0;
                        content.Add(new StreamContent(ms), item.FileKey, item.FileName);
                    }

                    request.Content = content;

                    _httpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(userName, password);
                    var response = await _httpClient.SendAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        try
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            result.Data = JsonSerializer.Deserialize<PostPayslipResult>(responseContent) ?? new PostPayslipResult();
                            result.Status = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Unable to deserialize response, even though the upload response is success. {ex}", ex);
                            throw new JsonException($"Unable to deserialize response, even though the upload response is success.");
                        }
                    }
                    else if (isRedirectResponse(response.StatusCode))
                    {
                        url = response.Headers.Location;
                        maxRedirect--;
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var message = $"Unable to post the payslip, received error response from Dayforce {response.StatusCode} Content: {responseContent}";
                        _logger.LogError("{message}", message);

                        result.Status = false;
                        result.Errors.Add(message);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                throw;
            }
            finally
            {
                // It disposes the memory stream of file in request model
                await requestModel.DisposeAsync();
            }

			return result;
		}

        // Kept as public so it can be unit tested
        public Uri GetRequestUrl(Uri? baseUrl, string clientId, string actionPath, string? urlPrefix=null)
		{
			if (baseUrl == null || string.IsNullOrEmpty(actionPath))
			{
                throw new ArgumentNullException(nameof(baseUrl), actionPath);
			}
            if (string.IsNullOrEmpty(urlPrefix) == true)
            {
                _logger.LogDebug("[GetRequestUrl] - urlPrefix is Empty");
                return new Uri(baseUrl, $"Api/{clientId}/V1{actionPath}");
            } else
            {
                _logger.LogDebug("[GetRequestUrl] - urlPrefix is not Empty {urlPrefix}", urlPrefix);
                // Insert prefix before the domain
                Uri baseUri = new Uri(baseUrl.ToString());
                string newBaseUrl = $"{baseUri.Scheme}://{urlPrefix}.{baseUri.Host}";
                _logger.LogDebug("[GetRequestUrl] - {newBaseUrl}", newBaseUrl);
                return new Uri($"{newBaseUrl}/Api/{clientId}/V1{actionPath}");
            }
            
        }

        public string GetGpriXmlFileName(string countryCode, string legalEntityCode, string paygroupCode)
        {
            var now = _dateTimeHelper.GetDateTimeNow();
            return $"{countryCode}_{legalEntityCode}_{paygroupCode}_{now:yyyyMMdd}_{now:HHmmss}.xml";
        }
	}
}

