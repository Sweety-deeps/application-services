using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TransformedEmployeeSalary
    {
        public int id { get; set; }
        public int requestid { get; set; }
        public string? entitystate { get; set; }
        public string? employeeid { get; set; }
        public string? paygroup { get; set; }
        public string? effectivedate { get; set; }
        public string? enddate { get; set; }
        public string? typeofsalary { get; set; }
        public string? hourlyrate { get; set; }
        public string? annualpay { get; set; }
        public string? periodicsalary { get; set; }
        public string? normalweeklyhours { get; set; }
        public string? noofinstallments { get; set; }
        public string? errordetails { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string status { get; set; }
    }
}
