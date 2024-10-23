using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PaySlipsModel
    {

        //public int id { get; set; }
        //public string FileId { get; set; }

        //public string? PayGroupCode { get; set; }
        //public string? LegalEntityCode { get; set; }
        //public int? PayPeriod { get; set; }
        //public string? EmpId { get; set; }

        //public string? FileName { get; set; }
        //public DateTime? PayDate { get; set; }
        //public DateTime? PeriodStartDate { get; set; }
        //public DateTime? PeriodEndDate { get; set; }
        //public int? NetPay { get; set; }
        //public int? GrossPay { get; set; }
        //public string? RequestID { get; set; }
        //public string? Level1 { get; set; }
        //public string? Message { get; set; }
        //public string? FTPSMessage{ get; set; }
        //public string? payslipstatus { get; set; }
        //public string? status { get; set; }
        //public string? requeststatus { get; set; }
        //public string? timestamp { get; set; }
        //public DateTime queuetimestamp { get; set; }
        //public string? requestjson { get; set; }
        //public string? responsejson { get; set; }
        //public string? createdby { get; set; }
        //public DateTime? createdat { get; set; }
        //public string? modifiedby { get; set; }
        //public DateTime? modifiedat { get; set; }


        public int id { get; set; }
        public string fileid { get; set; }

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
        public string? level1 { get; set; }
        public string? message { get; set; }
        public string? ftpsstatus { get; set; }
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
        public bool MetadataExists { get; set; }
        public string? Context { get; set; }
        public string? ApiErrorMessage { get; set; }

    }
}
