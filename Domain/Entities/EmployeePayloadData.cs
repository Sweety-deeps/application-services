using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class EmployeePayloadData
    {
        public List<EmployeePayload> EmployeesPayload { get; set; }

    }

    public partial class EmployeePayload
    {
        public string? EmployeeXrefCode { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public List<EmploymentStatuses>? employmentStatuses { get; set; }
        public List<WorkAssignments>? workAssignments { get; set; }
        public List<WorkContracts>? workContracts { get; set; }
        public List<Addresses>? addresses { get; set; }

    }

    public partial class EmploymentStatuses
    {
        public int Id { get; set; }
        public DateTime EffectiveStart { get; set; }
        public DateTime EffectiveEnd { get; set; }
        public string EmploymentStatusReason { get; set; }
        public string EmploymentStatus { get; set; }
        public string PayClass { get; set; }
        public string PayType { get; set; }
        public string PayGroup { get; set; }
        public double BaseRate { get; set; }
        public double BaseSalary { get; set; }
        public double NormalWeeklyHours { get; set; }
    }

    public partial class WorkAssignments
    {
        public int Id { get; set; }
        public DateTime EffectiveStart { get; set; }
        public DateTime EffectiveEnd { get; set; }
        public string JobStatusReason { get; set; }
        public string Job { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Position { get; set; }
        public bool IsPrimary { get; set; }
    }

    public partial class WorkContracts
    {
        public string WorkContractName { get; set; }
        public int AverageNumOfDays { get; set; }
        public double BaseHours { get; set; }
        public double BaseComplementaryHours { get; set; }
        public double ContractWorkPercent { get; set; }
        public double FullTimeHours { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public partial class Addresses
    {
        public DateTime? EffectiveStart { get; set; }
        public DateTime? AverageNumOfDays { get; set; }
        public string? AddressType { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? Address5 { get; set; }
        public string? Address6 { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? County { get; set; }
        public string? State { get; set; }
        public bool? IsPayrollMailing { get; set; }
    }
}
