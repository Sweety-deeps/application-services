using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Payslips
{
    public class PostPayslipItem
    {
        public int PayslipId { get; set; }
        public string S3ObjectKey { get; set; }
        public PostPayslipMetadataItem PostPayslipMetadataItem { get; set; }
    }

    public class PostPayslipMetadataItem
    {
        public string FileName { get; set; }
        public string DocumentTypeXRefCode { get; set; }
        public string EntityTypeXRefCode { get; set; }
        public string Entity { get; set; }
        public string EmployeeXRefCode { get; set; }
        public string FileKey { get; set; }
        public string AdditionalData { get; set; }
    }

    public class PostPayslipAdditionalData
    {
        public string PayGroupXRefCode { get; set; }
        public string LegalEntity { get; set; }
        public string PayDate { get; set; }
        public string PeriodStartDate { get; set; }
        public string PeriodEndDate { get; set; }
        public string NetPay { get; set; }
        public string GrossPay { get; set; }
        public string Type { get; set; }
    }

    public class PostPayslipResult
    {
        public string Status { get; set; }
        public List<PostPayslipFilesProcessed> FilesProcessed { get; set; }
    }

    public class PostPayslipFilesProcessed
    {
        public int Index { get; set; } = -1;
        public string? DocumentGUID { get; set; }
        public string UploadStatus { get; set; }
        public string Message { get; set; }
        public int? PayslipId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Metadata { get; set; }
    }
}
