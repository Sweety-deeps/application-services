using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Services.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationService.Controllers
{
    [Route("downloaddeltareport")]
    [ApiController]
    [Authorize]
    public class ReportDeltaController : ControllerBase
    {
        private readonly Dictionary<string, IReportDeltaService> _reportDeltaServices;
        private readonly Dictionary<string, IPayrollElementServices> _payrollElementServices;
        private readonly ILogger<ReportDeltaController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public ReportDeltaController(IEnumerable<IReportDeltaService> reportDeltaServices, IEnumerable<IPayrollElementServices> payrollElementServices, ILogger<ReportDeltaController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _reportDeltaServices = reportDeltaServices.ToDictionary(service => service.GetType().Namespace);
            _payrollElementServices = payrollElementServices.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpPost]
        public IActionResult DeltaReportDownload([FromBody] DeltaRequestModel _data)
        {
            string base64String = string.Empty;
            DatabaseResponse Response = new DatabaseResponse();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IReportDeltaService reportDeltaService = _loggedInUserRoleService.GetServiceForController<IReportDeltaService>(loggedInUser, _reportDeltaServices);
                AdjustDates(_data);
                var _deltareport = reportDeltaService.GetDeltaReport(loggedInUser, _data);
                ExportToExcelHelper _helper = new ExportToExcelHelper();
                var res = _helper.ApplyDeltaReport(_data, _deltareport);
                var sheetName = _data.paygroup + "_DeltaReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                base64String = reportDeltaService.GetDeltaReport(loggedInUser, _deltareport, _data, Constants.deltareportExcelTemplate).Result;
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
        private void AdjustDates(DeltaRequestModel _data)
        {
            if (_data.startdate.HasValue)
            {
                _data.startdate = _data.startdate.Value;
            }

            if (_data.enddate.HasValue)
            {
                _data.enddate = _data.enddate.Value;
            }
        }
    }
}
