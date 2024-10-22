using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities;
using Domain.Entities.Users;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;
using SQLitePCL;


namespace Services
{
    public class GenericListServices : IGenericListServices
    {
        protected readonly ILogger<GenericListServices> _logger;
        protected readonly AppDbContext _appDbContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        public GenericListServices(AppDbContext appDbContext, ILogger<GenericListServices> logger, IDateTimeHelper dateTimeHelper)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _dateTimeHelper = dateTimeHelper;
        }

        public virtual async Task<List<GenericListModel>> GetGenericListDetails(LoggedInUser user)
        {
            var lstGenericListservice = new List<GenericListModel>();
            try
            {
                lstGenericListservice = await (from P in _appDbContext.Set<GenericList>()
                                          select new GenericListModel()
                                          {
                                              id = P.id,
                                              tablename = P.tablename,
                                              columnname = P.columnname,
                                              lookupvalue = P.lookupvalue

                                          })
                                          .OrderBy(a => a.tablename)
                                           .ThenBy(a => a.columnname)
                                           .ThenBy(a => a.lookupvalue)
                                          .ToListAsync();
                
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex); 
            }
            return lstGenericListservice;
        }

        public virtual async Task<DatabaseResponse> AddOrUpdateGenericListAsync(LoggedInUser loggedInUser, GenericListModel data)
        {
            var response = new DatabaseResponse();
            try
            {
                if (data != null)
                {
                    if (data.tablename != null && data.columnname != null && data.lookupvalue != null)
                    {
                        var res = _appDbContext.lookupdetails
                                    .Where(t => t.tablename.Trim().ToLower() == data.tablename.Trim().ToLower()
                                                && t.columnname.Trim().ToLower() == data.columnname.Trim().ToLower()
                                                && t.lookupvalue.Trim().ToLower() == data.lookupvalue.Trim().ToLower()).ToList();

                        if (res.Count > 0)
                        {
                            response.status = false;
                            response.message = "Value already exist";
                            return response;

                        }
                        else
                        {
                            if (data.id > 0)
                            {
                                var affectedRows = await _appDbContext.lookupdetails
                                    .Where(x => x.id == data.id)
                                    .ExecuteUpdateAsync(setters => setters
                                        .SetProperty(x => x.tablename, data.tablename)
                                        .SetProperty(x => x.columnname, data.columnname)
                                        .SetProperty(x => x.lookupvalue, data.lookupvalue)
                                        .SetProperty(x => x.modifiedby, loggedInUser.UserName)
                                        .SetProperty(x => x.modifiedat, _dateTimeHelper.GetDateTimeNow())
                                         );

                                response.status = affectedRows > 0;
                                response.message = affectedRows > 0 ? "Updated Successfully" : "Update Failed";
                                return response;

                            }
                            else
                            {
                                var genericlist = new GenericList
                                {
                                    tablename = data.tablename,
                                    columnname = data.columnname,
                                    lookupvalue = data.lookupvalue,
                                    createdby = loggedInUser.UserName,
                                    createdat = _dateTimeHelper.GetDateTimeNow()
                                };

                                await _appDbContext.Set<GenericList>().AddAsync(genericlist);
                                await _appDbContext.SaveChangesAsync();

                                if (genericlist.id > 0)
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
                        response.message = "Received blank/empty tablename, columnname or lookupvalue received";
                        return response;

                    }
                }
                else
                {
                    response.status = false;
                    response.message = "Received empty Fixed List (Generic) details";
                    return response;
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError("{ex}", ex);
            }
            return response;
        }

        public virtual  async Task<DatabaseResponse> DeleteGenericList(LoggedInUser user, int id)
        {
            DatabaseResponse response = new DatabaseResponse();
            try
            {
                var _genericlist = _appDbContext.lookupdetails.Where(s => s.id == id);
                if (_genericlist == null)
                {
                    response.status = false;
                    response.message = "Record not found";
                }
                else
                {
                    int res = await _genericlist.ExecuteDeleteAsync();
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
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                response.status = false;
                response.message = "An error occurred while deleting the record";
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
