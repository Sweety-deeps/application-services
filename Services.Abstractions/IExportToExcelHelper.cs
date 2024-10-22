using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IExportToExcelHelper : IUIPermissions
    {
        string GetGPRIReportData(LoggedInUser user, List<GPRI> _gpri, GPRIRequestModel _mdl);
        string GetPayPeriodRegisterReportData(LoggedInUser user, List<PayPeriodRegisterResponseModel> results, PayPeriodRegisterRequestModel _mdl);
        string GetHrDatawarehouseReportData(LoggedInUser user, List<HrDatawarehouseResponseModel> results, HrDatawarehouseRequestModel _mdl);
        string GetCSPFDatawarehouseReportData(LoggedInUser user, List<CSPFDatawarehouseResponseModel> results, CSPFDatawarehouseRequestModel _mdl);
        string GetPAYDDatawarehouseReportData(LoggedInUser user, List<PaydDatawarehouseResponseModel> results, PaydDatawarehouseRequestModel _mdl);
        string GetPeriodChangeFile(LoggedInUser user, List<PeriodChangeFileDataModel> results, PeriodChangeFileRequestModel _mdl);
        string GetPayElementReport(LoggedInUser user, List<PayElementResponseModel> results, PayElementRequestModel _mdl);
        string GetSystemUserReport(LoggedInUser user, List<SystemUserResponseModel> results, SystemUserRequestModel _mdl);
        string GetTransactionReport(LoggedInUser user, List<TransactionResponseModel> results, TransactionRequestModel _mdl);
        string GetVarianceReport(LoggedInUser user, List<VarianceResponseModel> results, VarianceRequestModel _mdl);
        string GetErrorLogReport(LoggedInUser user, List<ErrorLogResponseModel> results, ErrorLogRequestModel _mdl);
        
    }
}
