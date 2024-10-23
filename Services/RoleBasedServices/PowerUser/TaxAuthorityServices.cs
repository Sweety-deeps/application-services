using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services.PowerUser
{
    public class TaxAuthorityServices : Services.TaxAuthorityServices
    {
        private readonly ILogger<TaxAuthorityServices> _logger;
        private readonly AppDbContext _appDbContext;

        public TaxAuthorityServices(AppDbContext appDbContext, ILogger<TaxAuthorityServices> logger, IDateTimeHelper dateTimeHelper) : base(appDbContext, logger, dateTimeHelper)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public override DatabaseResponse DeleteTaxAuthority(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public override DatabaseResponse InsertTaxAuthority(LoggedInUser user, TaxAuthorityModel taxAuthorityModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override Task<DatabaseResponse> UpdateTaxAuthority(LoggedInUser user, TaxAuthorityModel taxAuthorityModel)
        {
            throw new UnauthorizedAccessException();
        }

        public override bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public override bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }

    }
}
