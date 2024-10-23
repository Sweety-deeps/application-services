using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Services.Helpers;

namespace ApplicationService.Controllers
{
    [Route("api/payslip")]
    [ApiController]
    [Authorize]
    public class PayslipController : ControllerBase
    {
        private readonly Dictionary<string, IPayslipsService> _payslipsService;
        private readonly ILogger<PayslipController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public PayslipController(IEnumerable<IPayslipsService> payslipsService, ILogger<PayslipController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper)
        {
            _payslipsService = payslipsService.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
            _dateTimeHelper = dateTimeHelper;
        }

        [HttpPost]
        [Route("/uploadpayslip")]
        public async Task<IActionResult> UploadPaySlips([FromBody] PaySlipsUploadModel model)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                var payslipservice = _loggedInUserRoleService.GetServiceForController(loggedInUser, _payslipsService);
                var response = await payslipservice.UploadPaySlips(loggedInUser, model);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new
                {
                    status = false,
                    message = "You do not have permissions to peform this action.",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message,
                });
            }
        }

        [HttpGet]
        [Route("/payslip/uploadurl/{paygroup}/{fileName}")]
        public async Task<IActionResult> GeneratePayslipUploadUrl(string paygroup, string fileName)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayslipsService payslipservice = _loggedInUserRoleService.GetServiceForController<IPayslipsService>(loggedInUser, _payslipsService);

                var response = await payslipservice.GetPayslipUploadUrl(loggedInUser, paygroup, fileName);

                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred in getting S3 Signed URL {ex}", ex);
                return StatusCode(500, "Error occurred when getting S3 Signed URL");
            }
        }

        [HttpGet]
        [Route("/getpayslipdata/{fileID}")]
        public string GetPaySlipsData(string fileID)
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayslipsService payslipservice = _loggedInUserRoleService.GetServiceForController<IPayslipsService>(loggedInUser, _payslipsService);

                var res = payslipservice.GetPayslipData(loggedInUser, fileID);
                response.data = res.Cast<dynamic>().ToList();
                response.permissions = new Permissions
                {
                    create = payslipservice.CanAdd(loggedInUser),
                    read = payslipservice.CanView(loggedInUser),
                    write = payslipservice.CanEdit(loggedInUser),
                    delete = payslipservice.CanDelete(loggedInUser),
                };
                response.totalData = res.Count;
                sResponse = JsonSerializer.Serialize(response).ToString();
                return sResponse;

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return null;
        }

        [HttpPost]
        [Route("/downloadpayslipdata/{fileID}")]
        public IActionResult DownloadSlipsData(string fileID)
        {
            string base64String = string.Empty;

            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayslipsService payslipservice = _loggedInUserRoleService.GetServiceForController<IPayslipsService>(loggedInUser, _payslipsService);
                var res = payslipservice.DownloadPayslipData(loggedInUser, fileID);
                string message = $"{res[0].PayGroupCode}_{fileID}_{res[0].PayPeriod}_PayslipMatched_{_dateTimeHelper.GetDateTimeNow():yyyyMMddHHmmss}";
                base64String = payslipservice.DownloadPayslips(loggedInUser, res, Constants.payslipmatcheddataExcelTemplate).Result;
                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = message,
                    Data = base64String
                });

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return null;
        }

        [HttpGet]
        [Route("/payslip/metadata/{fileId}/{gpriPayslipId}")]
        public async Task<IActionResult> GetRequestJson(string fileId, int gpriPayslipId)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayslipsService payslipservice = _loggedInUserRoleService.GetServiceForController<IPayslipsService>(loggedInUser, _payslipsService);

                var metadataJson = await payslipservice.GetGpriPayslipMetadata(loggedInUser, fileId, gpriPayslipId);

                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = $"payslipmetadata_{fileId}_{gpriPayslipId}.json",
                    Data = metadataJson
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/payslip/send/{paygroupCode}/{fileid}")]
        public async Task<IActionResult> SendPaySlips(string paygroupCode, string fileid)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayslipsService payslipservice = _loggedInUserRoleService.GetServiceForController<IPayslipsService>(loggedInUser, _payslipsService);

                var result = await payslipservice.SendPayslip(loggedInUser, paygroupCode, fileid);
                return result.Status ? Ok(result) : BadRequest(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError("Failure occurred in Send Payslip, {ex}", ex);
                return StatusCode(500, "You do not have permissions to peform this action.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failure occurred in Send Payslip, {ex}", ex);
                return StatusCode(500, "Error occurred when sending payslips");
            }
        }
    }
}
