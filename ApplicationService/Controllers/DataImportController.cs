using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities.Users;
using Domain.Models.Users;
using Services.Helpers;
using Domain;

namespace ApplicationService.Controllers
{
    [Route("api/[dataimport]")]
    [ApiController]
    [Authorize]
    public class DataImportController : Controller
    {
        private readonly Dictionary<string, IDataImportService> _dataImportService;
        private readonly ILogger<DataImportController> _logger;
        private readonly ILoggedInUserService _loggedInUserServices;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IReportServiceHelper _reportServiceHelper;


        public DataImportController(IEnumerable<IDataImportService> dataImportService, ILogger<DataImportController> logger, ILoggedInUserService loggedInUserService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper, IReportServiceHelper reportServiceHelper)
        {
            _dataImportService = dataImportService.ToDictionary(service => service.GetType().Namespace);
            _logger = logger;
            _loggedInUserServices = loggedInUserService;
            _loggedInUserRoleService = loggedInUserRoleService;
            _dateTimeHelper = dateTimeHelper;
            _reportServiceHelper = reportServiceHelper;
        }

        [HttpGet]
        [Route("/dataimport")]
        public async Task<IActionResult> GetDataImport([FromQuery] string paygroupCode)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IDataImportService dataImportService = _loggedInUserRoleService.GetServiceForController<IDataImportService>(loggedInUser, _dataImportService);
            try
            {
                var response = new ApiResultFormat<List<DataImport>>();
                if (string.IsNullOrEmpty(paygroupCode))
                {
                    return BadRequest("Request does not have a valid paygroup code ");
                }

                var result = await dataImportService.GetDataImportAsync(loggedInUser, paygroupCode);

                response.data = result;
                response.permissions = new Permissions
                {
                    create = dataImportService.CanAdd(loggedInUser),
                    read = dataImportService.CanView(loggedInUser),
                    write = dataImportService.CanEdit(loggedInUser),
                    delete = dataImportService.CanDelete(loggedInUser),
                };
                response.totalData = result.Count;

                return Ok(response)
;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/dataimport")]
        public async Task<IActionResult> UploadDataImport([FromBody] DataImportUploadModel _data)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IDataImportService dataImportService = _loggedInUserRoleService.GetServiceForController<IDataImportService>(loggedInUser, _dataImportService);
            try
            {
                var dataImportRequestModel = new DataImportRequestModel
                {
                    file = _data.excelfile,
                    entityName = _data.entityname,
                    payGroup = _data.paygroup
                };
                var response = await dataImportService.UploadDataImport(loggedInUser, dataImportRequestModel);
                return Ok(response);
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message,
                });
            }
        }

        [HttpPost]
        [Route("/dataimport/downloaderrorreport")]
        public async Task<IActionResult> DownloadErrorReport([FromBody] ErrorDetailsRequestModel requestModel)
        {
            var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
            IDataImportService dataImportService = _loggedInUserRoleService.GetServiceForController<IDataImportService>(loggedInUser, _dataImportService);

            try
            {

                var url = requestModel.entityId!=null
                    ? await dataImportService.DownloadErrorReportByEntityId(loggedInUser, requestModel.entityId)
                    : await dataImportService.DownloadErrorReport(loggedInUser, requestModel.guid);
                string fileId = await dataImportService.GetFileIdByEntityIdOrGuid(loggedInUser, requestModel.entityId,requestModel.guid);
                var base64string = await dataImportService.DownloadErrorReportAsync(loggedInUser, url, requestModel, requestModel.entityname == "GPRI" ? Services.Helpers.Constants.errordetailsreportExcelTemplate : Services.Helpers.Constants.payrollerrordetailsreportExcelTemplate);

                //var sheetname = requestModel.paygroupcode+"_"+"GPRI-EL_"+fileId+"_"+_dateTimeHelper.GetDateTimeNow();
                var sheetname = _reportServiceHelper.GenerateSheetName(requestModel.paygroupcode, $"{requestModel.entityname}-EL_" + fileId );
                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = sheetname,
                    Data = base64string
                });
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

        [HttpGet]
        [Route("/dataimport/downloaddataimport/{id}")]
        public async Task<IActionResult> DownloadDataImport(Guid id)
        {
            try
            {
                var loggedInUser = _loggedInUserServices.GetLoggedInUser(this.User);
                IDataImportService dataImportService = _loggedInUserRoleService.GetServiceForController<IDataImportService>(loggedInUser, _dataImportService);
                var response = await dataImportService.DownloadDataImport(loggedInUser, id);
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
