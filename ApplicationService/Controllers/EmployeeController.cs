using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationService.Controllers
{
    [Route("api/employee")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly Dictionary<string, IEmployeeService> _employeeService;
        private readonly ILogger<EmployeeController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public EmployeeController(IEnumerable<IEmployeeService> employeeService, ILogger<EmployeeController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _employeeService = employeeService.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpGet]
        [Route("/getempInfo/{paygroupCode}")]
        public async Task<IActionResult> GetEmployeeInfos(string paygroupCode)
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            var responses = new List<ApiResultFormat<dynamic>>();
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IEmployeeService empService = _loggedInUserRoleService.GetServiceForController<IEmployeeService>(loggedInUser, _employeeService);
            try
            {
                var emps = empService.GetEmployees(loggedInUser, paygroupCode);
                response.data = emps.Cast<dynamic>().ToList();
                response.permissions = new Permissions
                {
                    create = empService.CanAdd(loggedInUser),
                    read = empService.CanView(loggedInUser),
                    write = empService.CanEdit(loggedInUser),
                    delete = empService.CanDelete(loggedInUser),
                };
                response.totalData = emps.Count;
                responses.Add(response);

                var jobs = empService.GetEmpJobs(loggedInUser, paygroupCode);
                response = new ApiResultFormat<dynamic>();
                response.data = jobs.Cast<dynamic>().ToList();
                response.totalData = jobs.Count;
                responses.Add(response);

                var address = empService.GetEmpAddress(loggedInUser, paygroupCode);
                response = new ApiResultFormat<dynamic>();
                response.data = address.Cast<dynamic>().ToList();
                response.totalData = address.Count;
                responses.Add(response);

                var banks = empService.GetEmpBanks(loggedInUser, paygroupCode);
                response = new ApiResultFormat<dynamic>();
                response.data = banks.Cast<dynamic>().ToList();
                response.totalData = banks.Count;
                responses.Add(response);

                var salarys = empService.GetEmpSalarys(loggedInUser, paygroupCode);
                response = new ApiResultFormat<dynamic>();
                response.data = salarys.Cast<dynamic>().ToList();
                response.totalData = salarys.Count;
                responses.Add(response);

                var payds = empService.GetEmpPayDs(loggedInUser, paygroupCode);
                response = new ApiResultFormat<dynamic>();
                response.data = payds.Cast<dynamic>().ToList();
                response.totalData = payds.Count;
                responses.Add(response);

                var csps = await empService.GetEmpCSPs(loggedInUser, paygroupCode);
                response = new ApiResultFormat<dynamic>();
                response.data = csps.Cast<dynamic>().ToList();
                response.totalData = csps.Count;
                responses.Add(response);

                var conf = empService.GetEmpConf(loggedInUser, paygroupCode);
                response = new ApiResultFormat<dynamic>();
                response.data = conf.Cast<dynamic>().ToList();
                response.totalData = conf.Count;
                responses.Add(response);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs");
            }
        }

        [HttpGet]
        [Route("history/{type}/{paygroupId}/{id}")]
        public async Task<IActionResult> GetEmployeeHistoryInfos(string type, int paygroupId, int id)
        {
            var response = new ApiResultFormat<dynamic>();
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IEmployeeService empService = _loggedInUserRoleService.GetServiceForController<IEmployeeService>(loggedInUser, _employeeService);
            
            try
            {
                if (type == "personal")
                {
                    var emps = await empService.GetHistoryEmployees(loggedInUser, paygroupId, id);
                    response.data = emps.Cast<dynamic>().ToList();
                    response.totalData = emps.Count;
                }
                else if (type == "job")
                {
                    var jobs = await empService.GetHistoryEmpJobs(loggedInUser, paygroupId, id);
                    response.data = jobs.Cast<dynamic>().ToList();
                    response.totalData = jobs.Count;
                }
                else if (type == "address")
                {
                    var address = await empService.GetHistoryEmpAddress(loggedInUser, paygroupId, id);
                    response.data = address.Cast<dynamic>().ToList();
                    response.totalData = address.Count;
                }
                else if (type == "bank")
                {
                    var banks = await empService.GetHistoryEmpBanks(loggedInUser, paygroupId, id);
                    response.data = banks.Cast<dynamic>().ToList();
                    response.totalData = banks.Count;
                }
                else if (type == "salary")
                {
                    var salarys = await empService.GetHistoryEmpSalarys(loggedInUser, paygroupId, id);
                    response.data = salarys.Cast<dynamic>().ToList();
                    response.totalData = salarys.Count;
                }
                else if (type == "payd")
                {
                    var payds = await empService.GetHistoryEmpPayDs(loggedInUser, paygroupId, id);
                    response.data = payds.Cast<dynamic>().ToList();
                    response.totalData = payds.Count;
                }
                else if (type == "csp")
                {
                    var csps = await empService.GetHistoryEmpCSPs(loggedInUser, paygroupId, id);
                    response.data = csps.Cast<dynamic>().ToList();
                    response.totalData = csps.Count;
                }
                else if (type == "conf")
                {
                    var conf = await empService.GetHistoryEmpConf(loggedInUser, paygroupId, id);
                    response.data = conf.Cast<dynamic>().ToList(); 
                    response.totalData = conf.Count;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route("/gettimeandattendance/{paygroupCode}")]
        public string GetTimeAndAttendance(string paygroupCode)
        {
            string sResponse;
            var response = new ApiResultFormat<dynamic>();
            var responses = new List<ApiResultFormat<dynamic>>();
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IEmployeeService empService = _loggedInUserRoleService.GetServiceForController<IEmployeeService>(loggedInUser, _employeeService);
            try
            {
                var time = empService.GetTimeAndAttendance(loggedInUser, paygroupCode);
                response = new ApiResultFormat<dynamic>();
                response.data = time.Cast<dynamic>().ToList();
                response.totalData = time.Count;
                responses.Add(response);

                sResponse = JsonSerializer.Serialize(responses).ToString();
                return sResponse;

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return null;

        }
    }
}
