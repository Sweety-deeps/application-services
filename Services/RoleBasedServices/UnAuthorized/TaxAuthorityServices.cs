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

namespace Services.UnAuthorized
{
    public class TaxAuthorityServices : ITaxAuthorityServices
    {
        public bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public bool CanView(LoggedInUser user)
        {
            return false;
        }

        private readonly ILogger<TaxAuthorityServices> _logger;
        private readonly AppDbContext _appDbContext;

        public TaxAuthorityServices(AppDbContext appDbContext, ILogger<TaxAuthorityServices> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;

        }

        public DatabaseResponse DeleteTaxAuthority(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public List<TaxAuthorityModel> GetTaxAuthority(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public DatabaseResponse InsertTaxAuthority(LoggedInUser user, TaxAuthorityModel taxAuthorityModel)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<DatabaseResponse> UpdateTaxAuthority(LoggedInUser user, TaxAuthorityModel taxAuthorityModel)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
