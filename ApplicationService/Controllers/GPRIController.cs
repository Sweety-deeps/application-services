using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using System.Text.Json;
using System.Data;
using Domain.Entities;
using Services.Helpers;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Domain.Enums;

namespace ApplicationService.Controllers
{
    [Route("api/gpri")]
    [ApiController]
    [Authorize]
    public class GPRIController : ControllerBase
    {
        private readonly Dictionary<string, IGPRIService> _GPRIService;
        private readonly ILogger<GPRIController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public GPRIController(IEnumerable<IGPRIService> GPRIService, ILogger<GPRIController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService)
        {
            _GPRIService = GPRIService.ToDictionary(service => service.GetType().Namespace); ;
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        [HttpPost]
        [Route("/addgpri")]
        public async Task<IActionResult> InsertGPRI([FromBody] GPRIModel _data)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                var gpriSerivce = _loggedInUserRoleService.GetServiceForController<IGPRIService>(loggedInUser, _GPRIService);
                var result = await gpriSerivce.AddGPRI(loggedInUser, _data);
                return result switch
                {
                    StatusTypes.Success => Ok(new { status = StatusTypes.Success, message = "GPRI added successfully." }),
                    StatusTypes.Invalid => Ok(new { status = StatusTypes.Invalid, message = $"The Payroll Results file format does not match the Payroll format {_data.payrollformat} selected in the form" }),
                    StatusTypes.Failure => StatusCode(500, new { status = StatusTypes.Failure, message = "An error occurred while adding GPRI." })
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, new
                {
                    status = false,
                    message = "You do not have permissions to peform this action.",
                });
            }
            catch (Exception ex)
            {
                //TODO need to do error logging
                _logger.LogInformation(ex.Message);
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message,
                });
            }
        }

        [HttpGet]
        [Route("/gpri")]
        public async Task<IActionResult> GetGPRI([FromQuery] bool isPayslip, [FromQuery] string paygroupCode)
        {
            var response = new ApiResultFormat<List<GPRIModel>>();
            try
            {
                if (string.IsNullOrEmpty(paygroupCode))
                {
                    return BadRequest("Request does not have a valid paygroup code");
                }
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IGPRIService gpriSerivce = _loggedInUserRoleService.GetServiceForController<IGPRIService>(loggedInUser, _GPRIService);

                var result = await gpriSerivce.GetGPRI(loggedInUser, isPayslip, paygroupCode);
                response.data = result; 
                response.permissions = new Permissions
                {
                    create = gpriSerivce.CanAdd(loggedInUser),
                    read = gpriSerivce.CanView(loggedInUser),
                    write = gpriSerivce.CanEdit(loggedInUser),
                    delete = gpriSerivce.CanDelete(loggedInUser),
                };
                response.totalData = result.Count;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);

                return StatusCode(500, "Something went wrong, please check the logs");
            }
        }

        [HttpDelete]
        [Route("/deletegpri/{id}")]
        public void DeleteGPRI(int id)
        {
            string sResponse = string.Empty;
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IGPRIService gpriSerivce = _loggedInUserRoleService.GetServiceForController<IGPRIService>(loggedInUser, _GPRIService);

                gpriSerivce.DeleteGPRI(loggedInUser, id);
                //return sResponse;

            }
            catch (Exception ex)
            {
                //TODO need to do error logging
                _logger.LogError(ex.Message);
            }
            //return null;

        }

        [HttpGet]
        [Route("/processgpri/{fileid}/{status}")]
        public async Task<string> ProcessGPRI(string fileid, string status)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IGPRIService gpriSerivce = _loggedInUserRoleService.GetServiceForController<IGPRIService>(loggedInUser, _GPRIService);

                await gpriSerivce.UpdateGPRI(loggedInUser, fileid, status, string.Empty, string.Empty);
                if (status == "sendgpri")
                {
                    await gpriSerivce.SendGPRI(loggedInUser, fileid, status);
                }
            }
            catch (Exception ex)
            { 
                //TODO need to do error logging
                _logger.LogError(ex.Message);
            }
            return null;

        }

        [HttpGet]
        [Route("/downloadgpri/{fileId}")]
        public async Task<IActionResult> DownloadGPRI(string fileId)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IGPRIService gpriSerivce = _loggedInUserRoleService.GetServiceForController<IGPRIService>(loggedInUser, _GPRIService);
                var url = await gpriSerivce.DownloadGpri(loggedInUser, fileId);
                Response.Headers.Add("Location", url);
                return StatusCode((int)HttpStatusCode.Found);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message,
                });
            }
        }
    }
}
