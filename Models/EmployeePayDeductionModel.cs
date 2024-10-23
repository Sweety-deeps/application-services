using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class EmployeePayDeductionModel
    {
        public int id { get; set; }
        public string? entitystate { get; set; }
        public string? entrytype { get; set; }
        public string employeeid { get; set; }
        public int paygroup { get; set; }
        public DateTime effectivedate { get; set; }
        public DateTime? enddate { get; set; }
        public string payelementtype { get; set; }
        public string? payelementcode { get; set; }
        public string? payelementname { get; set; }
        public float? amount { get; set; }
        public float? percentage { get; set; }
        public float? payperiodnumber { get; set; }
        public float? payperiodnumbersuffix { get; set; }
        public DateTime? paydate { get; set; }
        public string? overrides { get; set; }
        public string? payrollflag { get; set; }
        public string? modificationowner { get; set; }
        public DateTime? modificationdate { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
    }
}
