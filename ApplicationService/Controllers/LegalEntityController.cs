using System.Text;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Microsoft.AspNetCore.Authorization;


namespace ApplicationService.Controllers
{
    [Route("api/legalentity")]
    [ApiController]
    [Authorize]
    public class LegalEntityController : ControllerBase
    {
        private readonly Dictionary<string, ILegalEntityServices> _legalEntityServices;
        private readonly ILogger<LegalEntityController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public LegalEntityController(IEnumerable<ILegalEntityServices> legalEntityServices, ILogger<LegalEntityController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _legalEntityServices = legalEntityServices.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpGet]
        [Route("/getlegalentity")]
        public async Task<IActionResult> GetLegalEntityDetails()
        {
            var response = new ApiResultFormat<List<LegalEntityModel>>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                ILegalEntityServices leService = _loggedInUserRoleService.GetServiceForController<ILegalEntityServices>(loggedInUser, _legalEntityServices);
                var res = await leService.GetLegalEntityDetails(loggedInUser);
                response.data = res;
                response.totalData = res.Count;
                response.permissions = new Permissions
                {
                    create = leService.CanAdd(loggedInUser),
                    read = leService.CanView(loggedInUser),
                    write = leService.CanEdit(loggedInUser),
                    delete = leService.CanDelete(loggedInUser),
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");
            }
        }

        [HttpGet]
        [Route("/getlegalentitybase")]
        public async Task<IActionResult> GetLegalEntityBaseDetails()
        {
            var response = new ApiResultFormat<dynamic>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                ILegalEntityServices leService = _loggedInUserRoleService.GetServiceForController<ILegalEntityServices>(loggedInUser, _legalEntityServices);
                var res = await leService.GetLegalEntityBaseDetails(loggedInUser);
                response.data = res.Cast<dynamic>().ToList();
                response.totalData = res.Count;
                response.permissions = new Permissions
                {
                    create = leService.CanAdd(loggedInUser),
                    read = leService.CanView(loggedInUser),
                    write = leService.CanEdit(loggedInUser),
                    delete = leService.CanDelete(loggedInUser),
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
        [Route("/addlegalentity")]
        public async Task<IActionResult> InsertLegalEntity([FromBody] LegalEntityModel _data)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                ILegalEntityServices leService = _loggedInUserRoleService.GetServiceForController<ILegalEntityServices>(loggedInUser, _legalEntityServices);
                DatabaseResponse res;
                if (_data.id == 0)
                {
                    res = await leService.InsertLegalEntity(loggedInUser, _data);
                }
                else
                {
                    res = await leService.UpdateLegalEntity(loggedInUser, _data);
                }

                return Ok(res);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "You do not have permissions to peform this action.");
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs");
            }
        }


        [HttpGet]
        [Route("/deletelegalentity/{id}")]
        public async Task<IActionResult> DeleteLegalEntity(int id)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                ILegalEntityServices leService = _loggedInUserRoleService.GetServiceForController<ILegalEntityServices>(loggedInUser, _legalEntityServices);
                var response = await leService.DeleteLegalEntity(loggedInUser, id);

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
