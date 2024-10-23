using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PayPeriodRegisterRequestModel
    {
        public int? gpriId {  get; set; }
        public string? paygroup { get; set; }
        public string? startpp { get; set; }
        public string? endpp { get; set; }
        public int? year { get; set; }
    }

    public class PayPeriodRegisterResponseModel
    {
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        public string? EmployeeNumber { get; set; }
        public int? PayPeriod { get; set; }
        public string? Offcycle { get; set; }
        public string? HireDate { get; set; }
        public string? TerminationDate { get; set; }
        public string? LastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public Decimal? EREEEarnings { get; set; }
        public Decimal? EEERDeductions { get; set; }
        public Decimal? EEOTEEContributions { get; set; }
        public Decimal? EROTERContributions { get; set; }
        public Decimal? NetPay { get; set; }
        public Decimal? TotalDisbursement { get; set; }
        public Decimal? EmployerCost { get; set; }
        public Decimal? TotalDeductions { get; set; }
        public Decimal? TotalGrossPay { get; set; }
    }


    public class PayPeriodRegistorDBValuesDetail
    {
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        public string? EmployeeNumber { get; set; }
        public int? PayPeriod { get; set; }
        public string? Offcycle { get; set; }
        public string? HireDate { get; set; }
        public string? TerminationDate { get; set; }
        public string? LastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public string? ItemType { get; set; }
        public decimal? ItmeAmount { get; set; }
        public string? Froms { get; set; }
        public string? Tos { get; set; }
        public string? PECode { get; set; }
        public string? PEName { get; set; }
    }
    public class PayPeriodRegisterDetailResponseModel
    {
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        public string? EmployeeNumber { get; set; }
        public int? PayPeriod { get; set; }
        public string? Offcycle { get; set; }
        public string? HireDate { get; set; }
        public string? TerminationDate { get; set; }
        public string? LastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public List<EmployerToEmployee> EmployerToEmployee { get; set; }
        public List<EmployeeToEmployer>? EmployeeToEmployer { get; set; }
        public List<EmployerToOther>? EmployerToOther { get; set; }
        public List<EmployeeToOther>? EmployeeToOther { get; set; }
        public Decimal? NetPay { get; set; }
        public Decimal? TotalDisbursement { get; set; }
        public Decimal? EmployerCost { get; set; }
        public Decimal? TotalDeductions { get; set; }
        public Decimal? TotalGrossPay { get; set; }

    }

    public class EmployerToEmployee
    { 
        public string code { get; set; }
        public string name { get; set; }   
        public string displayname { get; set; }
        public Decimal? value { get; set; }
    }
    public class EmployeeToEmployer
    {
        public string code { get; set; }
        public string name { get; set; }
        public string displayname { get; set; }
        public Decimal? value { get; set; }
    }
    public class EmployerToOther
    {
        public string code { get; set; }
        public string name { get; set; }
        public string displayname { get; set; }
        public Decimal? value { get; set; }
    }
    public class EmployeeToOther
    {
        public string code { get; set; }
        public string name { get; set; }
        public string displayname { get; set; }
        public Decimal? value { get; set; }
    }
}

