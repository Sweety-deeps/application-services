using Domain.Models;
using Domain.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Services.Users;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;

namespace ApplicationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly Dictionary<string, IUserService> _userService;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public UserController(IEnumerable<IUserService> userService, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _userService = userService.ToDictionary(service => service.GetType().Namespace);
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserModel userModel)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IUserService userService = _loggedInUserRoleService.GetServiceForController<IUserService>(loggedInUser, _userService);
            var result = await userService.AddUser(userModel, loggedInUser);
            return Ok(result);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(String userId, [FromBody] UserModel userModel)
        {
            BaseResponseModel<UserResponseModel> result;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IUserService userService = _loggedInUserRoleService.GetServiceForController<IUserService>(loggedInUser, _userService);
                result = await userService.UpdateUser(userId, userModel, loggedInUser);
                if (result.Status)
                {
                    return Ok(result);
                }

                // Todo: refine the response code in a better way
                return BadRequest(result);
            }
            catch (System.UnauthorizedAccessException unauthex)
            {
                result = new BaseResponseModel<UserResponseModel>();
                result.Status = false;
                result.Errors = new List<string>
                {
                    unauthex.Message
                };
                result.Message = unauthex.Message != "" ? unauthex.Message : "You do not have permissions to peform this action.";
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                result = new BaseResponseModel<UserResponseModel>();
                result.Status = false;
                result.Errors = new List<string>
                {
                    ex.Message
                };
                result.Message = "Unable to process this request at the moment, please try again later.";
                return Ok(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserQueryParameters userQueryParameters)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IUserService userService = _loggedInUserRoleService.GetServiceForController<IUserService>(loggedInUser, _userService);
            var userPage = await userService.GetAllUser(userQueryParameters.Limit, userQueryParameters.Offset, loggedInUser);
            userPage.permissions = new Permissions
            {
                create = userService.CanAdd(loggedInUser),
                read = userService.CanView(loggedInUser),
                write = userService.CanEdit(loggedInUser),
                delete = userService.CanDelete(loggedInUser),
            };
            return Ok(userPage);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IUserService userService = _loggedInUserRoleService.GetServiceForController<IUserService>(loggedInUser, _userService);
            var user = await userService.GetUser(id, loggedInUser);
            user.permissions = new Permissions
            {
                create = userService.CanAdd(loggedInUser),
                read = userService.CanView(loggedInUser),
                write = userService.CanEdit(loggedInUser),
                delete = userService.CanDelete(loggedInUser),
            };
            return Ok(user);
        }

        [HttpDelete("/deleteuser/{id}")]
        public async Task<IActionResult> DeleteUserPayGroup(int id)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IUserService userService = _loggedInUserRoleService.GetServiceForController<IUserService>(loggedInUser, _userService);
            var userPage = await userService.DeleteUserPayGroupAsync(id, loggedInUser);

            return Ok(userPage);

        }


        [HttpPost("search")]
        public IActionResult SearchUsers([FromQuery] UserQueryParameters userQueryParameters, [FromBody] Dictionary<string, string> userSearchFilterParams)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IUserService userService = _loggedInUserRoleService.GetServiceForController<IUserService>(loggedInUser, _userService);
            var userPage = userService.SearchUsers(userQueryParameters.Limit, userQueryParameters.Offset, userSearchFilterParams, loggedInUser);
            userPage.permissions = new Permissions
            {
                create = userService.CanAdd(loggedInUser),
                read = userService.CanView(loggedInUser),
                write = userService.CanEdit(loggedInUser),
                delete = userService.CanDelete(loggedInUser),
            };
            return Ok(userPage);
        }

        //public async Task<IActionResult> DeleteUser(int id)
        //{
        //    var loggedInUser = new LoggedInUser();
        //    var result = await userService.DeleteUserAsync(id, loggedInUser);
        //    if (result.Status)
        //    {
        //        return Ok(result);
        //    }

        //    // Todo: refine the response code in a better way
        //    return BadRequest(result);
        //}

        [HttpPut("lastLoggedIn")]
        public async Task<IActionResult> LastLoggedIn()
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IUserService userService = _loggedInUserRoleService.GetServiceForController<IUserService>(loggedInUser, _userService);
            var result = await userService.SetLastLoggedIn(loggedInUser);
            if (result.Status)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
