namespace Services.Dayforce
{
    public class DayforcePostPayslipListRequest
    {
        public IList<DayforcePostPayslipRequest> FileData { get; set; }
        public string Metadata { get; set; }
        public string FileId { get; set; }
        public int GpriId { get; set; }
        public string OutboundFormat { get; set; } = "JSON";
        public string PaygroupCode { get; set; }
        public string CountryCode { get; set; }
        public string LegalEntityCode { get; set; }
        public string? PayslipSftpFolder { get; set; }
        public string? ApiClientId { get; set; }
        public string? EncryptedApiUserName { get; set; }
        public string? EncryptedApiPassword { get; set; }
        public string? urlPrefix { get; set; }
        public async Task DisposeAsync()
        {
            foreach (var item in FileData)
            {
                await item.DisposeAsync();
            }
        }
    }

    public class DayforcePostPayslipRequest
    {
        public string FileKey { get; set; }
        public string FileName { get; set; }
        public MemoryStream File { get; set; }
        public string S3ObjectKey { get; set; }

        public async Task DisposeAsync()
        {
            if (File != null)
            {
                await File.DisposeAsync();
            }
        }
    }
}
