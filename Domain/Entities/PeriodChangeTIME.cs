using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities
{
    public class PeriodChangeTIME
    {
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        //public DateTime? CreatedAt { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? LocalPayCode { get; set; }
        public string? PartnerPayCode { get; set; }
        public string? PayElementName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? BusinessDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public string? CostCenter { get; set; }
        public string? Positon { get; set; }
        public string? Location { get; set; }
        public string? Project { get; set; }
        public string? Retroactive { get; set; }
        public int? PPN { get; set; }
        public DateTime? ImportDate { get; set; }
        public string? CustomizableField0 { get; set; }
        public string? CustomizableField1 { get; set; }
        public string? CustomizableField2 { get; set; }
        public string? CustomizableField3 { get; set; }
        public string? CustomizableField4 { get; set; }
        public string? CustomizableField5 { get; set; }
        public string? CustomizableField6 { get; set; }
        public string? CustomizableField7 { get; set; }
        public string? CustomizableField8 { get; set; }
        public string? CustomizableField9 { get; set; }
        
    }
}
