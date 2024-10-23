using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class TransformedEmployeeSalaryModel
    {
        public int id { get; set; }
        public int requestid { get; set; }
        public string? entitystate { get; set; }
        public string? entrytype { get; set; }
        public string? employeeid { get; set; }
        public string? paygroup { get; set; }
        public string? effectivedate { get; set; }
        public string? enddate { get; set; }
        public string? payelementtype { get; set; }
        public string? payelementcode { get; set; }
        public string? payelementname { get; set; }
        public string? amount { get; set; }
        public string? percentage { get; set; }
        public string? payperiodnumber { get; set; }
        public string? payperiodnumbersuffix { get; set; }
        public string? paydate { get; set; }
        public string? overrides { get; set; }
        public string? payrollflag { get; set; }
        public string? modificationowner { get; set; }
        public string? modificationdate { get; set; }
        public string? errordetails { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
    }
}
