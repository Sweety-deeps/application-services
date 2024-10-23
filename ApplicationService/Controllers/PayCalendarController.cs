using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Services.Abstractions;
using Services;
using Azure;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationService.Controllers
{
    [Route("api/[paycalendar]")]
    [ApiController]
    [Authorize]
    public class PayCalendarController : ControllerBase
    {
        public readonly Dictionary<string, IPayCalendarService> _payCalendarService;
        private readonly ILogger<PayCalendarController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public PayCalendarController(IEnumerable<IPayCalendarService> payCalendarService, ILogger<PayCalendarController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _payCalendarService = payCalendarService.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }


        [HttpPost]
        [Route("/addpaycalendar")]

        public async Task<IActionResult> InsertPayCalendar([FromBody] PayCalendarModel _data)
        {
            DatabaseResponse res = new DatabaseResponse();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayCalendarService payCalService = _loggedInUserRoleService.GetServiceForController<IPayCalendarService>(loggedInUser, _payCalendarService);
                string input = _data.cutoffhours;
                var timeFromInput = DateTime.ParseExact(input, "H:m", null, DateTimeStyles.None);
                _data.cutoffhours = timeFromInput.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);

                if (_data.id == 0)
                {
                    res = payCalService.AddPayCalendar(loggedInUser, _data);
                }
                else
                {
                    res = payCalService.UpdatePayCalendar(loggedInUser, _data);
                }
                return Ok(res);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError("{ex}", ex);
                res.status = false;
                res.message = "You do not have permissions to peform this action.";
                return StatusCode(StatusCodes.Status200OK, res);
            }
            catch (Exception ex)
            {
                //TODO need to do error logging
                _logger.LogError(ex, ex.Message);
                res.status = false;
                res.message = ex.Message;
                return StatusCode(StatusCodes.Status200OK, res);
            }

        }

        [HttpGet]
        [Route("/getpaycalendar/{paygroupCode}")]
        public string GetPayCalendar(string paygroupCode)
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayCalendarService payCalService = _loggedInUserRoleService.GetServiceForController<IPayCalendarService>(loggedInUser, _payCalendarService);
                var res = payCalService.GetPayCalendar(loggedInUser, paygroupCode);
                response.data = res.Cast<dynamic>().ToList();
                response.permissions = new Permissions
                {
                    create = payCalService.CanAdd(loggedInUser),
                    read = payCalService.CanView(loggedInUser),
                    write = payCalService.CanEdit(loggedInUser),
                    delete = payCalService.CanDelete(loggedInUser),
                };
                response.totalData = res.Count;
                sResponse = JsonSerializer.Serialize(response).ToString();
                return sResponse;

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
                _logger.LogError(ex, ex.Message);
                return "Error getting pay calendar";
            }
        }
        [HttpGet]
        [Route("/getpaycalenderyears/{id}")]
        public async Task<IActionResult> GetPayCalenderYears(int id)
        {
            try
            {
                string serviceNamespace = "Services";
                var payCalService = _payCalendarService[serviceNamespace];
                List<int> paygroupyearList = await payCalService.GetPayGroupCalendarYears(id);

                return Ok(paygroupyearList);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");
            }
        }

        [HttpGet]
        [Route("/deletepaycalendar/{id}")]

        public async Task<string> DeletePayCalendar(int id)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayCalendarService payCalService = _loggedInUserRoleService.GetServiceForController<IPayCalendarService>(loggedInUser, _payCalendarService);
                DatabaseResponse databaseResponse = new DatabaseResponse();
                payCalService.DeletePayCalendar(loggedInUser, id);
                return JsonSerializer.Serialize(databaseResponse);
            }
            catch (Exception ex)
            {
                //TODO need to do error logging
                _logger.LogError(ex, ex.Message);
                return "Error deleting pay calendar";
            }
        }

        [HttpPost]
        [Route("/uploadpaycalender")]
        public async Task<IActionResult> UploadPayCalender([FromBody] PayCalendarUploadModal _data)
        {
            DatabaseResponse res = new DatabaseResponse();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayCalendarService payCalService = _loggedInUserRoleService.GetServiceForController<IPayCalendarService>(loggedInUser, _payCalendarService);
                res = await payCalService.UploadPayCalender(loggedInUser, _data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, res);
            }
        }

        [HttpGet]
        [Route("/getpayperiods/{paygroupID}/{year}")]
        public List<int> GetPayPeriods(int paygroupID, int year)
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayCalendarService payCalService = _loggedInUserRoleService.GetServiceForController<IPayCalendarService>(loggedInUser, _payCalendarService);
                var res = payCalService.GetPayPeriodsByYear(loggedInUser, paygroupID, year).OrderBy(t => t).Distinct().ToList();
                response.data = res.Cast<dynamic>().ToList();
                response.totalData = res.Count;
                sResponse = JsonSerializer.Serialize(response).ToString();
                return res;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return null;
        }
    }
}
