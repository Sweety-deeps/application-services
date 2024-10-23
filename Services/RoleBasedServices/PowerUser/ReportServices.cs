using Services.Abstractions;
using Microsoft.Extensions.Logging;
using Persistence;
using Domain.Entities;
using System.Text.Json;
using System.Reflection;
using Domain.Models;
using Amazon.S3;
using Amazon.S3.Model;
using ClosedXML.Excel;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Npgsql;
using Services.Helpers;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DocumentFormat.OpenXml.InkML;

namespace Services.PowerUser
{
    public class ReportServices : Services.ReportServices
    {
        public ReportServices(AppDbContext appDbContext, IAmazonS3 s3Client, ILogger<ReportServices> logger, ISelectListHelper selectListHelper, IDateTimeHelper dateTimeHelper, IReportServiceHelper reportServiceHelper, Config config) : base(appDbContext, s3Client, logger, selectListHelper, dateTimeHelper, reportServiceHelper, config)
        {
        }
    }
}
