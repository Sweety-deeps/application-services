using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Services.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationService.Controllers
{
    [Route("api/[taxauthority]")]
    [ApiController]
    [Authorize]
    public class TaxAuthorityController : ControllerBase
    {
        private readonly Dictionary<string, ITaxAuthorityServices> _taxAuthorityServices;
        private readonly ILogger<PayGroupController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;
        public TaxAuthorityController(IEnumerable<ITaxAuthorityServices> taxAuthorityServices, ILogger<PayGroupController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _taxAuthorityServices = taxAuthorityServices.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }


        [HttpGet]
        [Route("/gettaxauthority")]
        public string GetTaxAuthority()
        {
            string strRespone = string.Empty;
            var response = new ApiResultFormat<List<TaxAuthorityModel>>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                ITaxAuthorityServices taService = _loggedInUserRoleService.GetServiceForController<ITaxAuthorityServices>(loggedInUser, _taxAuthorityServices);
                var lstTaxAuthorityModel = taService.GetTaxAuthority(loggedInUser);
                response.data = lstTaxAuthorityModel;
                response.permissions = new Permissions
                {
                    create = taService.CanAdd(loggedInUser),
                    read = taService.CanView(loggedInUser),
                    write = taService.CanEdit(loggedInUser),
                    delete = taService.CanDelete(loggedInUser),
                };
                response.totalData = lstTaxAuthorityModel.Count;
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
        [Route("/addtaxauthority")]
        public async Task<IActionResult> InsertTaxAuthority([FromBody] TaxAuthorityModel _data)
        {
            string strResponse = string.Empty;
            DatabaseResponse res = new DatabaseResponse();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                ITaxAuthorityServices taService = _loggedInUserRoleService.GetServiceForController<ITaxAuthorityServices>(loggedInUser, _taxAuthorityServices);
                if (_data.id == 0)
                {
                    res = taService.InsertTaxAuthority(loggedInUser, _data);
                }
                else
                {
                    res = await taService.UpdateTaxAuthority(loggedInUser, _data);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(500, "You do not have permissions to peform this action.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
            return Ok(res);
        }

        [HttpGet]
        [Route("/deletetaxauthority/{id}")]

        public async Task<string> DeleteTaxAuthority(int id)
        {
            string strRespone = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                ITaxAuthorityServices taService = _loggedInUserRoleService.GetServiceForController<ITaxAuthorityServices>(loggedInUser, _taxAuthorityServices);
                DatabaseResponse databaseResponse = new DatabaseResponse();
                taService.DeleteTaxAuthority(loggedInUser, id);
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
