using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StagingEmployeeBank
    {
        public int id { get; set; }
        public int requestid { get; set; }
        public string? entitystate { get; set; }
        public string? employeeid { get; set; }
        public string? paygroup { get; set; }
        public string? effectivedate { get; set; }
        public string? enddate { get; set; }
        public string? bankname { get; set; }
        public string? accounttype { get; set; }
        public string? accountnumber { get; set; }
        public string? ibancode { get; set; }
        public string? swiftcode { get; set; }
        public string? localclearingcode { get; set; }
        public string? amountorpercentage { get; set; }
        public string? beneficiaryname { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
    }
}
