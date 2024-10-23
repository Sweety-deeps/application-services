using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PeriodChangeStarters
    {
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        //public DateTime? CreatedAt { get; set; }
        public string? EmployeeNumber { get; set; }
        public DateTime? HireDate { get; set; }
        public string? PayPeriod { get; set; }
        public string? LastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public string? FullName { get; set; }

    }
}
