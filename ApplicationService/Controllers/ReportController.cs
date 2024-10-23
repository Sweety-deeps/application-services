using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using System.Text.Json;
using ApplicationService.Filters;
using Services.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Net;
using Domain.Models.Users;
using DocumentFormat.OpenXml.Drawing;

namespace ApplicationService.Controllers
{
    [Route("api/reports")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly Dictionary<string, IReportServices> _reportServices;
        private readonly Dictionary<string, IPayrollElementServices> _payrollElementServices;
        private readonly ILogger<ReportController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public ReportController(IEnumerable<IReportServices> reportServices, IEnumerable<IPayrollElementServices> payrollElementServices, ILogger<ReportController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper)
        {
            _reportServices = reportServices.ToDictionary(service => service.GetType().Namespace);
            _payrollElementServices = payrollElementServices.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
            _dateTimeHelper = dateTimeHelper;
        }

        [HttpGet]
        [Route("/getcomparisondata/{reqID}")]
        public string GetComparisonData(int iRequestID)
        {
            string sResponse = string.Empty;
            try
            {
                #region Validation
                //DateTime temp;
                //if (!DateTime.TryParse(dRequestStartDate.ToString(), out temp) && dRequestStartDate!=null)
                //{
                //    return "Received Invalid Start Date";
                //}
                //if (!DateTime.TryParse(dRequestEndDate.ToString(), out temp) && dRequestEndDate != null)
                //{
                //    return "Received Invalid End Date";
                //}
                #endregion
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);

                sResponse = reportService.GetComparisonData(loggedInUser, iRequestID);
                return sResponse;
            }
            catch (Exception ex)
            {
                //TODO Need to implement the error logging
                return sResponse;
            }

        }

