using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;
using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using Newtonsoft.Json;

namespace Services
{
    public class PayGroupServices : IPayGroupServices
    {
        protected readonly ILogger<PayGroupServices> _logger;
        protected readonly AppDbContext _appDbContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEncrytionHelper _encrytionHelper;

        public PayGroupServices(AppDbContext appDbContext, ILogger<PayGroupServices> logger, IDateTimeHelper dateTimeHelper, IEncrytionHelper encrytionHelper)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _dateTimeHelper = dateTimeHelper;
            _encrytionHelper = encrytionHelper;
        }

        public virtual async Task<List<PaygroupMinimalModel>> GetActivePaygroups(LoggedInUser user)
        {
            var query = _appDbContext.paygroup
                .Where(e => e.status != null && e.status.ToLower() == "active")
                .Select(e => new PaygroupMinimalModel { Id = e.id, Code = e.code, Name = e.name })
                .OrderBy(e => e.Code);

            return await query.ToListAsync();
        }

        public virtual async Task<List<PayGroupModel>> GetPayGroupDetails(LoggedInUser user)
        {
            var lstPayGroupModel = new List<PayGroupModel>();
            try
            {
                lstPayGroupModel = await (from P in _appDbContext.Set<PayGroup>()
                                          join L in _appDbContext.Set<LegalEntity>() on P.legalentityid equals L.id into jls
                                          from LResult in jls.DefaultIfEmpty()
                                          join PF in _appDbContext.Set<PayFrequency>() on P.payfrequencyid equals PF.id into jps
                                          from PFResult in jps.DefaultIfEmpty()
                                          join C in _appDbContext.Set<Country>() on P.countryid equals C.id into jcs
                                          from CResult in jcs.DefaultIfEmpty()
                                          select new PayGroupModel()
                                          {
                                              id = P.id,
                                              name = P.name,
                                              code = P.code,
                                              payfrequencyid = PFResult != null ? PFResult.id : 0,
                                              payfrequencycode = PFResult != null ? PFResult.code : "",
                                              legalentityid = LResult != null ? LResult.id : 0,
                                              legalentitycode = LResult != null ? LResult.code : "",
                                              countryid = CResult != null ? CResult.id : 0,
                                              countrycode = CResult != null ? CResult.code : "",
                                              emailto = P.emailto,
                                              emailcc = P.emailcc,
                                              emailsubject = P.emailsubject,
                                              transactioncurrency = P.transactioncurrency,
                                              createdat = P.createdat,
                                              createdby = P.createdby,
                                              modifiedat = P.modifiedat,
                                              modifiedby = P.modifiedby,
                                              status = P.status.Trim(),
                                              outboundformat = P.outboundformat,
                                              inbound_sftp_folder = P.InboundSftpFolder,
                                              gpri_sftp_folder = P.GpriSftpFolder,
                                              payslip_sftp_folder = P.PayslipSftpFolder,
                                              cvecia = P.cvecia,
                                              collectchanges = P.collectchanges ? "Yes" : "No",
                                              ApiClientId = P.ApiClientId,
                                              ApiUserName = _encrytionHelper.Decrypt(P.ApiUserName),
                                              ApiPassword = _encrytionHelper.Decrypt(P.ApiPassword),
                                              urlPrefix = P.urlPrefix,
                                              outbound_sftp_server = P.OutboundSftpServer,
                                          })
                                    .OrderByDescending(a => a.id).ToListAsync();

                return lstPayGroupModel;

            }
            catch (Exception ex)
            {
                //TODO Need to do error logging
                return lstPayGroupModel;
            }
        }
        
