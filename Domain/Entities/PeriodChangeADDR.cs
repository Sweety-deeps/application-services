using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PeriodChangeADDR
    {
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        //public DateTime? CreatedAt { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? RecordType { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
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
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Comments { get; set; }
        public int? PPN { get; set; }
        public DateTime? ImportDate { get; set; }
    }
}
