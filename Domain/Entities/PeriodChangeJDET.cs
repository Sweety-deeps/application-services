using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PeriodChangeJDET
    {
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        //public DateTime? CreatedAt { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? RecordType { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? SeniorityDate {  get; set; }
        public string? TerminationReason {  get; set; }
        public string? PersonalJobTitle { get; set; }
        public string? EmployeeStatus { get; set; }
        public string? EmploymentStatus { get; set; }
        public string? EmployeePackage { get; set; }
        public string? EmployeeType { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public string? Positions { get; set; }
        public string? WorkLocationCity { get; set; }
        public string? WorkLocationState { get; set; }
        public string? CostCenter { get; set; }
        public string? HiringType { get; set; }
        public string? PayClass { get; set; }
        public string? Job { get; set; }
        public string? OrgUnit1 { get; set; }
        public string? OrgUnit2 { get; set; }
        public string? OrgUnit3 { get; set; }
        public string? OrgUnit4 { get; set; }
        public decimal? DailyWorkingHours { get; set; }
        public DateTime? TerminationPaymentDate { get; set; } 
        public string? PrimaryAssignment { get; set; }
        public int? WorkingDaysPerWeek { get; set; }
        public DateTime? WorkStartDate { get; set; }
        public DateTime? WorkEndDate { get; set; }
        public string? Comments { get; set; }
        public int? PPN { get; set; }
        public DateTime? ImportDate { get; set; }

    }
}
