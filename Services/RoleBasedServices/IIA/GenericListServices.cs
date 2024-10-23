﻿using Domain.Models.Users;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Helpers;

namespace Services.IIA
{
    public class GenericListServices : Services.PowerUser.GenericListServices
    {
        public GenericListServices(AppDbContext appDbContext, ILogger<GenericListServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {

        }

        public override async Task<List<GenericListModel>> GetGenericListDetails(LoggedInUser loggedInUser)
        {
            return await base.GetGenericListDetails(loggedInUser);
        }
        public override Task<DatabaseResponse> AddOrUpdateGenericListAsync(LoggedInUser loggedInUser, GenericListModel data)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> DeleteGenericList(LoggedInUser loggedInUser, int id)
        {
            return base.DeleteGenericList(loggedInUser, id);
        }

        public override bool CanAdd(LoggedInUser user)
        {
            return false;
        }
        public override bool CanEdit(LoggedInUser user)
        {
            return false;
        }
        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }
    }
}