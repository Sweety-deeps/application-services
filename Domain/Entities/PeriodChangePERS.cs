using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PeriodChangePERS
    {
        public string? Paygroup { get; set; }
        public string? EmployeeID { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? RecordType { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? DateOfLeaving { get; set; }
        public string? LastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public string? Title { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? BirthCity { get; set; }
        public string? BirthCountry { get; set; }
        public string? Nationality { get; set; }
        public string? TerminationReason { get; set; }
        public DateTime? SeniorityDate { get; set; }
        public string? WorkEmail { get; set; }
        public string? PersonalEmail { get; set; }
        public string? WorkPhone { get; set; }
        public string? MobilePhone { get; set; }
        public string? HomePhone { get; set; }
        public DateTime? CompanyStartDate { get; set; }
        public DateTime? EffectiveStart {  get; set; }
        public string? Comments { get; set; }
        public int? PPN { get; set; }
        public DateTime? ImportDate { get; set; }

    }
}
