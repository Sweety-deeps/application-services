using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Services.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Services;

namespace ApplicationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GenericListController : ControllerBase
    {
        private readonly Dictionary<string, IGenericListServices> _genericListService;
        private readonly ILogger<GenericListController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;
        public GenericListController(IEnumerable<IGenericListServices> genericListService, ILogger<GenericListController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            this._genericListService = genericListService.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpGet]
        [Route("/getgenericlistdetails")]
        public async Task<IActionResult> GetGenericListDetails()
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IGenericListServices glService = _loggedInUserRoleService.GetServiceForController<IGenericListServices>(loggedInUser, _genericListService);
            try
            {
                var response = new ApiResultFormat<List<GenericListModel>>();
                List<GenericListModel> lstGenericListModel = await glService.GetGenericListDetails(loggedInUser);
                response.data = lstGenericListModel;
                response.permissions = new Permissions
                {
                    create = glService.CanAdd(loggedInUser),
                    read = glService.CanView(loggedInUser),
                    write = glService.CanEdit(loggedInUser),
                    delete = glService.CanDelete(loggedInUser),
                };
                response.totalData = lstGenericListModel.Count;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");
            }
        }

        [HttpPost]
        [Route("/genericlist")]

        public async Task<IActionResult> InsertGenericList([FromBody] GenericListModel _data)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IGenericListServices glService = _loggedInUserRoleService.GetServiceForController<IGenericListServices>(loggedInUser, _genericListService);
            string strRespone = string.Empty;
            DatabaseResponse res = new DatabaseResponse();
            try
            {
                res = await glService.AddOrUpdateGenericListAsync(loggedInUser, _data);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");

            }
            return Ok(res);

        }
        
        [HttpGet]
        [Route("/deletegenericlist/{id}")]

        public async Task<IActionResult> DeleteGenericList(int id)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IGenericListServices glService = _loggedInUserRoleService.GetServiceForController<IGenericListServices>(loggedInUser, _genericListService); string strRespone = string.Empty;
            try
            {
                var result = await glService.DeleteGenericList(loggedInUser, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");
            }
        }
    }

}