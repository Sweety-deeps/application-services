using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PeriodChangeBANK
    {
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        //public DateTime? CreatedAt { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? RecordType { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? BankName { get; set; }
        public string? BankNumber { get; set; }
        public string? AccountType { get; set; }
        public string? AccountNumber { get; set; }
        public string? IBANCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? LocalClearingCode { get; set; }
        public string? BeneficiaryName { get; set; }
        public string? BankSecondaryId { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? City { get; set; }
        public string? StateProvinceCanton { get; set; }
        public string? PostalCode { get; set; }
        public string? CountryCode { get; set; }
        public string? FundingMethod {  get; set; }
        public string? SplitBankingType { get; set; }
        public int? Priority { get; set; }
        public double? AmountOrPercentage { get; set; }
        public string? Comments { get; set; }
        public int? PPN { get; set; }
        public DateTime? ImportDate { get; set; }

    }
}
