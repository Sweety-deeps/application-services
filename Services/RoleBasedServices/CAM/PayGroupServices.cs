using Amazon.EventBridge;
using Amazon.Lambda.Core;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;
using SQLitePCL;

namespace Services.CAM
{
    public class PayGroupServices : Services.CM.PayGroupServices
    {
        public PayGroupServices(AppDbContext appDbContext, ILogger<PayGroupServices> logger, IDateTimeHelper dateTimeHelper, IEncrytionHelper encrytionHelper) : base(appDbContext, logger, dateTimeHelper, encrytionHelper)
        {
        }
    }
}
