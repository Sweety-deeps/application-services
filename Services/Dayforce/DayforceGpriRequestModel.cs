namespace Services.Dayforce
{
    public class DayforceGpriRequestModel
	{
		public string GpriFilePayload { get; set; }
		public string FileId { get; set; }
	}

	public class PaygroupBaseGpriModel
	{
		public int PaygroupId { get; set; }
		public string OutboundFormat { get; set; }
        public string? GpriSftpFolder { get; set; }
        public string? PayslipSftpFolder { get; set; }
		public string CountryCode { get; set; }
		public string LegalEntityCode { get; set; }
		public string PaygroupCode { get; set; }
        public string? ApiClientId { get; set; }
        public string? EncryptedApiUserName { get; set; }
        public string? EncryptedApiPassword { get; set; }
		public string? UrlPrefix { get; set; }
		public bool IsTestSftp { get; set; }
    }
}
