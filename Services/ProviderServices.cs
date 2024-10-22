using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Abstractions;
using Domain.Models;
using Domain.Entities;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using Domain.Models.Users;
using Services.Helpers;

namespace Services
{
    public class ProviderServices : IProviderServices
    {
        protected readonly ILogger _logger;
        protected readonly AppDbContext _appDbContext;
        private readonly IDateTimeHelper _dateTimeHelper;

        public ProviderServices(AppDbContext appDbContext, IDateTimeHelper dateTimeHelper,ILogger logger = null)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _dateTimeHelper = dateTimeHelper;

        }
        public List<ProviderModel> GetProviderDetails(LoggedInUser user)
        {
            List<ProviderModel> lstProviderModel = new List<ProviderModel>();
            try
            {
                var dbValue = _appDbContext.provider.ToList();

                //if (strCode != "null")
                //{ dbValue = dbValue.Where(x => x.code == strCode).ToList(); }

                //if (strName != "null")
                //{ dbValue = dbValue.Where(x => x.name == strName).ToList(); }

                lstProviderModel = (from P in dbValue
                                    //where P.status =="Active"

                                    select new ProviderModel()
                                    {
                                        id = P.id,
                                        name = P.name,
                                        code = P.code,
                                        createdat = P.createdat,
                                        createdby = P.createdby,
                                        modifiedat = P.modifiedat,
                                        modifiedby = P.modifiedby,
                                        status = P.status
                                    }).OrderByDescending(a=>a.id).ToList();

                return lstProviderModel;

            }
            catch (Exception ex)
            {
                //TODO Need to do error logging
                return lstProviderModel;
            }

        }

        public virtual DatabaseResponse InsertProviderDetails(LoggedInUser user, ProviderModel providerModel)
        {
            DatabaseResponse response = new DatabaseResponse();
            try
            {
                if (providerModel != null)
                {
                    ProviderModel model = new ProviderModel();
                    if (providerModel.code != null & providerModel.name != null)
                    {
                        List<ProviderModel> lstProviderModel = new List<ProviderModel>();

                        lstProviderModel = GetProviderDetails(user);
                        var res = _appDbContext.provider.Where(t => t.code.Trim().ToLower() == providerModel.code.Trim().ToLower()).ToList();

                        if (res.Count > 0)
                        {
                            response.status = false;
                            response.message = "Code already exist";
                            return response;

                        }
                        else
                        {
                            var provider = new Provider
                            {
                                code = providerModel.code,
                                name = providerModel.name,
                                createdat = _dateTimeHelper.GetDateTimeNow(),
                                status=providerModel.status,
                                createdby = user.UserName
                            };
                            _appDbContext.Set<Provider>().Add(provider);
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
                    response.message = "Received empty Provider details";
                    return response;
                }

            }
            catch (Exception ex) { }
            return response;
        }

        public virtual DatabaseResponse UpdateProviderDetails(LoggedInUser user, ProviderModel providerModel)
        {
            DatabaseResponse response = new DatabaseResponse();

            if (providerModel != null)
            {
                if (providerModel.id > 0)
                {
                    var dbResponse = _appDbContext.provider.Where(x => x.id == providerModel.id).AsNoTracking().FirstOrDefault();
                    if (dbResponse != null)
                    {
                        dbResponse.id = dbResponse.id;
                        dbResponse.code = providerModel.code;
                        dbResponse.name = providerModel.name;
                        dbResponse.modifiedat = _dateTimeHelper.GetDateTimeNow();
                        dbResponse.modifiedby = user.UserName;
                        dbResponse.status = providerModel.status;
                        _appDbContext.provider.Update(dbResponse);
                        _appDbContext.SaveChanges();
                        _appDbContext.Entry(dbResponse).State = EntityState.Detached;
                        response.status = true;
                        response.message = "Updated Successfully";
                        return response;
                    }
                    else
                    {

                    }
                }
                else
                {
                    response.status = false;
                    response.message = "Received invalid Primary key";
                    return response;
                }
            }
            else
            {
                response.status = false;
                response.message = "Received empty Provider details";
                return response;
            }

            return response;

        }
        //public void DeleteProvider(int id)
        //{
        //    try
        //    {
        //        var _provider = _appDbContext.provider.Where(s => s.id == id).FirstOrDefault();

        //        if (_provider != null)
        //        {
        //            _provider.status = "In-Active";
        //            _provider.modifiedat = DateTime.Now;
        //            _provider.modifiedby = _provider.modifiedby;
        //            _appDbContext.provider.Update(_provider);
        //            _appDbContext.SaveChanges();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log(LogLevel.Error, ex.ToString());
        //    }
        //    //return sResponse;
        //}


        public virtual DatabaseResponse DeleteProvider(LoggedInUser user, int id)
        {
            var responseText = "This Partner is associated with ";
            DatabaseResponse response = new DatabaseResponse();
            try
            {
                var _provider = _appDbContext.provider.Where(s => s.id == id);

                var _client = _appDbContext.client.Where(s => s.providerid == id);

                if (_client.Any())
                {
                    response.status = false;
                    responseText += " Client ";

                }
                else if (_provider.Any())
                {
                    _appDbContext.provider.RemoveRange(_provider);
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
            response.message = responseText;
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
