using Services.Abstractions;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;

namespace Services.UnAuthorized
{
    public class ReportServices : IReportServices
    {
        public virtual bool CanView(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public Task<string> GetCalendarReport(LoggedInUser user, CalendarRequestModel requestModel,string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public string GetComparisonData(LoggedInUser user, int? iRequestID)
        {
            throw new UnauthorizedAccessException();
        }

        public string GetCountryByClientCode(LoggedInUser user, string paygroup)
        {
            throw new UnauthorizedAccessException();
        }

        public int? GetCountryIDByCode(LoggedInUser user, string strCode)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<(string,string)> GetStartAndEndTime(LoggedInUser user, StartEndTimeRequestModel requestBody)
        {
            throw new UnauthorizedAccessException();
        }
        public CSAndCFDatawarehouseResponseModel GetCSPFDatawarehouse(LoggedInUser user, CSPFDatawarehouseRequestModel _data)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> GetCSPFDatawarehouse(LoggedInUser user, CSAndCFDatawarehouseResponseModel results, CSPFDatawarehouseRequestModel _mdl, string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public List<ErrorLogResponseModel> GetErrorLogReport(LoggedInUser user, ErrorLogRequestModel _data)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> GetErrorLogReport(LoggedInUser user, List<ErrorLogResponseModel> results, ErrorLogRequestModel _mdl, string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public List<HrDatawarehouseResponseModel> GetHrDatawarehouse(LoggedInUser user, HrDatawarehouseRequestModel _data)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> GetHrDatawarehouse(LoggedInUser user, List<HrDatawarehouseResponseModel> results, HrDatawarehouseRequestModel _mdl, string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public List<RequestLowLevelDetails> GetLowLevelDetails(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public List<PaydDatawarehouseResponseModel> GetPAYDDatawarehouse(LoggedInUser user, PaydDatawarehouseRequestModel _data)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> GetPAYDDatawarehouse(LoggedInUser user, List<PaydDatawarehouseResponseModel> results, PaydDatawarehouseRequestModel _mdl, string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public List<PayElementResponseModel> GetPayElementReport(LoggedInUser user, PayElementRequestModel _data)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<string> GetConfigurationReportAsync(LoggedInUser user,Task<ConfigurationResponseModel> model,ConfigurationRequestModel requestModel, string filename)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<string> GetPayElementReport(LoggedInUser user, List<PayElementResponseModel> results, PayElementRequestModel _mdl, string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public string GetPayFrequencyForPayGroup(LoggedInUser user, string PayGroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public int GetPayGroupIDByCode(LoggedInUser user, string strCode)
        {
            throw new UnauthorizedAccessException();
        }

        public List<PayPeriodWithPrevioudCutOff> GetPayPeriodCutoff(LoggedInUser user, int paygroupId, int year, int? payperiod)
        {
            throw new UnauthorizedAccessException();
        }

        public DateTime GetPayPeriodDate(LoggedInUser user, string strPayGroupCode, string strTaskName, int iPayPeriod)
        {
            throw new UnauthorizedAccessException();
        }

        public async Task<List<PayPeriodRegisterDetailResponseModel>> GetPayPeriodDetailRegistersAsync(LoggedInUser user, PayPeriodRegisterRequestModel _data, int iPayGroupID)
        {
            throw new UnauthorizedAccessException();
        }

        public async Task<List<PayPeriodRegisterResponseModel>> GetPayPeriodRegistersAsync(LoggedInUser user, PayPeriodRegisterRequestModel _data, int iPayGroupID)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> GetPayPeriodRegisters(LoggedInUser user, List<PayPeriodRegisterResponseModel> results, List<PayPeriodRegisterDetailResponseModel> detailresults, PayPeriodRegisterRequestModel _mdl, List<PayrollElementsModel> peData, string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public List<int> GetPayPeriodsByCountryCode(LoggedInUser user, int? iCountryID)
        {
            throw new UnauthorizedAccessException();
        }

        public List<int> GetPayPeriodsByPayGroupCode(LoggedInUser user, string strPayGroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public List<int> GetPayPeriodsByPayGroupID(LoggedInUser user, int payGroupID)
        {
            throw new UnauthorizedAccessException();
        }

        public List<int> GetPayPeriodsByYear(LoggedInUser user, int payGroupID, int year)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> GetPeriodChangeFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> GetHybridChageFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName)
        {
            throw new UnauthorizedAccessException();
        }
        public VariancePayPeriodDetails GetPreviousPayPeriod(LoggedInUser user, int iPayGroupID, int iPayPeriodYear)
        {
            throw new UnauthorizedAccessException();
        }

        public List<RequestHighLevelDetails> GetRequestHighLevelDetails(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public List<SystemUserResponseModel> GetSystemUserReport(LoggedInUser user, SystemUserRequestModel _data)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<ConfigurationResponseModel> GetConfigurationData(LoggedInUser user, ConfigurationRequestModel model)
        {
            throw new UnauthorizedAccessException();
        }
        public string? GetTemplateForConfigurationReport(string? configname)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<string> GetSystemUserReport(LoggedInUser user, List<SystemUserResponseModel> results, SystemUserRequestModel _mdl, string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public List<TransactionCountryResponseModel> GetTransactionByCountryReport(LoggedInUser user, TransactionCountryRequestModel _data)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> GetTransactionByCountryReport(LoggedInUser user, List<TransactionCountryResponseModel> results, TransactionCountryRequestModel _mdl, string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public List<TransactionResponseModel> GetTransactionByPayGroupReport(LoggedInUser user, TransactionRequestModel _data)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> GetTransactionByPayGroupReport(LoggedInUser user, List<TransactionResponseModel> results, TransactionRequestModel _mdl, string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public List<VarianceResponseModel> GetVarianceReport(LoggedInUser user, VarianceRequestModel _data)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> GetVarianceReport(LoggedInUser user, List<VarianceResponseModel> results, VarianceRequestModel _mdl, string fileName)
        {
            throw new UnauthorizedAccessException();
        }

        public void InsertError(LoggedInUser user, int iRequestID, string strProject, string strCode, string strShortDescription, string strLongDescription)
        {
            throw new UnauthorizedAccessException();
        }

        public PeriodChangeFileRequestModel SetStartandEndDateBasedonPayperiod(LoggedInUser user, PeriodChangeFileRequestModel _req)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<string> GetCenamPeriodChangeFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<string> GetMexicoPeriodChangeFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<List<ReportServiceDetails>> GetReportServiceDetailsAsync(LoggedInUser user, string? paygroup)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<string> DownloadReportServiceDetails(LoggedInUser loggedInUser, int id)
        {
            throw new UnauthorizedAccessException();
        }

    }
}