        [HttpGet]
        //[Route("/requestHighlevel/{dtFrom}/{dtTo}/{payGroup}")]
        [Route("/requestHighlevel")]
        //public string GetRequestHighLevelDetails(DateTime dtFrom, DateTime dtTo,string payGroup)
        public string GetRequestHighLevelDetails()
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var res = reportService.GetRequestHighLevelDetails(loggedInUser);
                response.data = res.Cast<dynamic>().ToList();
                response.totalData = res.Count;
                sResponse = JsonSerializer.Serialize(response).ToString();
                return sResponse;
            }
            catch (Exception ex)
            {
                //TODO Need to implement the error logging
                return sResponse;
            }

        }

        [HttpGet]
        [Route("/getrequestLowlevel")]
        public string GetRequestLowLevelDetails()
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var res = reportService.GetLowLevelDetails(loggedInUser);
                response.data = res.Cast<dynamic>().ToList();
                response.totalData = res.Count;
                sResponse = JsonSerializer.Serialize(response).ToString();
                return sResponse;
            }
            catch (Exception ex)
            {
                //TODO Need to implement the error logging
                return sResponse;
            }

        }

        [HttpGet]
        [Route("/paygroupTotalRecords/{dtFrom}/{dtTo}/{payGroup}")]
        public string GetTotalRecordsForPayGroups(DateTime dtFrom, DateTime dtTo, string payGroup)
        {
            string sResponse = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                //sResponse = reportService.GetRequestHighLevelDetails(dtFrom, dtTo);
                return sResponse;
            }
            catch (Exception ex)
            {
                //TODO Need to implement the error logging
                return sResponse;
            }

        }

        [HttpGet]
        [Route("/paygroupTotalRequests/{dtFrom}/{dtTo}/{payGroup}")]
        public string GetTotalRequestsForPayGroups(DateTime dtFrom, DateTime dtTo, string payGroup)
        {
            string sResponse = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                //sResponse = reportService.GetRequestHighLevelDetails(dtFrom, dtTo);
                return sResponse;
            }
            catch (Exception ex)
            {
                //TODO Need to implement the error logging
                return sResponse;
            }

        }
        [HttpPost]
        [Route("/getstartandendtime")]
        public async Task<IActionResult> GetStartAndEndTimeForPayPeriod([FromBody] StartEndTimeRequestModel requestBody)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var (starttime, endtime) = await reportService.GetStartAndEndTime(loggedInUser, requestBody);
                var response = new
                {
                    StartTime = starttime,
                    EndTime = endtime
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while getting start and end time, {ex}", ex);
                return StatusCode(500, "Error occured while getting start and end time.");
            }
        }

            #region Download Reports

        [HttpPost]
        [Route("/downloadPayPeriodRegister")]
        public async Task<IActionResult> PayPeriodRegisterReportDownloadAsync([FromBody] PayPeriodRegisterRequestModel _data)
        {
            string base64String = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                IPayrollElementServices peService = _loggedInUserRoleService.GetServiceForController<IPayrollElementServices>(loggedInUser, _payrollElementServices);
                int iPayGroupID = reportService.GetPayGroupIDByCode(loggedInUser, _data.paygroup);
                var _payPeriodRegisters = await reportService.GetPayPeriodRegistersAsync(loggedInUser, _data, iPayGroupID);
                var _payPeriodRegistersdetail =await  reportService.GetPayPeriodDetailRegistersAsync(loggedInUser, _data, iPayGroupID);
                var _payRollElements = peService.GetPayrollElements(loggedInUser, _data.paygroup);
                //ExportToExcelHelper _helper = new ExportToExcelHelper();
                //var res = _helper.ApplyPayPeriodRegisterFilter(_data, _payPeriodRegisters);
                var sheetName = _data.paygroup == null ? "All PayGroup" + "_PayPeriodRegister_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") : _data.paygroup + "_PayPeriodRegister_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");
                base64String = reportService.GetPayPeriodRegisters(loggedInUser, _payPeriodRegisters, _payPeriodRegistersdetail, _data, _payRollElements, Constants.PayPeriodRegisterExcelTemplate).Result;
                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = sheetName,
                    Data = base64String
                });

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return null;

        }
        [HttpPost]
        [Route("/downloadhrdatawarehouse")]
        public async Task<IActionResult> HrDatawarehouseDownload([FromBody] HrDatawarehouseRequestModel _data)
        {
            string base64String = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                        var _hrdatawarehouse = reportService.GetHrDatawarehouse(loggedInUser, _data);

                DateTime dtPayPeriodDate = _dateTimeHelper.GetDateTimeNow();
                if (_data.payperiod != null)
                {
                    dtPayPeriodDate = reportService.GetPayPeriodDate(loggedInUser, _data.paygroup, "Period End Date", Convert.ToInt32(_data.payperiod));
                }

                ExportToExcelHelper _helper = new ExportToExcelHelper();
                var res = _helper.ApplyHRDataWarehouseFilter(_data, _hrdatawarehouse, dtPayPeriodDate);
                var sheetName = _data.paygroup == null ? "All PayGroup" + "_DataWarehouse_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") : _data.paygroup + "_HRD_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");
                base64String = await reportService.GetHrDatawarehouse(loggedInUser, res, _data, Constants.hrdatawarehouseExcelTemplate);
                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = sheetName,
                    Data = base64String
                });

            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError("Exception occurred during report generation, {ex}", ex);
                return StatusCode(500, "You do not have permissions to peform this action.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred during report generation, {ex}", ex);
                return StatusCode(500, "Exception occurred");
            }
        }

        [HttpPost]
        [Route("/downloadcspfdatawarehouse")]
        public IActionResult CSPFDatawarehouseDownload([FromBody] CSPFDatawarehouseRequestModel _data)
        {
            string base64String = string.Empty;
            DatabaseResponse Response = new DatabaseResponse();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var _cspfdatawarehouse = reportService.GetCSPFDatawarehouse(loggedInUser, _data);
                ExportToExcelHelper _helper = new ExportToExcelHelper();
                var res = _helper.ApplyCSPFDataWarehouseFilter(_data, _cspfdatawarehouse);
                //base64String = _helper.GetCSPFDatawarehouseReportData(_cspfdatawarehouse, _data);
                var sheetName = _data.paygroup == null ? "All PayGroup" + "_CSPFDataWareHouse_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") : _data.paygroup + "_CSPFDataWareHouse_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");
                base64String = reportService.GetCSPFDatawarehouse(loggedInUser, res, _data, Constants.cspfdatawarehouseExcelTemplate).Result;
                // Return a CreatedResult with the base64-encoded Excel data
                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = sheetName,
                    Data = base64String
                });

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return Ok(Response);

        }
        [HttpPost]
        [Route("/downloadpayddatawarehouse")]
        public IActionResult PAYDDatawarehouseDownload([FromBody] PaydDatawarehouseRequestModel _data)
        {
            string base64String = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var _payddatawarehouse = reportService.GetPAYDDatawarehouse(loggedInUser, _data);
                DateTime dtPayPeriodDate = _dateTimeHelper.GetDateTimeNow();
                if (_data.payperiod != null)
                {
                    dtPayPeriodDate = reportService.GetPayPeriodDate(loggedInUser, _data.paygroup, "Period End Date", Convert.ToInt32(_data.payperiod));
                }
                ExportToExcelHelper _helper = new ExportToExcelHelper();
                var res = _helper.ApplyPaydDataWarehouseFilter(_data, _payddatawarehouse, dtPayPeriodDate);
                var sheetName = _data.paygroup == null ? "All PayGroup" + "_PaydDataWareHouse_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") : _data.paygroup + "_PYD_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");
                base64String = reportService.GetPAYDDatawarehouse(loggedInUser, res, _data, Constants.payddatawarehouseExcelTemplate).Result;
                // Return a CreatedResult with the base64-encoded Excel data
                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = sheetName,
                    Data = base64String
                });

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return null;

        }

        [HttpPost]
        [Route("/downloadperiodchangefilereport")]
        [RejectInvalidPpcReport]
        public async Task<IActionResult> PcFileReport([FromBody] PcFileRequestModel requestModel)
        {
            var response = new BaseResponseModel<string>
            {
                Status = false,
                Errors = new List<string>(),
            };

            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var sheetName = $"{requestModel.PaygroupCode}__PeriodChangesFile_{DateTime.Now:yyyyMMdd_HHmmss}";
                var base64String = await reportService.GetPeriodChangeFileAsync(loggedInUser, requestModel, sheetName);

                string text = base64String switch
                {
                    "" => $"Data not available for Task id 3 in {requestModel.PaygroupCode} paygroup for Previous Payperiod",
                    "prevexists" => $"Data not available for Task id 3 in {requestModel.PaygroupCode} paygroup for {requestModel.Year}-{requestModel.PayPeriod}",
                    null => $"Data not available for Task id 3 in {requestModel.PaygroupCode} paygroup for {requestModel.Year}-{requestModel.PayPeriod} and Previous Payperiod",
                    _ => string.Empty
                };

                return new CreatedResult(string.Empty, new
                {
                    Code = text == string.Empty ? 200 : 400,
                    Status = text == string.Empty,
                    Message = sheetName,
                    Data = text == string.Empty ? base64String : null,
                    text
                });
            }
            catch (UnauthorizedAccessException)
            {
                response.Errors.Add("You do not have permissions to peform this action.");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [Route("/downloadchangefilereport")]
        [RejectInvalidPpcReport]
        public async Task<IActionResult> HybridChangeFileReport([FromBody] PcFileRequestModel requestModel)
        {
            var response = new BaseResponseModel<string>
            {
                Status = false,
                Errors = new List<string>(),
            };

            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var sheetName = $"{requestModel.PaygroupCode}__ChangesFile_{DateTime.Now:yyyyMMdd_HHmmss}";
                var base64String = await reportService.GetHybridChageFileAsync(loggedInUser, requestModel, sheetName);

                //Message text needs to be change for the period change , cename report and also in changes report(hybrid)
                string text = base64String switch
                {
                    "" => $"Data not available for Task id 3 in {requestModel.PaygroupCode} paygroup for Previous Payperiod",
                    "prevexists" => $"Data not available for Task id 3 in {requestModel.PaygroupCode} paygroup for {requestModel.Year}-{requestModel.PayPeriod}",
                    "Previous Pay Period doesnt exist" => "Previous Pay Period doesnt exist",
                    "Current Cutoff date doesnt exist" => "Current Cutoff date doesnt exist",
                    _ => string.Empty
                };

                return new CreatedResult(string.Empty, new
                {
                    Code = text == string.Empty ? 200 : 400,
                    Status = text == string.Empty,
                    Message = sheetName,
                    Data = text == string.Empty ? base64String : null,
                    text
                });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogInformation("UnauthorizedAccessException : You do not have permissions to peform this action");
                response.Errors.Add("You do not have permissions to peform this action.");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception Occured: {ex}", ex);
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [Route("/downloadcenamperiodchangefilereport")]
        [RejectInvalidPpcReport]
        public async Task<IActionResult> CenamPcFileReport([FromBody] PcFileRequestModel requestModel)
        {
            var response = new BaseResponseModel<string>
            {
                Status = false,
                Errors = new List<string>(),
            };

            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var sheetName = $"{requestModel.PaygroupCode}_CenamPeriodChangesFile_{DateTime.Now:yyyyMMdd_HHmmss}";
                var base64String = await reportService.GetCenamPeriodChangeFileAsync(loggedInUser, requestModel, sheetName);

                string text = base64String switch
                {
                    "" => $"Data not available for Task id 3 in {requestModel.PaygroupCode} paygroup for Previous Payperiod",
                    "prevexists" => $"Data not available for Task id 3 in {requestModel.PaygroupCode} paygroup for {requestModel.Year}-{requestModel.PayPeriod}",
                    "Pay Groups country code is neither CRI, DOM, GTM, HND, NIC, PAN, or SLV" => "Pay Groups country code is neither CRI, DOM, GTM, HND, NIC, PAN, or SLV",
                    "Previous Pay Period doesnt exist" => "Previous Pay Period doesnt exist",
                    "Current Cutoff date doesnt exist" => "Current Cutoff date doesnt exist",
                    _ => string.Empty
                };

                return new CreatedResult(string.Empty, new
                {
                    Code = text == string.Empty ? 200 : 400,
                    Status = text == string.Empty,
                    Message = sheetName,
                    Data = text == string.Empty ? base64String : null,
                    text
                });
            }
            catch (UnauthorizedAccessException)
            {
                response.Errors.Add("You do not have permissions to peform this action.");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [Route("/downloadmexicoperiodchangefilereport")]
        [RejectInvalidPpcReport]
        public async Task<IActionResult> MexicoPcFileReport([FromBody] PcFileRequestModel requestModel)
        {
            var response = new BaseResponseModel<string>
            {
                Status = false,
                Errors = new List<string>(),
            };

            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var sheetName = $"{requestModel.PaygroupCode}__MexPeriodChangesFile_{DateTime.Now:yyyyMMdd_HHmmss}";
                var base64String = await reportService.GetMexicoPeriodChangeFileAsync(loggedInUser, requestModel, sheetName);
                string text = base64String switch
                {
                    "" => $"Data not available for Task id 3 in {requestModel.PaygroupCode} paygroup for Previous Payperiod",
                    "prevexists" => $"Data not available for Task id 3 in {requestModel.PaygroupCode} paygroup for {requestModel.Year}-{requestModel.PayPeriod}",
                    "Pay Groups country code is not Mexico" => "Pay Groups country code is not Mexico",
                    "Previous Pay Period doesnt exist" => "Previous Pay Period doesnt exist",
                    "Current Cutoff date doesnt exist" => "Current Cutoff date doesnt exist",
                    _ => string.Empty
                };
                return new CreatedResult(string.Empty, new
                {
                    Code = text == string.Empty ? 200 : 400,
                    Status = text == string.Empty,
                    Message = sheetName,
                    Data = text == string.Empty ? base64String : null,
                    text
                });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogInformation("Unauthorized user exception");
                response.Errors.Add("You do not have permissions to peform this action.");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while generating mexico report:{exMessage}", ex.Message);
                response.Errors.Add(ex.Message);
                return StatusCode(500, response);
            }
        }


        [HttpGet]
        [Route("/getreportservicedata/{paygroup}")]
        public async Task<IActionResult> GetReportsData(string? paygroup)
        {
            var response = new ApiResultFormat<List<ReportServiceDetails>>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);

                var result = await reportService.GetReportServiceDetailsAsync(loggedInUser,paygroup);
                response.data = result;
                response.totalData = result.Count;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);

                return StatusCode(500, "Something went wrong while fetching reports data, please check the logs");
            }
        }

        [HttpPost]
        [Route("/payelementreport")]
        public IActionResult PayElementReport([FromBody] PayElementRequestModel _data)
        {
            string base64String = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);

                var _payelement = reportService.GetPayElementReport(loggedInUser, _data);
                ExportToExcelHelper _helper = new ExportToExcelHelper();
                var res = _helper.ApplyPayElementFilter(_data, _payelement);
                var sheetName = _data.paygroup == null ? "All PayGroup" + "_PayElementsReport_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") : _data.paygroup + "_PayElementsReport_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");
                base64String = reportService.GetPayElementReport(loggedInUser, res, _data, Constants.payelementreportExcelTemplate).Result;

                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = sheetName,
                    Data = base64String
                });

            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(500, "You do not have permissions to peform this action.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Exception occurred while downloading pay element report");
            }
        }

        [HttpPost]
        [Route("/downloadsystemuserreport")]
        public IActionResult SystemUserReport([FromBody] SystemUserRequestModel _data)
        {
            string base64String = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var _systemusers = reportService.GetSystemUserReport(loggedInUser, _data);
                ExportToExcelHelper _helper = new ExportToExcelHelper();
                var sheetName = _data.paygroup == null ? "All PayGroup" + "_SYSU_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") : _data.paygroup + "_SYSU_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");
                base64String = reportService.GetSystemUserReport(loggedInUser, _systemusers, _data, Constants.systemuserreportExcelTemplate).Result;

                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = sheetName,
                    Data = base64String
                });

            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(500, "You do not have permissions to peform this action.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Exception occurred while downloading system user report");
            }
        }

        [HttpPost]
        [Route("/downloadtransactionreport")]
        public IActionResult TransactionByPayGroupReport([FromBody] TransactionRequestModel _data)
        {
            string base64String = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var _transaction = reportService.GetTransactionByPayGroupReport(loggedInUser, _data);
                ExportToExcelHelper _helper = new ExportToExcelHelper();
                var res = _helper.ApplyTransactionByPayGroupFilter(_data, _transaction);
                var sheetName = _data.paygroup == null ? "All PayGroup" + "_TransactionByPayGroupReport_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") : _data.paygroup + "_TransactionByPayGroupReport_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");
                base64String = reportService.GetTransactionByPayGroupReport(loggedInUser, _transaction, _data, Constants.transactionreportbypaygroupExcelTemplate).Result;

                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = sheetName,
                    Data = base64String
                });

            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(500, "You do not have permissions to peform this action.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Exception occurred while downloading transaction report");
            }
        }

        [HttpPost]
        [Route("/downloadtransactioncountryreport")]
        public IActionResult TransactionByCountryReport([FromBody] TransactionCountryRequestModel _data)
        {
            string base64String = string.Empty;
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
            try
            {
                var _transaction = reportService.GetTransactionByCountryReport(loggedInUser, _data);
                ExportToExcelHelper _helper = new ExportToExcelHelper();
                var res = _helper.ApplyTransactionByCountryFilter(_data, _transaction);
                var sheetName = _data.country == null ? "All Country" + "_TransactionByCountryReport_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") : _data.country + "_TransactionByCountryReport_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");
                base64String = reportService.GetTransactionByCountryReport(loggedInUser, _transaction, _data, Constants.transactionreportbycountryExcelTemplate).Result;

                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = sheetName,
                    Data = base64String
                });

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
                reportService.InsertError(loggedInUser, 0, "Application Service", "400", "downloadtransactioncountryreport", ex.ToString());
                return StatusCode(500, "Exception occurred while downloading transaction report");
            }
        }

        [HttpPost]
        [Route("/downloadvariancereport")]
        public IActionResult VarianceReport([FromBody] VarianceRequestModel _data)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var currentVarianceReport = reportService.GetVarianceReport(loggedInUser, _data);
                var previousPayPeriodData = _data;
                int? ipayperiod = _data.payperiod;
                if (currentVarianceReport.Count == 0)
                {
                    return new CreatedResult(string.Empty, new
                    {
                        code = 200,
                        status = false,
                        payPeriod = ipayperiod
                    });
                }
                previousPayPeriodData.paygroup = _data.paygroup;
                int iPayGroupID = reportService.GetPayGroupIDByCode(loggedInUser, _data.paygroup);
                var prev = reportService.GetPayPeriodCutoff(loggedInUser, iPayGroupID, Convert.ToInt32(_data.year), _data.payperiod);
                foreach (var cutoff in prev)
                {
                    previousPayPeriodData.payperiod = cutoff.PreviousPayPeriod;
                    previousPayPeriodData.year = cutoff.PreviousCutOffDate.Value.Year;
                }
                var previousVarianceReport = reportService.GetVarianceReport(loggedInUser, previousPayPeriodData);
                _data.payperiod = ipayperiod;
                var res = currentVarianceReport;
                if (previousVarianceReport.Count > 0)
                {
                    ExportToExcelHelper _helper = new ExportToExcelHelper();
                    res = _helper.ApplyVarianceFilter(previousVarianceReport, currentVarianceReport);
                }
                else
                {
                    ipayperiod = previousPayPeriodData.payperiod;
                    return new CreatedResult(string.Empty, new
                    {
                        code = 400,
                        status = false,
                        payPeriod = ipayperiod
                    });
                }
                var sheetName = _data.paygroup == null ? "All PayGroup" : _data.paygroup;
                sheetName += "_VarianceReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var base64String = reportService.GetVarianceReport(loggedInUser, res, _data, Constants.variancereportExcelTemplate).Result;

                return new CreatedResult(string.Empty, new
                {
                    code = 200,
                    status = true,
                    Message = sheetName,
                    Data = base64String,
                });
            }
            catch (UnauthorizedAccessException unAuthEx)
            {
                // Log the error
                _logger.LogError(unAuthEx, "An error occurred while generating the variance report.");
                return StatusCode(500, "You do not have permissions to peform this action.");
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "An error occurred while generating the variance report.");
                // Return an error response
                return StatusCode(500, "An error occurred while generating the variance report.");
            }
        }


        [HttpPost]
        [Route("/downloaderrorlogreport")]
        public IActionResult ErrorLogReport([FromBody] ErrorLogRequestModel _data)
        {
            string base64String = string.Empty;
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
            //DatabaseResponse Response = new DatabaseResponse();
            try
            {
                var _errorlogreport = reportService.GetErrorLogReport(loggedInUser, _data);
                ExportToExcelHelper _helper = new ExportToExcelHelper();
                var res = _helper.ApplyErrorLogFilter(_data, _errorlogreport);

                //Todo :Report / sheetname should not exceed 31 chars Issue create  Name Generic / common function - temp fix applied for time being

                var sheetName = _data.paygroup == null ? "All PayGroup" + "_ErrorLog_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") : (_data.paygroup.Length < 20 ? _data.paygroup : _data.paygroup.Substring(0, 19).Trim()) + "_EL_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");
                base64String = reportService.GetErrorLogReport(loggedInUser, res, _data, Constants.errorlogreportExcelTemplate).Result;

                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = sheetName,
                    Data = base64String
                });
            }
            catch (Exception ex)
            {
                //TODO need to do error logging
                reportService.InsertError(loggedInUser, 0, "Application Service", "400", "downloaderrorlogreport", ex.ToString());
            }

            return null;

        }
        
        [HttpPost]
        [Route("/downloadcalendarreport")]
        public async Task<IActionResult> CalendarReportDownloadAsync([FromBody] CalendarRequestModel requestModel)
        {
            string base64String = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                base64String = await reportService.GetCalendarReport(loggedInUser, requestModel, Constants.calendarreportExcelTemplate);
                var sheetname = requestModel.paygroup == null ? "All PayGroup" + "_CR_" + DateTime.Now.ToString("yyyyMMdd") : requestModel.paygroup + "_CalendarReport_" + DateTime.Now.ToString("yyyyMMdd");

                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = sheetname,
                    Data = base64String
                });

            }
            catch (Exception ex)
            {
            }
            return null;

        }

        #endregion Download Reports

        [HttpGet]
        [Route("/getpayperiodbypaygroupid/{paygroupID}")]
        public List<int> GetPayPeriodByPayGroupID(int paygroupID)
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var res = reportService.GetPayPeriodsByPayGroupID(loggedInUser, paygroupID);
                response.data = res.Cast<dynamic>().ToList();
                response.totalData = res.Count;
                sResponse = JsonSerializer.Serialize(response).ToString();
                return res;

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return null;
        }

        [HttpGet]
        [Route("/getpayperiodbypaygroupcode/{strPayGroupCode}")]
        public List<int> GetPayPeriodByPayGroupCode(string strPayGroupCode)
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var res = reportService.GetPayPeriodsByPayGroupCode(loggedInUser, strPayGroupCode).OrderBy(t => t).ToList();
                response.data = res.Cast<dynamic>().ToList();
                response.totalData = res.Count;
                sResponse = JsonSerializer.Serialize(response).ToString();
                return res;

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return null;
        }

        [HttpGet]
        [Route("/getpayperiodsbyyear/{strPayGroupCode}/{year}")]
        public List<int> GetPayPeriodsByYear(string strPayGroupCode, int year)
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                int iPayGroupID = 0;
                if (strPayGroupCode != null)
                {
                    iPayGroupID = reportService.GetPayGroupIDByCode(loggedInUser, strPayGroupCode);
                }
                var res = reportService.GetPayPeriodsByYear(loggedInUser, iPayGroupID, year).OrderBy(t => t).Distinct().ToList();
                response.data = res.Cast<dynamic>().ToList();
                response.totalData = res.Count;
                sResponse = JsonSerializer.Serialize(response).ToString();
                return res;

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return null;
        }

        [HttpGet]
        [Route("/getpayperiodbycountrycode/{strCountryCode}")]
        public List<int> GetPayPeriodByCountryCode(string strCountryCode)
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                int? iCountryID = 0;
                if (strCountryCode != null)
                {
                    iCountryID = reportService.GetCountryIDByCode(loggedInUser, strCountryCode);
                }
                var res = reportService.GetPayPeriodsByCountryCode(loggedInUser, iCountryID).OrderBy(t => t).ToList();
                response.data = res.Cast<dynamic>().ToList();
                response.totalData = res.Count;
                sResponse = JsonSerializer.Serialize(response).ToString();
                return res;

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return null;
        }

        [HttpGet]
        [Route("/getcountrybyclientcode/{paygroup}")]
        public string GetCountryByClientCode(string paygroup)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var res = reportService.GetCountryByClientCode(loggedInUser, paygroup);
                return res;

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return null;
        }
        [HttpPost]
        [Route("/downloadconfigreport")]
        public IActionResult ConfigurationReport([FromBody] ConfigurationRequestModel model) // ConfigurationRequestModel
        {
            string base64String = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var configList = reportService.GetConfigurationData(loggedInUser, model);
                var template = reportService.GetTemplateForConfigurationReport(model.configname);
                base64String = reportService.GetConfigurationReportAsync(loggedInUser, configList, model, template).Result;

                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = configList.Result.filename,
                    Data = base64String
                });

            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(500, "You do not have permissions to peform this action.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Exception occurred while downloading system user report");
            }
        }

        [HttpGet]
        [Route("/downloadreportservicedetails/{Id}")]
        public async Task<IActionResult> DownloadReportServiceDetails(int Id)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportServices reportService = _loggedInUserRoleService.GetServiceForController<IReportServices>(loggedInUser, _reportServices);
                var url = await reportService.DownloadReportServiceDetails(loggedInUser, Id);
                return Ok(new { downloadUrl = url }); // Return URL directly
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message,
                });
            }
        }

    }
}
