using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Models
{
    public class TransactionRequestModel
    {
        public string? clientname { get; set; }
        public string? filteroption { get; set; }
        public string? paygroup { get; set; }
        public string? country { get; set; }
        public string? ppoption { get; set; }
        public string?  payperiod { get; set; }
        public string? startpp { get; set; }
        public string? endpp { get; set; }
    }

    public class TransactionResponseModel
    {
        public string? ClientName {  get; set; }
        public string? PayGroup { get; set; }
        
        //public string StartPP {  get; set; }
        //public string EndPP { get; set; }
        public string? PayrollYear { get; set; }
        public string? PayPeriod { get; set; }
        public string? PPStartDate { get; set; }
        public string? PPEndDate { get; set; }
        public string? PayDate { get; set; }
        public string? LastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Months { get; set; }
        public string EmployeeID { get; set; }
        public string? PayElementNameLocal { get; set; }
        public string? PayElementName { get; set; }
        public string? PayElementCode { get; set; }
        public string? ExportCode { get; set; }
        public string? ElementType { get; set; }
        public string? Department { get; set; }
        public string? GLCreditCode { get; set; }
        public string? GLDebitCode { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public decimal? Amount { get; set; }
        public string? TransactionCurrency { get; set; }
        public string? CostCenter { get; set; }
        public string? Orglevel1 { get; set; }
        public string? Orglevel2 { get; set; }
        public string? Orglevel3 { get; set; }
        public string? Orglevel4 { get; set; }
        public string? Country { get; set; }
        public int? Countryid { get; set; }

    }
}
