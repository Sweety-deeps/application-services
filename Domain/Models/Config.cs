namespace Domain.Models
{
    public class Config
	{
        public bool UtcEnabled { get; set; } = true;
        public string DefaultTimeZone { get; set; } = "Eastern Standard Time";
        public string S3PayslipBucketName { get; set; } = string.Empty;
        public string S3DataImportBucketName { get; set; } = string.Empty;
        public string S3ErrorDetailsBucketName { get; set; } = string.Empty;
        public string S3SerializedGpriBucketName { get; set; } = string.Empty;
        public string S3TempPayslipBucketName { get; set; } = string.Empty;
        public string S3ReportOutputBucketName { get; set; } = string.Empty;

    }
}
