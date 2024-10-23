using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PeriodChangeCSPF
    {
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        //public DateTime? CreatedAt { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? RecordType { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Category { get; set; }
        public string? CsFldName { get; set; }
        public string? Value { get; set; }
        public int? PPN { get; set; }
        public DateTime? ImportDate { get; set; }
    }
}
