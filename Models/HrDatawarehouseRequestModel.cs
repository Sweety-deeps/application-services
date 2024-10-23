namespace Domain.Models
{
    public class HrDatawarehouseRequestModel
    {
        public string? paygroup { get; set; }
        public string? includeterminated { get; set; } 
        public string? payperiod { get; set; }
        public int? year { get; set; }

    }

    public class HrDatawarehouseResponseModel
    {
      
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? HireDate { get; set; }
        public string? LastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public string? Title { get; set; }
        public string? Gender { get; set; }
        public string? DateofBirth { get; set; }
        public string? BirthCity { get; set; }
        public string? BirthCountry { get; set; }
        public string? Nationality { get; set; }
        public string? TerminationDate { get; set; }
        public string? SeniorityDate { get; set; }
        public string? WorkEmail { get; set; }
        public string? PersonalEmail { get; set; }
        public string? WorkPhone { get; set; }
        public string? MobilePhone { get; set; }
        public string? HomePhone { get; set; }
        public string? MaritalStatus { get; set; }
        public string? EmployeeStatus { get; set; }
        public string? TerminationReason { get; set; }
        public string? PayClass { get; set; }
        public string? SalaryEffectiveDate { get; set; }
        public string? SalaryEndDate { get; set; }
        public string? TypeofSalary { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? AnnualPay { get; set; }
        public decimal? PeriodicSalary { get; set; }
        public decimal? NoofInstallments { get; set; }
        public decimal? DailyWorkingHours { get; set; }
        public string? TerminationPaymentDate { get; set; }
        public string? WorkContractEffectiveDate { get; set; } //workcontract
        public string? WorkContractEnddate { get; set; } //workcontract
        public string? HiringType { get; set; }
        public int? WorkingDaysPerWeek { get; set; }
        public string? WorkStartDate { get; set; }  //workcontract
        public string? WorkEndDate { get; set; }  //workcontract
        public string? JobChangeReason { get; set; }
        public string? PersonalJobTitle { get; set; }
        public string? Department { get; set; }
        public string? CostCenter { get; set; }
        public string? EmployeePackage { get; set; }
        public string? EmployeeType { get; set; }
        public string? Job { get; set; }
        public string? Location { get; set; }
        public string? PrimaryAssignment { get; set; }
        public string? OrgUnit1 { get; set; }
        public string? OrgUnit2 { get; set; }
        public string? OrgUnit3 { get; set; }
        public string? OrgUnit4 { get; set; }
        public string? AddressType { get; set; }
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? StreetAddress3 { get; set; }
        public string? StreetAddress4 { get; set; }
        public string? StreetAddress5 { get; set; }
        public string? StreetAddress6 { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? State { get; set; }
        public string? country { get; set; }
        public string? PostalCode { get; set; }
        public string? BankName { get; set; }
        public string? BankNumber { get; set; }
        public string? AccountType { get; set; }
        public string? AccountNumber { get; set; }
        public string? IbanCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? LocalClearingCode { get; set; }
        public string? BeneficiaryName { get; set; }
        public string? ClientName { get; set; }
        public string? payperiod { get; set; }
        public string? IncludeTerminated { get; set; }
        public int? year { get; set; }
    }
}
