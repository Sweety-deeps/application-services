using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class VarianceRequestModel
    {
        public string? clientname {  get; set; }
        public string? paygroup {  get; set; }
        public int? payperiod { get; set; }
        public int? year { get; set; }
    }

    public class VarianceResponseModel
    {
        //public string? ClientName { get; set; }
        //public string? Type { get; set; }
        public string? PayGroup { get; set; }
        public int? PayPeriod { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string EmpolyeeID { get; set; }
        public string PayElementNameLocal { get; set; }
        public string PayElementName {  get; set; }
        public string PayElementCode { get; set; }
        public string? ExportCode { get; set; }
        public decimal OldAmount { get; set; }
        public decimal NewAmount { get; set; }
        public decimal VarianceAmount { get; set; }
        public decimal VariancePercentage { get; set; }
        public string? OldPeriod { get; set;}
        public string? NewPeriod { get; set;}
    }

    public class VariancePayPeriodDetails
    { 
        public int PayPeriod { get; set; }
        public int? PayPeriodYear { get; set; }
    }
}
