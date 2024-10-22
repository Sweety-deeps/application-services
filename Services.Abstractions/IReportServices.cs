using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IReportServices : IUIPermissions
    {
        //string GetRequestHighLevelDetails(LoggedInUser user, DateTime dtFrom, DateTime dtTo, string payGroup);
        List<RequestHighLevelDetails> GetRequestHighLevelDetails(LoggedInUser user);
        List<RequestLowLevelDetails> GetLowLevelDetails(LoggedInUser user);
        string GetComparisonData(LoggedInUser user, int? iRequestID);
        Task<List<PayPeriodRegisterResponseModel>> GetPayPeriodRegistersAsync(LoggedInUser user, PayPeriodRegisterRequestModel _data, int iPayGroupID);
        List<HrDatawarehouseResponseModel> GetHrDatawarehouse(LoggedInUser user, HrDatawarehouseRequestModel _data);
        CSAndCFDatawarehouseResponseModel GetCSPFDatawarehouse(LoggedInUser user, CSPFDatawarehouseRequestModel _data);
        List<PaydDatawarehouseResponseModel> GetPAYDDatawarehouse(LoggedInUser user, PaydDatawarehouseRequestModel _data);
        List<PayElementResponseModel> GetPayElementReport(LoggedInUser user, PayElementRequestModel _data);
        List<TransactionResponseModel> GetTransactionByPayGroupReport(LoggedInUser user, TransactionRequestModel _data);
        List<TransactionCountryResponseModel> GetTransactionByCountryReport(LoggedInUser user, TransactionCountryRequestModel _data);
        List<SystemUserResponseModel> GetSystemUserReport(LoggedInUser user, SystemUserRequestModel _data);
        List<VarianceResponseModel> GetVarianceReport(LoggedInUser user, VarianceRequestModel _data);
        List<ErrorLogResponseModel> GetErrorLogReport(LoggedInUser user, ErrorLogRequestModel _data);
        List<int> GetPayPeriodsByPayGroupID(LoggedInUser user, int payGroupID);
        List<int> GetPayPeriodsByPayGroupCode(LoggedInUser user, string strPayGroupCode);
        List<int> GetPayPeriodsByCountryCode(LoggedInUser user, int? iCountryID);
        string GetCountryByClientCode(LoggedInUser user, string paygroup);
        List<int> GetPayPeriodsByYear(LoggedInUser user, int payGroupID, int year);
        VariancePayPeriodDetails GetPreviousPayPeriod(LoggedInUser user, int iPayGroupID, int iPayPeriodYear);
        Task<string> GetPayElementReport(LoggedInUser user, List<PayElementResponseModel> results, PayElementRequestModel _mdl, string fileName);
        Task<string> GetCSPFDatawarehouse(LoggedInUser user, CSAndCFDatawarehouseResponseModel results, CSPFDatawarehouseRequestModel _mdl, string fileName);
        Task<string> GetPAYDDatawarehouse(LoggedInUser user, List<PaydDatawarehouseResponseModel> results, PaydDatawarehouseRequestModel _mdl, string fileName);
        Task<string> GetSystemUserReport(LoggedInUser user, List<SystemUserResponseModel> results, SystemUserRequestModel _mdl, string fileName);
        Task<string> GetTransactionByPayGroupReport(LoggedInUser user, List<TransactionResponseModel> results, TransactionRequestModel _mdl, string fileName);
        Task<string> GetTransactionByCountryReport(LoggedInUser user, List<TransactionCountryResponseModel> results, TransactionCountryRequestModel _mdl, string fileName);
        Task<string> GetHrDatawarehouse(LoggedInUser user, List<HrDatawarehouseResponseModel> results, HrDatawarehouseRequestModel _mdl, string fileName);
        Task<string> GetVarianceReport(LoggedInUser user, List<VarianceResponseModel> results, VarianceRequestModel _mdl, string fileName);
        Task<string> GetPeriodChangeFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName);
        Task<string> GetCenamPeriodChangeFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName);
        Task<string> GetMexicoPeriodChangeFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName);
        Task<string> GetPayPeriodRegisters(LoggedInUser user, List<PayPeriodRegisterResponseModel> results, List<PayPeriodRegisterDetailResponseModel> detailresults, PayPeriodRegisterRequestModel _mdl, List<PayrollElementsModel> peData, string fileName);
        Task<string> GetErrorLogReport(LoggedInUser user, List<ErrorLogResponseModel> results, ErrorLogRequestModel _mdl, string fileName);
        void InsertError(LoggedInUser user, int iRequestID, string strProject, string strCode, string strShortDescription, string strLongDescription);
        DateTime GetPayPeriodDate(LoggedInUser user, string strPayGroupCode, string strTaskName, int iPayPeriod);
        int? GetCountryIDByCode(LoggedInUser user, string strCode);
        List<PayPeriodWithPrevioudCutOff> GetPayPeriodCutoff(LoggedInUser user, int paygroupId, int year, int? payperiod);
        int GetPayGroupIDByCode(LoggedInUser user, string strCode);
        PeriodChangeFileRequestModel SetStartandEndDateBasedonPayperiod(LoggedInUser user, PeriodChangeFileRequestModel _req);
        string GetPayFrequencyForPayGroup(LoggedInUser user, string PayGroupCode);
        Task<List<PayPeriodRegisterDetailResponseModel>> GetPayPeriodDetailRegistersAsync(LoggedInUser user, PayPeriodRegisterRequestModel _data, int iPayGroupID);
        Task<string> GetCalendarReport(LoggedInUser loggedInUser, CalendarRequestModel requestModel, string fileName);
        Task<ConfigurationResponseModel> GetConfigurationData(LoggedInUser user, ConfigurationRequestModel model);
        Task<string> GetConfigurationReportAsync(LoggedInUser loggedInUser, Task<ConfigurationResponseModel> configList, ConfigurationRequestModel model, string filename);
        string GetTemplateForConfigurationReport(string? configname);
        Task<string> GetHybridChageFileAsync(LoggedInUser user, PcFileRequestModel requestModel, string sheetName);
        Task<List<ReportServiceDetails>> GetReportServiceDetailsAsync(LoggedInUser loggedInUser, string? paygroup);
        Task<string> DownloadReportServiceDetails(LoggedInUser loggedInUser, int id);
        Task<(string?, string?)> GetStartAndEndTime(LoggedInUser loggedInUser, StartEndTimeRequestModel requestBody);
    }
}
