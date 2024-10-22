using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IS3Handling : IUIPermissions
    {
        public Task<(bool success, long fileSizeBytes)> UploadToS3(LoggedInUser user, string s3BucketName, string s3ObjectKey, string content);
        public Task<string> DownloadFromS3(LoggedInUser user, string s3BucketName, string s3ObjectKey);
        public Task<byte[]> DownloadDataImportFromS3(LoggedInUser user, string s3BucketName, string s3ObjectKey);
        public string GeneratePreSignedUrl(LoggedInUser user, string s3BucketName, string s3ObjectKey);
    }
}
