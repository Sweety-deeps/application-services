using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PayElementRequestModel
    {
        public string? clientname {  get; set; }
        public string? paygroup {  get; set; }
        public string? type {  get; set; }
    }

    public class PayElementResponseModel
    {
        // public string? format { get; set; }
        public string? PayGroup { get; set; }
        public string? PayElementNameLocal { get; set; }
        public string? PayElementName { get; set; }
        public string? PayElementCode { get; set; }
        public string? ExportCode { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? Format { get; set; }
        public string? GLCreditCode { get; set; }
        public string? GLDebitCode { get; set; }
        public string? ClientReported { get; set; }
        public string? PayslipPrint { get; set; }
        public string? Comments { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? ContributesToNetPay { get; set; }
        public string? IsEmployerTax { get; set; }
        public string? IsEmployeeDeduction { get; set; }
        public string? Clientname { get; set; }
       // public string? type { get; set; }


    }
}
								
