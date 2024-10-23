using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PeriodChangeSLRY 
    {
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        //public DateTime? CreatedAt { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? RecordType { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? SalaryEffectiveDate { get; set; }
        public string? TypeofSalary { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? AnnualPay { get; set; }
        public decimal? PeriodicSalary { get; set; }
        public decimal? NoofInstallments { get; set; }
        public string? Comments { get; set; }
        public int? PPN { get; set; }
        public DateTime? ImportDate { get; set; }
    }
}
