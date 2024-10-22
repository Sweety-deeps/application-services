using Persistence;
using Services.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.Data;
using EFCore.BulkExtensions;
using Domain;
using Domain.Models.Users;
using Services.Helpers;

namespace Services
{
    public class PayrollElementServices : IPayrollElementServices
    {
        protected readonly ILogger _logger;
        protected readonly AppDbContext _appDbContext;
        protected readonly Dictionary<string, IDataImportService> _dataImportService;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public PayrollElementServices(AppDbContext appDbContext, IEnumerable<IDataImportService> dataImportService, ILoggedInUserRoleService loggedInUserRoleService, IDateTimeHelper dateTimeHelper, ILogger logger = null)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _dataImportService = dataImportService.ToDictionary(service => service.GetType().Namespace);
            _dateTimeHelper = dateTimeHelper;
            _loggedInUserRoleService = loggedInUserRoleService;
            
        }

        public virtual List<PayrollElementsModel> GetPayrollElements(LoggedInUser user, string paygroupCode)
        {
            List<PayrollElementsModel> _payrollelements = new List<PayrollElementsModel>();
            try
            {
                var paygroupid = _appDbContext.paygroup.Where(p => p.code == paygroupCode).FirstOrDefault().id;
                _payrollelements = (from pe in _appDbContext.Set<PayrollElements>()
                                    where pe.paygroupid == paygroupid

                                    join pg in _appDbContext.Set<PayGroup>() on pe.paygroupid equals pg.id into jps
                                    from pgResult in jps.DefaultIfEmpty()
                                        //where pgResult.status == "Active"

                                    join le in _appDbContext.Set<LegalEntity>() on pgResult.legalentityid equals le.id into jls
                                    from leResult in jls.DefaultIfEmpty()
                                        //where leResult.status == "Active"

                                    join ta in _appDbContext.Set<TaxAuthority>() on pe.taxauthorityid equals ta.id into jts
                                    from taResult in jts.DefaultIfEmpty()
                                        //where taResult.status == "Active"
                                    join C in _appDbContext.Set<Country>() on taResult.countryid equals C.id into jcs
                                    from CResult in jcs.DefaultIfEmpty()

                                        //orderby q.id descending

                                    select new PayrollElementsModel()
                                    {
                                        id = pe.id,
                                        paygroupid = pgResult != null ? pgResult.id : 0,
                                        paygroupcode = pgResult != null ? pgResult.code : "",
                                        legalentityid = leResult != null ? leResult.id : 0,
                                        legalentitycode = leResult != null ? leResult.code : "",
                                        code = pe.code,
                                        name = pe.name,
                                        type = pe.type,
                                        namelocal = pe.namelocal,
                                        exportcode = pe.exportcode,
                                        taxauthorityid = taResult != null ? taResult.id : 0,
                                        taxauthoritycode = taResult != null ? taResult.code : "",
                                        pestatus = pe.pestatus,
                                        itemtype = pe.itemtype,
                                        format = pe.format,
                                        glcreditcode = pe.glcreditcode,
                                        gldebitcode = pe.gldebitcode,
                                        clientreported = pe.clientreported,
                                        payslipprint = pe.payslipprint,
                                        comments = pe.comments,
                                        froms = pe.froms,
                                        tos = pe.tos,
                                        contributetonetpay = pe.contributetonetpay,
                                        isemployertax = pe.isemployertax,
                                        isemployerdeduction = pe.isemployerdeduction,
                                        createdby = pe.createdby,
                                        createdat = pe.createdat,
                                        modifiedby = pe.modifiedby,
                                        modifiedat = pe.modifiedat,
                                        status = pe.status,


                                    }).OrderByDescending(a => a.id).ToList();

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
            return _payrollelements;
        }
        public virtual DatabaseResponse InsertPayrollElements(LoggedInUser user, PayrollElementsModel payrollelementsModel)
        {
            DatabaseResponse response = new DatabaseResponse();
            try
            {
                if (payrollelementsModel != null)
                {
                    PayrollElementsModel model = new PayrollElementsModel();
                    if (payrollelementsModel.code != null & payrollelementsModel.name != null)
                    {
                        //List<PayrollElementsModel> lstPayrollElementsModel = new List<PayrollElementsModel>();

                        //lstPayrollElementsModel = GetPayrollElements();
                        var res = _appDbContext.payrollelements.Where(t => t.code.Trim().ToLower() == payrollelementsModel.code.Trim().ToLower()
                        && t.paygroupid == payrollelementsModel.paygroupid).ToList();

                        if (res.Count > 0)
                        {
                            response.status = false;
                            response.message = "Given Payroll Elements Code already exist";
                            return response;

                        }
                        else
                        {
                            var payrollelements = new PayrollElements
                            {

                                paygroupid = payrollelementsModel.paygroupid,
                                //paygroupcode=payrollelementsModel.paygroupcode,
                                code = payrollelementsModel.code,
                                name = payrollelementsModel.name,
                                type = payrollelementsModel.type,
                                namelocal = payrollelementsModel.namelocal,
                                exportcode = payrollelementsModel.exportcode,
                                taxauthorityid = payrollelementsModel.taxauthorityid,
                                pestatus = payrollelementsModel.pestatus,
                                itemtype = payrollelementsModel.itemtype,
                                format = payrollelementsModel.format,
                                glcreditcode = payrollelementsModel.glcreditcode,
                                gldebitcode = payrollelementsModel.gldebitcode,
                                clientreported = payrollelementsModel.clientreported,
                                payslipprint = payrollelementsModel.payslipprint,
                                comments = payrollelementsModel.comments,
                                froms = payrollelementsModel.froms,
                                tos = payrollelementsModel.tos,
                                contributetonetpay = payrollelementsModel.contributetonetpay,
                                isemployertax = payrollelementsModel.isemployertax,
                                isemployerdeduction = payrollelementsModel.isemployerdeduction,
                                createdat = _dateTimeHelper.GetDateTimeNow(),
                                createdby = user.UserName,
                                modifiedby = null,
                                modifiedat = null,
                                status = payrollelementsModel.status,
                            };

                            _appDbContext.Set<PayrollElements>().Add(payrollelements);
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
                    response.message = "Received empty Payroll Elements details";
                    return response;
                }

            }
            catch (Exception ex) { }
            return response;
        }

        public virtual DatabaseResponse UpdatePayrollElements(LoggedInUser user, PayrollElementsModel payrollelementsModel)
        {
            DatabaseResponse response = new DatabaseResponse();

            if (payrollelementsModel != null)
            {
                if (payrollelementsModel.id > 0)
                {
                    var dbResponse = _appDbContext.payrollelements.Where(x => x.id == payrollelementsModel.id).AsNoTracking().FirstOrDefault();
                    if (dbResponse != null)
                    {
                        dbResponse.id = dbResponse.id;
                        dbResponse.paygroupid = payrollelementsModel.paygroupid;
                        dbResponse.code = payrollelementsModel.code;
                        dbResponse.name = payrollelementsModel.name;
                        dbResponse.type = payrollelementsModel.type;
                        dbResponse.namelocal = payrollelementsModel.namelocal;
                        dbResponse.exportcode = payrollelementsModel.exportcode;
                        dbResponse.taxauthorityid = payrollelementsModel.taxauthorityid;
                        dbResponse.pestatus = payrollelementsModel.pestatus;
                        dbResponse.itemtype = payrollelementsModel.itemtype;
                        dbResponse.format = payrollelementsModel.format;
                        dbResponse.glcreditcode = payrollelementsModel.glcreditcode;
                        dbResponse.gldebitcode = payrollelementsModel.gldebitcode;
                        dbResponse.clientreported = payrollelementsModel.clientreported;
                        dbResponse.payslipprint = payrollelementsModel.payslipprint;
                        dbResponse.comments = payrollelementsModel.comments;
                        dbResponse.froms = payrollelementsModel.froms;
                        dbResponse.tos = payrollelementsModel.tos;
                        dbResponse.contributetonetpay = payrollelementsModel.contributetonetpay;
                        dbResponse.isemployertax = payrollelementsModel.isemployertax;
                        dbResponse.isemployerdeduction = payrollelementsModel.isemployerdeduction;
                        dbResponse.createdby = payrollelementsModel.createdby;
                        dbResponse.createdat = payrollelementsModel.createdat;
                        dbResponse.modifiedby = user.UserName;
                        dbResponse.modifiedat = _dateTimeHelper.GetDateTimeNow();
                        dbResponse.status = payrollelementsModel.status;
                        _appDbContext.payrollelements.Update(dbResponse);
                        _appDbContext.SaveChanges();
                        _appDbContext.Entry(dbResponse).State = EntityState.Detached;
                        response.status = true;
                        response.message = "Added Successfully";
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
                response.message = "Received empty Payroll Elements details";
                return response;
            }

            return response;

        }
        public virtual void DeletePayrollElements(LoggedInUser user, int id)
        {
            try
            {
                var _payrollelements = _appDbContext.payrollelements.Where(s => s.id == id);

                if (_payrollelements.Any())
                {
                    _appDbContext.payrollelements.RemoveRange(_payrollelements);
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.ToString());
            }
        }

        public virtual async Task<DatabaseResponse> UploadPayElements(LoggedInUser user, PayCalendarUploadModal mdl)
        {
            DatabaseResponse response = new DatabaseResponse();
            try
            {
                DataImportRequestModel dataImportRequestModel = new DataImportRequestModel
                {
                    file = mdl.Excelfile,
                    entityName = "PayrollElement",
                    payGroup = mdl.paygroup
                };

                IDataImportService dataImportService = _loggedInUserRoleService.GetServiceForController<IDataImportService>(user, _dataImportService);
                var result = await dataImportService.UploadDataImport(user, dataImportRequestModel);

                if (result.status == false)
                {
                    response.status = false;
                    response.message = result.message;
                    return response;
                }

                response.status = true;
                response.message = "File Uploaded";
                return response;
            }
            catch (Exception e)
            {
                response.status = false;
                response.message = e.StackTrace.ToString();
                return response;
            }
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
