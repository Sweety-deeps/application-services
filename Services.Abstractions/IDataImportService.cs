using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IDataImportService : IUIPermissions
    {
        Task<List<DataImport>> GetDataImportAsync(LoggedInUser user, string paygroupCode);
        Task<DatabaseResponse> UploadDataImport(LoggedInUser user, DataImportRequestModel dataImportRequestModel);
        Task<string> DownloadErrorReport(LoggedInUser user, Guid? id);
        Task<string> DownloadErrorReportByEntityId(LoggedInUser user, int? id);
        Task<string> DownloadErrorReportAsync(LoggedInUser user,string? url, ErrorDetailsRequestModel requestModel, string? filename);
        Task PublishToDataImportAsync(LoggedInUser user, int entityId, string entityName, string paygroupCode, string? s3ObjectId, string? additionalJson, int delay, long filesize);
        Task<string> GetFileIdByEntityIdOrGuid(LoggedInUser loggedInUser, int? entityId, Guid? guid);
        Task<BaseResponseModel<string>> DownloadDataImport(LoggedInUser user, Guid requestId);
    }
}
