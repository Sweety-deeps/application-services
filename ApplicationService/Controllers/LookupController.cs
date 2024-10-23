using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LookupController : ControllerBase
    {
        private readonly Dictionary<string, ILookupService>  _lookupService;
        private readonly ILogger<PayGroupController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;
        public LookupController(IEnumerable<ILookupService> lookupService, ILogger<PayGroupController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            this._lookupService = lookupService.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpGet]
        [Route("{id}")]
        public List<LookupValue> GetLookupFields(int id, [FromQuery] String? value)
        {
            try
            {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            ILookupService lkpService = _loggedInUserRoleService.GetServiceForController<ILookupService>(loggedInUser, _lookupService);
                return lkpService.GetLookupFields(loggedInUser, id, value);
            }
            catch (Exception ex)
            {
                //TODO need to do error logging
                Console.WriteLine(ex);
            }
            return null;
        }
    }
}
