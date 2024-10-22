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

namespace Services
{
    public class TaxAuthorityServices : ITaxAuthorityServices
    {
        private readonly ILogger<TaxAuthorityServices> _logger;
        private readonly AppDbContext _appDbContext;
        private readonly IDateTimeHelper _dateTimeHelper;

        public TaxAuthorityServices(AppDbContext appDbContext, ILogger<TaxAuthorityServices> logger, IDateTimeHelper dateTimeHelper)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _dateTimeHelper = dateTimeHelper;

        }
        public virtual List<TaxAuthorityModel> GetTaxAuthority(LoggedInUser user)
        {
            List<TaxAuthorityModel> _taxauthority = new List<TaxAuthorityModel>();
            try
            {
                _taxauthority = (from q in _appDbContext.Set<TaxAuthority>()

                                 join c in _appDbContext.Set<Country>() on q.countryid equals c.id into jcs
                                 from CResult in jcs.DefaultIfEmpty()

                                 select new TaxAuthorityModel()
                                 {
                                     id = q.id,
                                     code = q.code,
                                     name = q.name,
                                     namelocal = q.namelocal,
                                     countryid = CResult.id,
                                     countrycode = CResult.code,
                                     employeeliability = q.employeeliability,
                                     employerliability = q.employerliability,
                                     comments = q.comments,
                                     providercomments = q.providercomments,
                                     createdby = q.createdby,
                                     createdat = q.createdat,
                                     modifiedby = q.modifiedby,
                                     modifiedat = q.modifiedat,
                                     status = q.status,

                                 }).OrderByDescending(a => a.id).ToList();
                _taxauthority = _taxauthority.OrderByDescending(a => a.code).Distinct().ToList();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _taxauthority;
        }
        public virtual DatabaseResponse InsertTaxAuthority(LoggedInUser user, TaxAuthorityModel taxAuthorityModel)
        {
            DatabaseResponse response = new DatabaseResponse();
            try
            {
                if (taxAuthorityModel != null)
                {
                    TaxAuthorityModel model = new TaxAuthorityModel();
                    if (taxAuthorityModel.code != null & taxAuthorityModel.name != null)
                    {
                        List<TaxAuthorityModel> lstTaxAuthorityModel = new List<TaxAuthorityModel>();

                        lstTaxAuthorityModel = GetTaxAuthority(user);
                        var res = _appDbContext.taxauthority.Where(t => t.code.Trim().ToLower() == taxAuthorityModel.code.Trim().ToLower()).ToList();

                        if (res.Count > 0)
                        {
                            response.status = false;
                            response.message = "Tax Authorities Code already exist";
                            return response;
                        }
                        else
                        {

                            var taxAuthority = new TaxAuthority
                            {
                                code = taxAuthorityModel.code,
                                name = taxAuthorityModel.name,
                                namelocal = taxAuthorityModel.namelocal,
                                countryid = taxAuthorityModel.countryid,
                                employeeliability = taxAuthorityModel.employeeliability,
                                employerliability = taxAuthorityModel.employerliability,
                                comments = taxAuthorityModel.comments,
                                providercomments = taxAuthorityModel.providercomments,
                                createdat = _dateTimeHelper.GetDateTimeNow(),
                                status = taxAuthorityModel.status,
                                modifiedby = null,
                                modifiedat = null,
                                createdby = user.UserName
                            };

                            _appDbContext.Set<TaxAuthority>().Add(taxAuthority);
                            _appDbContext.SaveChanges();
                            response.status = true;
                            response.message = "Added Successfully";
                            return response;
                        }
                    }
                    else
                    {
                        response.status = false;
                        response.message = "Received blank/empty code or name received";
                        return response;

                    }


                }
                else
                {
                    response.status = false;
                    response.message = "Received empty Tax Authority details";
                    return response;
                }

            }
            catch (Exception ex) { }
            return response;
        }
        public virtual async Task<DatabaseResponse> UpdateTaxAuthority(LoggedInUser user, TaxAuthorityModel taxAuthorityModel)
        {
            DatabaseResponse response = new DatabaseResponse();

            try
            {
                if (taxAuthorityModel == null)
                {
                    response.status = false;
                    response.message = "Received empty Tax Authority details";
                    return response;
                }

                if (taxAuthorityModel.id <= 0)
                {
                    response.status = false;
                    response.message = "Received invalid Primary key";
                    return response;
                }

                var updateResult = await _appDbContext.taxauthority
                    .Where(x => x.id == taxAuthorityModel.id)
                    .ExecuteUpdateAsync(t => t
                        .SetProperty(x => x.name, taxAuthorityModel.name)
                        .SetProperty(x => x.countryid, taxAuthorityModel.countryid)
                        .SetProperty(x => x.employeeliability, taxAuthorityModel.employeeliability)
                        .SetProperty(x => x.employerliability, taxAuthorityModel.employerliability)
                        .SetProperty(x => x.namelocal, taxAuthorityModel.namelocal)
                        .SetProperty(x => x.comments, taxAuthorityModel.comments)
                        .SetProperty(x => x.providercomments, taxAuthorityModel.providercomments)
                        .SetProperty(x => x.modifiedat, _dateTimeHelper.GetDateTimeNow())
                        .SetProperty(x => x.modifiedby, user.UserName)
                        .SetProperty(x => x.status, taxAuthorityModel.status)
                    );

                if (updateResult > 0)
                {
                    response.status = true;
                    response.message = "Updated Successfully";
                }
                else
                {
                    response.status = false;
                    response.message = "Tax Authority not found";
                }
            }
            catch (Exception ex)
            {
                response.status = false;
                response.message = $"An error occurred: {ex.Message}";
                _logger.LogError("Exception occured {ex}", ex);
            }

            return response;
        }
        public virtual DatabaseResponse DeleteTaxAuthority(LoggedInUser user, int id)
        {
             DatabaseResponse response = new DatabaseResponse();
            try
            {
                
                var _taxauthority = _appDbContext.taxauthority.Where(t => t.id == id);

                if (_taxauthority.Any())
                {
                    _appDbContext.taxauthority.RemoveRange(_taxauthority);
                    _appDbContext.SaveChanges();
                    response.status = true;
                    response.message = "Deleted Succesfully";
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            response.status = false;
            response.message = response.message;
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
