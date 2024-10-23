using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace ApplicationService.Controllers
{
    [Route("api/[client]")]
    [ApiController]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly Dictionary<string, IClientServices> _clientServices;
        private readonly ILogger<ClientController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public ClientController(IEnumerable<IClientServices> clientServices, ILogger<ClientController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _clientServices = clientServices.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpGet]
        [Route("/getclient")]
        public async Task<IActionResult> Getclient()
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IClientServices clientService = _loggedInUserRoleService.GetServiceForController<IClientServices>(loggedInUser, _clientServices);
            try
            {
                var clients = await clientService.GetClient(loggedInUser);

                var response = new ApiResultFormat<List<ClientModel>>
                {
                    data = clients,
                    permissions = new Permissions
                    {
                        create = clientService.CanAdd(loggedInUser),
                        read = clientService.CanView(loggedInUser),
                        write = clientService.CanEdit(loggedInUser),
                        delete = clientService.CanDelete(loggedInUser),
                    },
                    totalData = clients.Count
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");
            }
        }


        [HttpPost]
        [Route("/addclient")]
        public async Task<IActionResult> InsertClient([FromBody] ClientModel _data)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IClientServices clientService = _loggedInUserRoleService.GetServiceForController<IClientServices>(loggedInUser, _clientServices);
            DatabaseResponse res;
            try
            {
                if (_data.id == 0)
                {
                    res = await clientService.InsertClient(loggedInUser, _data);
                }
                else
                {
                    res = await clientService.UpdateClient(loggedInUser, _data);
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "You do not have permissions to peform this action.");
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");
            }
            return Ok(res);
        }


        [HttpGet]
        [Route("/deleteclient/{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IClientServices clientService = _loggedInUserRoleService.GetServiceForController<IClientServices>(loggedInUser, _clientServices);
            try
            {
                var result = await clientService.DeleteClient(loggedInUser, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");
            }
        }

        [HttpGet]
        [Route("/getpaygroupbyclient/{clientid}")]
        public async Task<IActionResult> GetPayGroupByClient(int clientid)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IClientServices clientService = _loggedInUserRoleService.GetServiceForController<IClientServices>(loggedInUser, _clientServices);
            try
            {
                var paygroupList = await clientService.GetPayGroupByClient(loggedInUser, clientid);

                var response = new ApiResultFormat<List<PaygroupMinimalModel>>
                {
                    data = paygroupList,
                    totalData = paygroupList.Count
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");
            }
        }
    }
}
