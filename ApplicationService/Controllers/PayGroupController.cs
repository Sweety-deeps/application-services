using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Services.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationService.Controllers
{
    [Route("api/paygroup")]
    [ApiController]
    [Authorize]
    public class PayGroupController : ControllerBase
    {
        private readonly Dictionary<string, IPayGroupServices> _payGroupServices;
        private readonly ILogger<PayGroupController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public PayGroupController(IEnumerable<IPayGroupServices> payGroupServices, ILogger<PayGroupController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _payGroupServices = payGroupServices.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpGet]
        [Route("/getpaygroup/active")]
        public async Task<IActionResult> GetActivePaygroupAsync()
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IPayGroupServices pgservice = _loggedInUserRoleService.GetServiceForController<IPayGroupServices>(loggedInUser, _payGroupServices);

            var response = new ApiResultFormat<List<PaygroupMinimalModel>>();
            var result = await pgservice.GetActivePaygroups(loggedInUser);
            response.data = result;
            response.totalData = result.Count;
            return Ok(response);
        }

        [HttpGet]
        [Route("/getpaygroup")]
        public async Task<IActionResult> GetPayGroup()
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IPayGroupServices pgservice = _loggedInUserRoleService.GetServiceForController<IPayGroupServices>(loggedInUser, _payGroupServices);
            try
            {
                var response = new ApiResultFormat<List<PayGroupModel>>();
                var lstPayGroupModel = new List<PayGroupModel>();
                lstPayGroupModel = await pgservice.GetPayGroupDetails(loggedInUser);
                response.data = lstPayGroupModel;
                response.permissions = new Permissions
                {
                    create = pgservice.CanAdd(loggedInUser),
                    read = pgservice.CanView(loggedInUser),
                    write = pgservice.CanEdit(loggedInUser),
                    delete = pgservice.CanDelete(loggedInUser),
                };
                response.totalData = lstPayGroupModel.Count;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");
            }
        }

        [HttpGet]
        [Route("/getcountry")]
        public string GetCountry()
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IPayGroupServices pgservice = _loggedInUserRoleService.GetServiceForController<IPayGroupServices>(loggedInUser, _payGroupServices);
            string strRespone = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                List<CountryModel> lstCountryModel = new List<CountryModel>();
                lstCountryModel = pgservice.GetCountryDetails(loggedInUser);
                response.data = lstCountryModel.Cast<dynamic>().ToList();
                response.totalData = lstCountryModel.Count;
                strRespone = JsonSerializer.Serialize(response).ToString();

                return strRespone;
            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return strRespone;

        }
        [HttpGet]
        [Route("/getpayfrequency")]
        public string GetPayFrequency()
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IPayGroupServices pgservice = _loggedInUserRoleService.GetServiceForController<IPayGroupServices>(loggedInUser, _payGroupServices);
            string strRespone = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                List<PayFrequencyModel> lstPayFrequencyModel = new List<PayFrequencyModel>();
                lstPayFrequencyModel = pgservice.GetPayFrequencyDetails(loggedInUser);
                response.data = lstPayFrequencyModel.Cast<dynamic>().ToList();
                response.totalData = lstPayFrequencyModel.Count;
                strRespone = JsonSerializer.Serialize(response).ToString();

                return strRespone;
            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return strRespone;

        }
        
        [HttpPost]
        [Route("/addpaygroup")]

        public async Task<IActionResult> InsertPayGroup([FromBody] PayGroupModel _data)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IPayGroupServices pgservice = _loggedInUserRoleService.GetServiceForController<IPayGroupServices>(loggedInUser, _payGroupServices);
            string strRespone = string.Empty;
            DatabaseResponse res = new DatabaseResponse();
            try
            {
                if (_data.id == 0)
                {
                    res = pgservice.AddPayGroup(loggedInUser, _data);
                }
                else
                {
                    res = pgservice.UpdatePayGroupDetails(loggedInUser, _data);
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(res);

        }

        [HttpGet]
        [Route("/deletepaygroup/{id}")]

        public async Task<string> DeletePayGroup(int id)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IPayGroupServices pgservice = _loggedInUserRoleService.GetServiceForController<IPayGroupServices>(loggedInUser, _payGroupServices);
            string strRespone = string.Empty;
            try
            {
                DatabaseResponse databaseResponse = new DatabaseResponse();
                databaseResponse = pgservice.DeletePayGroup(loggedInUser, id);
                return JsonSerializer.Serialize(databaseResponse);
            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return strRespone;
        }

        [HttpPost]
        [Route("collectchangesbatch")]
        public async Task<IActionResult> AddCollectChangeBatch([FromBody] PayGroupModel payGroup)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IPayGroupServices service = _loggedInUserRoleService.GetServiceForController<IPayGroupServices>(loggedInUser, _payGroupServices);

                var isSuccess = await service.LauchCollectChangesAction(loggedInUser, payGroup, Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME"));

                return isSuccess? Ok() : StatusCode(500, "Failed to lauch the job");
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
