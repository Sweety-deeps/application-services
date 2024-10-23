using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PeriodChangePAYD
    {
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        //public DateTime? CreatedAt { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? RecordType { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? PayElementName { get; set; }
        public string? PayElementCode { get; set; }
        public string? ExportCode { get; set; }
        public string? RecurrentSchedule { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Percentage { get; set; }
        public string? PayrollFlag { get; set; }
        public string? Message { get; set; }
        public string? CostCenter { get; set; }
        public string? ModificationOwner { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string? Comments { get; set; }
        public int? PPN { get; set; }
        public DateTime? ImportDate { get; set; }
    }
}