        public virtual DatabaseResponse AddPayGroup(LoggedInUser user, PayGroupModel paygroupModel)
        {
            var response = new DatabaseResponse();
            try
            {
                if (paygroupModel != null)
                {
                    if (paygroupModel.code != null && paygroupModel.name != null)
                    {
                        var res = _appDbContext.paygroup.Where(t => t.code.Trim().ToLower() == paygroupModel.code.Trim().ToLower()).ToList();

                        if (res.Count > 0)
                        {
                            response.status = false;
                            response.message = "Code already exist";
                            return response;

                        }
                        else
                        {
                            var paygroup = new PayGroup
                            {
                                code = paygroupModel.code,
                                name = paygroupModel.name,
                                legalentityid = paygroupModel.legalentityid,
                                payfrequencyid = paygroupModel.payfrequencyid,
                                countryid = paygroupModel.countryid,
                                emailto = paygroupModel.emailto,
                                status = paygroupModel.status,
                                createdat = _dateTimeHelper.GetDateTimeNow(),
                                createdby = user.UserName,
                                modifiedby = null,
                                modifiedat = null,
                                outboundformat = paygroupModel.outboundformat,
                                InboundSftpFolder = paygroupModel.inbound_sftp_folder,
                                GpriSftpFolder = paygroupModel.gpri_sftp_folder,
                                PayslipSftpFolder = paygroupModel.payslip_sftp_folder,
                                cvecia = paygroupModel.cvecia,
                                transactioncurrency = paygroupModel.transactioncurrency,
                                collectchanges = paygroupModel.collectchanges == "Yes",
                                ApiClientId = paygroupModel.ApiClientId,
                                ApiUserName = _encrytionHelper.Encrypt(paygroupModel.ApiUserName),
                                ApiPassword = _encrytionHelper.Encrypt(paygroupModel.ApiPassword),
                                urlPrefix = paygroupModel.urlPrefix,
                                OutboundSftpServer = paygroupModel.outbound_sftp_server
                            };

                            _appDbContext.Set<PayGroup>().Add(paygroup);
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
                    response.message = "Received empty Pay Group details";
                    return response;
                }

            }
            catch (Exception ex) { }
            return response;
        }

        public virtual DatabaseResponse UpdatePayGroupDetails(LoggedInUser user, PayGroupModel payGroupModel)
        {
            DatabaseResponse response = new DatabaseResponse();

            if (payGroupModel != null)
            {
                if (payGroupModel.id > 0)
                {
                    var dbResponse = _appDbContext.paygroup.Where(x => x.id == payGroupModel.id).AsNoTracking().FirstOrDefault();
                    if (dbResponse != null)
                    {
                        dbResponse.id = payGroupModel.id;
                        dbResponse.code = payGroupModel.code;
                        dbResponse.name = payGroupModel.name;
                        dbResponse.legalentityid = payGroupModel.legalentityid;
                        dbResponse.payfrequencyid = payGroupModel.payfrequencyid;
                        dbResponse.countryid = payGroupModel.countryid;
                        dbResponse.emailto = payGroupModel.emailto;
                        dbResponse.modifiedat = _dateTimeHelper.GetDateTimeNow();
                        dbResponse.modifiedby = user.UserName;
                        dbResponse.status = payGroupModel.status;
                        dbResponse.outboundformat = payGroupModel.outboundformat;
                        dbResponse.InboundSftpFolder = payGroupModel.inbound_sftp_folder;
                        dbResponse.GpriSftpFolder = payGroupModel.gpri_sftp_folder;
                        dbResponse.PayslipSftpFolder = payGroupModel.payslip_sftp_folder;
                        dbResponse.cvecia = payGroupModel.cvecia;
                        dbResponse.transactioncurrency = payGroupModel.transactioncurrency;
                        dbResponse.collectchanges = payGroupModel.collectchanges == "Yes";
                        dbResponse.ApiClientId = payGroupModel.ApiClientId;
                        dbResponse.ApiUserName = _encrytionHelper.Encrypt(payGroupModel.ApiUserName);
                        dbResponse.ApiPassword = _encrytionHelper.Encrypt(payGroupModel.ApiPassword);
                        dbResponse.urlPrefix = payGroupModel.urlPrefix;
                        dbResponse.OutboundSftpServer = payGroupModel.outbound_sftp_server;
                        _appDbContext.paygroup.Update(dbResponse);
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
                response.message = "Received empty PayGroup details";
                return response;
            }

            return response;

        }

        public virtual DatabaseResponse DeletePayGroup(LoggedInUser user, int id)
        {
            var responseText = "This PayGroupID is associated with a records in";
            DatabaseResponse response = new DatabaseResponse();
            try
            {
                var _paygroup = _appDbContext.paygroup.Where(s => s.id == id);

                var _gpri = _appDbContext.gpri.Where(s => s.paygroupid == id);

                var _paycalendar = _appDbContext.paycalendar.Where(s => s.paygroupid == id);

                var _payrollelements = _appDbContext.payrollelements.Where(s => s.paygroupid == id);

                if (_gpri.Any())
                {
                    response.status = false;
                    responseText += " Global Pay Run Import,";

                }
                if (_paycalendar.Any())
                {
                    response.status = false;
                    responseText += " Pay Calendar,";

                }
                if (_payrollelements.Any())
                {
                    response.status = false;
                    responseText += " Payroll Elements";

                }
                else if (_paygroup.Any())
                {
                    _appDbContext.paygroup.RemoveRange(_paygroup);
                    _appDbContext.SaveChanges();
                    response.status = true;
                    response.message = "Deleted Succesfully";
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());
            }
            response.status = false;
            response.message = responseText;
            return response;
        }

        #region Country
        public virtual List<CountryModel> GetCountryDetails(LoggedInUser user)
        {
            List<CountryModel> lstCountryModel = new List<CountryModel>();
            try
            {
                var dbValue = _appDbContext.country.Where(x => x.status == "Active").ToList();

                lstCountryModel = (from P in dbValue
                                   select new CountryModel()
                                   {
                                       id = P.id,
                                       name = P.name,
                                       code = P.code,
                                       createdat = P.createdat,
                                       createdby = P.createdby,
                                       modifiedat = P.modifiedat,
                                       modifiedby = P.modifiedby,
                                       status = P.status
                                   }).OrderByDescending(a => a.id).ToList();

                return lstCountryModel;

            }
            catch (Exception ex)
            {
                //TODO Need to do error logging
                return lstCountryModel;
            }

        }
        #endregion

        #region payfrequency
        public virtual List<PayFrequencyModel> GetPayFrequencyDetails(LoggedInUser user)
        {
            List<PayFrequencyModel> lstPayFrequencyModel = new List<PayFrequencyModel>();
            try
            {
                var dbValue = _appDbContext.payfrequency.Where(x => x.status == "Active").ToList();

                lstPayFrequencyModel = (from P in dbValue
                                        select new PayFrequencyModel()
                                        {
                                            id = P.id,
                                            name = P.name,
                                            code = P.code,
                                            createdat = P.createdat,
                                            createdby = P.createdby,
                                            modifiedat = P.modifiedat,
                                            modifiedby = P.modifiedby,
                                            status = P.status
                                        }).OrderByDescending(a => a.id).ToList();

                return lstPayFrequencyModel;

            }
            catch (Exception ex)
            {
                //TODO Need to do error logging
                return lstPayFrequencyModel;
            }

        }
        #endregion

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

        public virtual async Task<bool> LauchCollectChangesAction(LoggedInUser loggedInUser, PayGroupModel payGroupModel, String eventFunctionArn)
        {
            var awsBridgeClient = new AmazonEventBridgeClient();
            PutEventsRequest collectChangesEvent = new PutEventsRequest
            {
                Entries =
                        {
                            new PutEventsRequestEntry
                            {
                                Source = eventFunctionArn,
                                EventBusName = "default",
                                DetailType = "collectchangesevent",
                                Time = DateTime.Now,
                                Detail = JsonConvert.SerializeObject(
                                    new
                                    {
                                        Action = "MANUAL_COLLECTCHANGES",
                                        Paygroup = payGroupModel.id,
                                        Scheduled = false,
                                        TriggerdBy = loggedInUser.UserName
                                    }
                                )
                            }
                        }
            };

            return (await awsBridgeClient.PutEventsAsync(collectChangesEvent)).HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
