using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Services.Abstractions;

namespace Services.UnAuthorized
{
    public class DataImportService : IDataImportService
    {
        public bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public bool CanView(LoggedInUser user)
        {
            return false;
        }
        public Task<string> DownloadErrorReport(LoggedInUser user, Guid? id)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<string> DownloadErrorReportByEntityId(LoggedInUser user, int? id)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<string> GetFileIdByEntityIdOrGuid(LoggedInUser user, int? entityId, Guid? guid)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<string> DownloadErrorReportAsync(LoggedInUser user, string? url, ErrorDetailsRequestModel requestModel, string filename)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<List<DataImport>> GetDataImportAsync(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public Task PublishToDataImportAsync(LoggedInUser user, int entityId, string entityName, string paygroupCode, string? s3ObjectId, string? additionalJson, int delay, long filesize)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<DatabaseResponse> UploadDataImport(LoggedInUser user, DataImportRequestModel dataImportRequestModel)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<BaseResponseModel<string>> DownloadDataImport(LoggedInUser user, Guid requestId)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
