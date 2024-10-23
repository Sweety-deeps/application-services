using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PaySlipsDownloadModel
    {
        public int id { get; set; }
        public string FileId { get; set; }
        public string? PayGroupCode { get; set; }
        public string? LegalEntityCode { get; set; }
        public int? PayPeriod { get; set; }
        public string? EmpId { get; set; }

        public string? FileName { get; set; }
        public string PayDate { get; set; }
        public string PeriodStartDate { get; set; }
        public string PeriodEndDate { get; set; }
        public decimal? NetPay { get; set; }
        public decimal? GrossPay { get; set; }
        public string? DocumentID { get; set; }
        public string? Level1 { get; set; }
        public string? Message { get; set; }
        public string? UploadStatus { get; set; }
        public string? Status { get; set; }
        public DateTime? Queuetimestamp { get; set; }

    }
}
