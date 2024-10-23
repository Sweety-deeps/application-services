using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Abstractions;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationService.Controllers
{
    [Route("api/[payrollelements]")]
    [ApiController]
    [Authorize]
    public class PayrollElementsController : ControllerBase
    {
        private readonly Dictionary<string, IPayrollElementServices> _payrollElementServices;
        private readonly ILogger<PayGroupController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public PayrollElementsController(IEnumerable<IPayrollElementServices> payrollElementServices, ILogger<PayGroupController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _payrollElementServices = payrollElementServices.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpGet]
        [Route("/getpayrollelements/{paygroupCode}")]
        public string Getpayrollelements(string paygroupCode)
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayrollElementServices peService = _loggedInUserRoleService.GetServiceForController<IPayrollElementServices>(loggedInUser, _payrollElementServices);
                var res = peService.GetPayrollElements(loggedInUser, paygroupCode);
                response.data = res.Cast<dynamic>().ToList();
                response.permissions = new Permissions
                {
                    create = peService.CanAdd(loggedInUser),
                    read = peService.CanView(loggedInUser),
                    write = peService.CanEdit(loggedInUser),
                    delete = peService.CanDelete(loggedInUser),
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
        [Route("/addpayrollelements")]
        public async Task<IActionResult> InsertPayrollElements([FromBody] PayrollElementsModel _data)
        {
            string strRespone = string.Empty;
            DatabaseResponse res = new DatabaseResponse();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayrollElementServices peService = _loggedInUserRoleService.GetServiceForController<IPayrollElementServices>(loggedInUser, _payrollElementServices);
                if (_data.id == 0)
                {
                    res = peService.InsertPayrollElements(loggedInUser, _data);
                }
                else
                {
                    res = peService.UpdatePayrollElements(loggedInUser, _data);
                }

            }

            catch (UnauthorizedAccessException)
            {
                res.status = false;
                res.message = "You do not have permissions to peform this action.";
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
            }
            return Ok(res);

        }

        [HttpGet]
        [Route("/deletepayrollelements/{id}")]
        public async Task<string> DeletePayrollElements(int id)
        {
            string strRespone = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayrollElementServices peService = _loggedInUserRoleService.GetServiceForController<IPayrollElementServices>(loggedInUser, _payrollElementServices);
                DatabaseResponse databaseResponse = new DatabaseResponse();
                //databaseResponse = _clientServices.DeleteClient(id);
                peService.DeletePayrollElements(loggedInUser, id);
                return JsonSerializer.Serialize(databaseResponse);
            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return strRespone;
        }

        [HttpPost]
        [Route("/uploadpayelements")]
        public async Task<IActionResult> UploadPayElements([FromBody] PayCalendarUploadModal _data)
        {
            DatabaseResponse res = new DatabaseResponse();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayrollElementServices peService = _loggedInUserRoleService.GetServiceForController<IPayrollElementServices>(loggedInUser, _payrollElementServices);
                res = await peService.UploadPayElements(loggedInUser, _data);
                return Ok(res);
            }
            catch (UnauthorizedAccessException ex)
            {
                res.status = false;
                res.message = "You do not have permissions to peform this action.";
                return StatusCode(StatusCodes.Status200OK, new
                {
                    status = false,
                    message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.StackTrace.ToString();
                return StatusCode(StatusCodes.Status200OK, new
                {
                    status = false,
                    message = ex.Message,
                });
            }
        }
    }
}
