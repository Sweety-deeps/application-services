using Amazon.S3.Model;
using Amazon.S3;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using Domain.Models.Users;
using Services.Helpers;

namespace Services
{
    public class S3Handling : IS3Handling
    {
        protected readonly ILogger<S3Handling> _logger;
        protected readonly IAmazonS3 _s3Client;
        private readonly IDateTimeHelper _dateTimeHelper;

        public S3Handling(ILogger<S3Handling> logger, IAmazonS3 s3Client,IDateTimeHelper dateTimeHelper)
        {
            _logger = logger;
            _s3Client = s3Client;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<(bool success, long fileSizeBytes)> UploadToS3(LoggedInUser user, string s3BucketName, string s3ObjectKey, string content)
        {
            byte[] excelBytes = Convert.FromBase64String(content);
            try
            {
                using (var memoryStream = new MemoryStream(excelBytes))
                {
                    PutObjectRequest request = new PutObjectRequest
                    {
                        BucketName = s3BucketName,
                        Key = s3ObjectKey,
                        InputStream = memoryStream,
                        ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    };
                    PutObjectResponse response = await _s3Client.PutObjectAsync(request);
                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // Get the object metadata to retrieve the file size
                        var metadata = await _s3Client.GetObjectMetadataAsync(s3BucketName, s3ObjectKey);
                        return (true, metadata.ContentLength); // Return file size in bytes
                    }
                    return (false, 0);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Unknown encountered on server. Message:'{e.Message}' when writing an object");
                return (false, 0);
            }
        }

        public async Task<string> DownloadFromS3(LoggedInUser user, string s3BucketName, string s3ObjectKey)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = s3BucketName,
                    Key = s3ObjectKey
                };
                
                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                {
                    using (System.IO.Stream responseStream = response.ResponseStream)
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(responseStream))
                        {
                            string content = reader.ReadToEnd();
                            return content;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"Unknown encountered on server. Message:'{e.Message}' when reading an object");
                return null;
            }
        }

        public async Task<byte[]> DownloadDataImportFromS3(LoggedInUser user, string s3BucketName, string s3ObjectKey)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = s3BucketName,
                    Key = s3ObjectKey
                };

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                {
                    using (System.IO.Stream responseStream = response.ResponseStream)
                    {
                        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                        {
                            responseStream.CopyTo(memoryStream);
                            byte[] fileBytes = memoryStream.ToArray();
                            return fileBytes;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"Unknown encountered on server. Message:'{e.Message}' when reading an object");
                return null;
            }
        }

        public virtual string GeneratePreSignedUrl(LoggedInUser user, string s3BucketName, string s3ObjectKey)
        {
            try
            {
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = s3BucketName,
                    Key = s3ObjectKey,
                    Expires = _dateTimeHelper.GetDateTimeNow().AddMinutes(5),
                };

                var url = _s3Client.GetPreSignedURL(request);
                return url;
            }
            catch (Exception e)
            {
                _logger.LogError($"Unknown encountered on server. Message:'{e.Message}' when generating pre-signed URL");
                return null;
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
