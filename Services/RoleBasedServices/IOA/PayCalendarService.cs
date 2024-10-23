using Amazon.CognitoIdentityProvider.Model;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;
using System.Data;

namespace Services.IOA
{
    public class PayCalendarService : Services.IIA.PayCalendarService
    {
        public PayCalendarService(AppDbContext appDbContext, ILogger<PayCalendarService> logger, IEnumerable<IDataImportService> dataImportService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dataImportService, loggedInUserRoleService,dateTimeHelper)
        {
        }
    }
}