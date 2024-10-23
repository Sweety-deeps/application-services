using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Domain.Models;
using System.Text.Json;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationService.Controllers
{
    [Route("api/provider")]
    [ApiController]
    [Authorize]
    public class ProviderController : ControllerBase
    {
        public readonly Dictionary<string, IProviderServices> _providerServices;
        private readonly ILogger<PayGroupController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public ProviderController(IEnumerable<IProviderServices> providerServices, ILogger<PayGroupController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _providerServices = providerServices.ToDictionary(service => service.GetType().Namespace); ;
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpGet]
        [Route("/getprovider")]
        public string GetProviderDetails()
        {
            string sResponse = string.Empty;
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IProviderServices providerService = _loggedInUserRoleService.GetServiceForController<IProviderServices>(loggedInUser, _providerServices);
                var res = providerService.GetProviderDetails(loggedInUser);
                response.data = res.Cast<dynamic>().ToList();
                response.permissions = new Permissions
                {
                    create = providerService.CanAdd(loggedInUser),
                    read = providerService.CanView(loggedInUser),
                    write = providerService.CanEdit(loggedInUser),
                    delete = providerService.CanDelete(loggedInUser),
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
        [Route("/addprovider")]
        public async Task<IActionResult> InsertProvider([FromBody] ProviderModel _data)
        {
            string strRespone = string.Empty;
            DatabaseResponse res = new DatabaseResponse();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IProviderServices providerService = _loggedInUserRoleService.GetServiceForController<IProviderServices>(loggedInUser, _providerServices);


                if (_data.id == 0)
                {
                    res = providerService.InsertProviderDetails(loggedInUser, _data);
                }
                else
                {
                    res = providerService.UpdateProviderDetails(loggedInUser, _data);
                }

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return Ok(res);

        }

        [HttpPut]
        [Route("/updateprovider")]
        public async Task<IActionResult> UpdateProvider([FromBody] ProviderModel _data)
        {
            string strRespone = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IProviderServices providerService = _loggedInUserRoleService.GetServiceForController<IProviderServices>(loggedInUser, _providerServices);


            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return Ok(null);

        }

        [HttpGet]
        [Route("/deleteprovider/{id}")]

        public async Task<string> DeleteProvider(int id)
        {
            string strRespone = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IProviderServices providerService = _loggedInUserRoleService.GetServiceForController<IProviderServices>(loggedInUser, _providerServices);
                DatabaseResponse databaseResponse = new DatabaseResponse();
                databaseResponse = providerService.DeleteProvider(loggedInUser, id);

                return JsonSerializer.Serialize(databaseResponse);
            }
            catch (Exception ex)
            {
                //TODO need to do error logging
            }
            return strRespone;
        }
    }
}
