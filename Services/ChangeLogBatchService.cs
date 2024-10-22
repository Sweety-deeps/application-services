using System.Data.SqlClient;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Persistence;
using Services.Abstractions;
using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using ClosedXML;
using Services.Helpers;

namespace Services
{
    public class ChangeLogBatchService : IChangeLogBatchService
    {
        protected readonly ILogger<ChangeLogBatchService> _logger;
        protected readonly AppDbContext _appDbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDateTimeHelper _dateTimeHelper;

        public ChangeLogBatchService(AppDbContext appDbContext, ILogger<ChangeLogBatchService> logger, IServiceProvider serviceProvider, IDateTimeHelper dateTimeHelper)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _dateTimeHelper = dateTimeHelper;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanView(LoggedInUser user)
        {
            return true;
        }

        public virtual async Task<List<ChangeLogBatchResponseModel>> GetAll()
        {
            try
            {
                var changeLogBatches = (from batch in _appDbContext.batchhistory
                                        where batch.batchtype.EndsWith("_CHANGELOG")
                                        orderby batch.starttime descending
                                        select new ChangeLogBatchResponseModel
                                        {
                                            id = batch.id,
                                            starttime = _dateTimeHelper.GetDateTimeWithTimezone(batch.starttime),
                                            finishtime = _dateTimeHelper.GetDateTimeWithTimezone(batch.finishtime),
                                            status = batch.status,
                                            progress = batch.progress,
                                            errorlog = batch.errorlog,
                                            scheduled = batch.scheduled,
                                            triggerdby = batch.triggerdby,
                                            createdby = batch.createdby,
                                            createdat = _dateTimeHelper.GetDateTimeWithTimezone(batch.createdat),
                                            modifiedby = batch.modifiedby,
                                            modifiedat = _dateTimeHelper.GetDateTimeWithTimezone(batch.modifiedat)
                                        }).Take(10).ToList();

                return changeLogBatches;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<bool> LauchChangeLogBatchProcess(LoggedInUser loggedInUser, String eventFunctionArn)
        {
            var awsBridgeClient = new AmazonEventBridgeClient();
            PutEventsRequest changeLogBatchEvent = new PutEventsRequest
            {
                Entries =
                        {
                            new PutEventsRequestEntry
                            {
                                Source = eventFunctionArn,
                                EventBusName = "default",
                                DetailType = "changelogevent",
                                Time = DateTime.Now,
                                Detail = JsonConvert.SerializeObject(
                                    new
                                    {
                                        Action = "MANUAL_CHANGELOG",
                                        Scheduled = false,
                                        TriggerdBy = loggedInUser.UserName
                                    }
                                )
                            }
                        }
            };

            return (await awsBridgeClient.PutEventsAsync(changeLogBatchEvent)).HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}

