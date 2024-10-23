using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace ApplicationService.Controllers
{
    [ApiController]
    [Route("api/changelog/")]
    [Authorize]
    public class ChangeLogBatchController : ControllerBase
    {
        private readonly Dictionary<string, IChangeLogBatchService> _services;
        private readonly ILogger<ChangeLogBatchController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public ChangeLogBatchController(IEnumerable<IChangeLogBatchService> services, ILogger<ChangeLogBatchController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _services = services.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpGet]
        [Route("changelogbatch")]
        public async Task<IActionResult> GetChangeLogBatchHistory()
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IChangeLogBatchService service = _loggedInUserRoleService.GetServiceForController<IChangeLogBatchService>(loggedInUser, _services);

                var responseModels = await service.GetAll();

                var response = new ApiResultFormat<List<ChangeLogBatchResponseModel>>
                {
                    data = responseModels,
                    permissions = new Permissions
                    {
                        create = service.CanAdd(loggedInUser),
                        read = service.CanView(loggedInUser),
                        write = service.CanEdit(loggedInUser),
                        delete = service.CanDelete(loggedInUser),
                    },
                    totalData = responseModels.Count
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("changelogbatch")]
        public async Task<IActionResult> AddChangeLogBatch()
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IChangeLogBatchService service = _loggedInUserRoleService.GetServiceForController<IChangeLogBatchService>(loggedInUser, _services);

                var isSuccess = await service.LauchChangeLogBatchProcess(loggedInUser, Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME"));

                return isSuccess ? Ok() : StatusCode(500, "Failed to lauch the job");
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
