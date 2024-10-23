using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class GPRI
    {
        public int id { get; set; }
        public string fileid { get; set; }
        public int paygroupid { get; set; }
        public int payperiod { get; set; }
        public string isoffcycle { get; set; }
        public string offcycletype { get; set; }
        public DateTime? overwritepaydate { get; set; }
        public string offcyclesuffix { get; set; }
        public string isoverride { get; set; }
        public int sheetnumber { get; set; }
        public int? employeerow { get; set; }
        public int? payelementrow { get; set; }
        public int? payelementcolumn { get; set; }
        public int? employeecolumn { get; set; }
        public DateTime? receivedtime { get; set; }
        public DateTime? processedtime { get; set; }
        public DateTime? completiontime { get; set; }
        public string? modifiedby { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdat { get; set; }

        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public string? payslipstatus { get; set; }
        public int? noofees { get; set; }
        public int? noofpayslips { get; set; }
        public string? nextstep { get; set; }
        public int? payperiodyear {  get; set; }
        public string? sendgpriresult { get; set; }
    }
}
