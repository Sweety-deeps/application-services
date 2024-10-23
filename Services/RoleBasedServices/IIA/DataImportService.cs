﻿using Amazon.S3;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.Extensions.Logging;
using Persistence;
using Services.Abstractions;
using Services.Helpers;

namespace Services.IIA
{
    public class DataImportService : Services.PowerUser.DataImportService
    {
        public DataImportService(ILogger<DataImportService> logger, IS3Handling s3Handling, ISQSHandling sQSHandling, AppDbContext appDbContext, Config config, IDateTimeHelper dateTimeHelper, HttpClient httpClient,IAmazonS3 s3Client) : base(logger, s3Handling, sQSHandling, appDbContext, config, dateTimeHelper, httpClient, s3Client)
        {
        }
        public override Task<DatabaseResponse> UploadDataImport(LoggedInUser user, DataImportRequestModel dataImportRequestModel)
        {
            if (user.Paygroups.Any(upa => upa.payGroupCode == dataImportRequestModel.payGroup))
            {
                return base.UploadDataImport(user, dataImportRequestModel);
            }
            throw new UnauthorizedAccessException();
        }
        public override bool CanDelete(LoggedInUser user)
        {
            return false;
        }
    }
}