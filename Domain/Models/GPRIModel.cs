using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class GPRIModel
    {
        public int id { get; set; }
        public string? fileid { get; set; }
        public string payrollformat{ get; set; }
        public int paygroupid { get; set; }
        public string paygroup { get; set; }
        public int payperiod { get; set; }
        public string isoffcycle { get; set; }
        public string offcycletype { get; set; }
        public string? overwritepaydate { get; set; }
        public string offcyclesuffix { get; set; }
        public string isoverride { get; set; }
        public int sheetnumber { get; set; }
        public string payrollresults { get; set; }
        public string? employeerow { get; set; }
        public string? payelementrow { get; set; }
        public string? payelementcolumn { get; set; }
        public string? employeecolumn { get; set; }
        public string? receivedtime { get; set; }
        public string? processedtime { get; set; }
        public string? completiontime { get; set; }
        public string? createdby { get; set; }
        public string? createdat { get; set; }
        public string? modifiedby { get; set; }
        public string? modifiedat { get; set; }
        public string? status { get; set; }
        public string? xml { get; set; }
        public string? payslipstatus { get; set; }
        public int? noofees { get; set; }
        public int? noofpayslips { get; set; }
        public string? nextstep { get; set; }
        public int? year { get; set; }
        public string? sendgpriresult { get; set; }
        public string? outboundformat { get; set; }
        public string? inboundformat { get; set; }
        public string? s3objectid { get; set; }
    }
}
