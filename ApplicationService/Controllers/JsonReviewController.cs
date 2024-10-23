using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationService.Controllers
{
    [ApiController]
    [Authorize]
    public class JsonReviewController : Controller
    {
        private readonly ILogger<JsonReviewController> _logger;
        private readonly Dictionary<string, IJsonReviewService> _jsonReviewService;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public JsonReviewController(ILogger<JsonReviewController> logger, IEnumerable<IJsonReviewService> jsonReviewService, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _jsonReviewService = jsonReviewService.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpGet]
        [Route("/getjsonreview")]
        public async Task<IActionResult> GetJsonReviewAsync([FromQuery] string paygroupCode)
        {
            var response = new ApiResultFormat<List<JsonReviewModel>>();
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IJsonReviewService jsrService = _loggedInUserRoleService.GetServiceForController<IJsonReviewService>(loggedInUser, _jsonReviewService);
                var jsonReview = await jsrService.GetJsonReview(loggedInUser, paygroupCode);
                response.data = jsonReview;
                response.permissions = new Permissions
                {
                    create = jsrService.CanAdd(loggedInUser),
                    read = jsrService.CanView(loggedInUser),
                    write = jsrService.CanEdit(loggedInUser),
                    delete = jsrService.CanDelete(loggedInUser),
                };
                response.totalData = jsonReview.Count;

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, "Something went wrong, please check the logs.");
            }
        }

        [HttpGet]
        [Route("/downloadjsonreview/{id}")]
        public async Task<IActionResult> DownloadJsonReview(int id)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IJsonReviewService jsrService = _loggedInUserRoleService.GetServiceForController<IJsonReviewService>(loggedInUser, _jsonReviewService);
                var response = await jsrService.DownloadJsonReview(loggedInUser, id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message,
                });
            }
        }
    }
}
