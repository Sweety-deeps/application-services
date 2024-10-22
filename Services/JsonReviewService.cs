using Domain;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class JsonReviewService : IJsonReviewService
    {
        protected readonly AppDbContext _appDbContext;
        protected readonly ILogger<JsonReviewService> _logger;
        protected readonly IS3Handling _s3Handling;
        protected readonly IConfigServices _configServices;
        private readonly IDateTimeHelper _dateTimeHelper;

        public JsonReviewService(AppDbContext appDbContext, ILogger<JsonReviewService> logger, IConfigServices configServices, IS3Handling s3Handling, IDateTimeHelper dateTimeHelper)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _configServices = configServices;
            _s3Handling = s3Handling;
            _dateTimeHelper = dateTimeHelper;
        }
        public virtual async Task<List<JsonReviewModel>> GetJsonReview(LoggedInUser user, string paygroupCode)
        {
            List<JsonReviewModel> res = new List<JsonReviewModel>();

            try
            {
                res = await (from rd in _appDbContext.requestdetails
                             where rd.s3objectid != null && rd.paygroup == paygroupCode
                             join crt in _appDbContext.configrequesttype on rd.requesttypeid equals crt.id
                             join pg in _appDbContext.paygroup on rd.paygroup equals pg.code
                             where pg.code == paygroupCode
                             select new JsonReviewModel
                             {
                                 requestId = rd.id,
                                 requestType = crt.code,
                                 payGroup = rd.paygroup,
                                 processStatus = rd.processstatus,
                                 createdAt = _dateTimeHelper.GetDateTimeWithTimezone(rd.createdat),
                                 modifiedAt = _dateTimeHelper.GetDateTimeWithTimezone(rd.modifiedat),
                                 fileextension = Path.GetExtension(rd.s3objectid),
                                 s3objectid = rd.s3objectid,
                                 processingtime = (rd.modifiedat != null && rd.createdat != null)
                                       ? CommonServiceHelper.DiffInTime((DateTime)rd.createdat, (DateTime)rd.modifiedat): null,
                                 entityname = rd.interfacetype == "XML" ? CommonServiceHelper.ExtractEntityName(rd.additionalinfo) : crt.code,
                                 interfacetype = rd.interfacetype,
                                 success = rd.success,
                                 failure = rd.failure,
                                 warning = rd.warning,

                             }).OrderByDescending(t => t.requestId).ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return res;
        }

        public virtual async Task<BaseResponseModel<string>> DownloadJsonReview(LoggedInUser user, int requestId)
        {
            var response = new BaseResponseModel<string>();
            try
            {
                var requestDetails = await _appDbContext.requestdetails.Where(t => t.id == requestId).FirstOrDefaultAsync();

                if (requestDetails == null)
                {
                    response.Message = "Request not found";
                    return response;
                }

                string? s3Key = requestDetails.s3objectid;
                if (s3Key == null)
                {
                    response.Message = "S3 Key not found";
                    return response;
                }

                var InboundJson_S3BucketName = Environment.GetEnvironmentVariable(Domain.Constants.INBOUND_JSON_BUCKETNAME);

                var json = await _s3Handling.DownloadFromS3(user, InboundJson_S3BucketName, s3Key);
                if (json == null)
                {
                    response.Status = false;
                    response.Message = "Json not found";
                    return response;
                }

                response.Status = true;
                response.Data = json;
                return response;

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
                response.Message = ex.Message;
                return response;
            }
        }

        public virtual bool CanView(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return false;
        }

    }
}
