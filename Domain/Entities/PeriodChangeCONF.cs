using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PeriodChangeCONF
    {
        public string Paygroup { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Country { get; set; }
        public DateTime? IssueDate { get; set; }
        public string? PlaceOfIssue { get; set; }
        public DateTime? ExpiryDate { get; set; }


    }
}
