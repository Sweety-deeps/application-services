using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace Services
{
    public class CspListServices : ICspListServices
    {
        protected readonly ILogger<CspListServices> _logger;
        protected readonly AppDbContext _appDbContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        public CspListServices(AppDbContext appDbContext, ILogger<CspListServices> logger, IDateTimeHelper dateTimeHelper)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _dateTimeHelper = dateTimeHelper;
        }

        public virtual async Task<List<CspListModel>> GetCspListDetails(LoggedInUser user)
        {
            var lstcspListservice = new List<CspListModel>();
            try
            {
                 lstcspListservice = await (
                                        from p in _appDbContext.Set<CountryPicklist>()
                                        join c in _appDbContext.Set<Country>() on p.countryid equals c.id
                                        select new CspListModel()
                                        {
                                            Id = p.id,
                                            countrycode = c.code, 
                                            tablename = p.tablename,
                                            columnname = p.columnname,
                                            jsonvalue = p.jsonvalue,
                                            displayvalue = p.displayvalue,
                                            outputvalue = p.outputvalue
                                        })
                                        .OrderBy(a => a.countrycode)
                                           .ThenBy(a => a.tablename)
                                           .ThenBy(a => a.columnname)
                                           .ThenBy(a => a.jsonvalue)
                                        .ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
            }
            return lstcspListservice;
        }
        public virtual async Task<DatabaseResponse> AddOrUpdateCspListAsync(LoggedInUser user, CspListModel data)
        {
            var response = new DatabaseResponse();

            try
            {
                if (data != null)
                {
                    
                    var country =  _appDbContext.Set<Country>()
                                .FirstOrDefault(c => c.code.Trim().ToLower() == data.countrycode.Trim().ToLower());

                    if (country == null)
                    {
                        response.status = false;
                        response.message = "Invalid country code.";
                        return response;
                    }

                    var countryid = country.id;

                    var res =  _appDbContext.countrypicklist
                                .Where(t => t.tablename.Trim().ToLower() == data.tablename.Trim().ToLower()
                                            && t.columnname.Trim().ToLower() == data.columnname.Trim().ToLower()
                                            && t.jsonvalue.Trim().ToLower() == data.jsonvalue.Trim().ToLower()
                                            && t.countryid == countryid)
                                            .ToList();

                    if (res.Count > 0)
                    {
                        response.status = false;
                        response.message = "Value already exists.";
                        return response;
                    }
                    else
                    {
                        if (data.Id > 0)
                        {
                            var result = await _appDbContext.Set<CountryPicklist>()
                                .Where(x => x.id == data.Id)
                                .ExecuteUpdateAsync(updates => updates
                                    .SetProperty(p => p.tablename, data.tablename)
                                    .SetProperty(p => p.columnname, data.columnname)
                                    .SetProperty(p => p.jsonvalue, data.jsonvalue)
                                    .SetProperty(p => p.displayvalue, data.displayvalue)
                                    .SetProperty(p => p.outputvalue, data.outputvalue)
                                    .SetProperty(p => p.countryid, countryid)
                                    .SetProperty(p => p.modifiedat, _dateTimeHelper.GetDateTimeNow())
                                    .SetProperty(p => p.modifiedby, user.UserName));

                            if (result > 0)
                            {
                                response.status = true;
                                response.message = "Updated Successfully";
                            }
                            else
                            {
                                response.status = false;
                                response.message = "Update Failed";
                            }
                            return response;
                        }
                        else
                        {
                            var newEntry = new CountryPicklist
                            {
                                tablename = data.tablename,
                                columnname = data.columnname,
                                jsonvalue = data.jsonvalue,
                                displayvalue = data.displayvalue,
                                outputvalue = data.outputvalue,
                                countryid = countryid,
                                createdby = user.UserName,
                                createdat = _dateTimeHelper.GetDateTimeNow()

                            };

                            await _appDbContext.Set<CountryPicklist>().AddAsync(newEntry);
                            await _appDbContext.SaveChangesAsync();

                            if (newEntry.id > 0)
                            {
                                response.status = true;
                                response.message = "Added Successfully";
                            }
                            else
                            {
                                response.status = false;
                                response.message = "Add Failed";
                            }
                            return response;
                        }
                    }
                }
                else
                {
                    response.status = false;
                    response.message = "Received empty details.";
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding/updating the country picklist entry.");
                response.status = false;
                response.message = "An error occurred while processing your request.";
                return response;
            }
        }

        public virtual async Task<DatabaseResponse> DeleteCspList(LoggedInUser loggedInUser, int id)
        {
            var response = new DatabaseResponse();

            try
            {
                if (id > 0)
                {
                    var _cspList = _appDbContext.Set<CountryPicklist>().Where(x => x.id == id);
                    if (_cspList == null || !_cspList.Any())
                    {
                        response.status = false;
                        response.message = "Record not found";
                    }
                    else
                    {
                        int res = await _cspList.ExecuteDeleteAsync();
                        if (res > 0)
                        {
                            response.status = true;
                            response.message = "Deleted Successfully";
                        }
                        else
                        {
                            response.status = false;
                            response.message = "Deletion Unsuccessful";
                        }
                    }
                }
                else
                {
                    response.status = false;
                    response.message = "Invalid Id.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the country picklist entry.");
                response.status = false;
                response.message = "An error occurred while processing your request.";
            }
            return response;
        }

        public virtual bool CanView(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return true;
        }
    }
}
