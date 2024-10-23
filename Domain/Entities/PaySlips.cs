using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class PaySlips
    {
        public int id { get; set; }
        public string fileid { get; set; }
        public string s3objectkey { get; set; }
        public string? paygroupxrefcode { get; set; }
        public string? legalentityxrefcode { get; set; }
        public int? payperiod { get; set; }
        public string? employeexrefcode { get; set; }
        public string? filename { get; set; }
        public DateTime? paydate { get; set; }
        public DateTime? payperiodstart { get; set; }
        public DateTime? payperiodend { get; set; }
        public decimal? contributetonetpay { get; set; }
        public decimal? itemamount { get; set; }
        public string? requestid { get; set; }
        public string? documentid { get; set; }
        public string? message { get; set; }
        public string? partneruploadstatus { get; set; }
        public string? payslipstatus { get; set; }
        public string? status { get; set; }
        public string? requeststatus { get; set; }
        public string? timestamp { get; set; }
        public DateTime? queuetimestamp { get; set; }
        public string? requestjson { get; set; }
        public string? responsejson { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        [Column("metadatajson", TypeName = "jsonb")]
        public string? MetadataJson { get; set; }
        [Column("context")]
        public string? Context { get; set; }
        [Column("apierrormessage")]
        public string? ApiErrorMessage { get; set; }
    }
}
