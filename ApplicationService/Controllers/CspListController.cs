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
    public class CspListController : ControllerBase
    {
        private readonly Dictionary<string, ICspListServices> _cspListService;
        private readonly ILogger<CspListController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public CspListController(IEnumerable<ICspListServices> cspListService, ILogger<CspListController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            this._cspListService = cspListService.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpGet]
        [Route("/getcsplistdetails")]
        public async Task<IActionResult> GetCspListDetails()
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            ICspListServices cspService = _loggedInUserRoleService.GetServiceForController<ICspListServices>(loggedInUser, _cspListService);
            try
            {
                var response = new ApiResultFormat<List<CspListModel>>();
                List<CspListModel> lstCspListModel = await cspService.GetCspListDetails(loggedInUser);
                response.data = lstCspListModel;
                response.permissions = new Permissions
                {
                    create = cspService.CanAdd(loggedInUser),
                    read = cspService.CanView(loggedInUser),
                    write = cspService.CanEdit(loggedInUser),
                    delete = cspService.CanDelete(loggedInUser),
                };
                response.totalData = lstCspListModel.Count;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");
            }
        }

        [HttpPost]
        [Route("/csplist")]

        public async Task<IActionResult> InsertCspList([FromBody] CspListModel _data)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            ICspListServices cspService = _loggedInUserRoleService.GetServiceForController<ICspListServices>(loggedInUser, _cspListService);
            string strRespone = string.Empty;
            DatabaseResponse res = new DatabaseResponse();
            try
            {
                res = await cspService.AddOrUpdateCspListAsync(loggedInUser, _data);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");

            }
            return Ok(res);

        }

        [HttpGet]
        [Route("/deletecsplist/{id}")]

        public async Task<IActionResult> DeletecspList(int id)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            ICspListServices cspService = _loggedInUserRoleService.GetServiceForController<ICspListServices>(loggedInUser, _cspListService); string strRespone = string.Empty;
            try
            {
                var result = await cspService.DeleteCspList(loggedInUser, id);
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
