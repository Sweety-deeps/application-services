using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PayslipRequestJsonModal
    {
        public string? FileID { get; set; }
        public string? FileName { get; set; }
        public string? DocumentTypeXRefCode { get; set; }
        public string? EntityTypeXRefCode { get; set; }
        public string? Entity { get; set; }
        public string? EmployeeXRefCode { get; set; }
        public string? FileKey { get; set; }
        //public string? Comment { get; set; }
        public AdditionalData AdditionalData { get; set; }
    }

    public class AdditionalData
    {
        public string? PayGroupXRefCode { get; set; }
        public string? LegalEntity { get; set; }
        public string? PayDate { get; set; }
        public string? PeriodStartDate { get; set; }
        public string? PeriodEndDate { get; set; }
        public string? NetPay { get; set; }
        public string? GrossPay { get; set; }
        public string? Type { get; set; }

    }
}
