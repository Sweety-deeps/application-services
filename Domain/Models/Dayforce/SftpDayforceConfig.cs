namespace Domain.Models.Dayforce
{
    public class SftpDayforceConfig
    {
        public string SftpUploadUrl { get; set; }
        public string SftpUsername { get; set; }
        public string SftpPassword { get; set; }
        public string SftpPort { get; set; }
    }
    //ToDo: instead of two model use Using Named Options Ioptions in DI to load same Type different values Injection.
    public class TestSftpDayforceConfig
    {
        public string SftpUploadUrl { get; set; }
        public string SftpUsername { get; set; }
        public string SftpPassword { get; set; }
        public string SftpPort { get; set; }
    }
}
