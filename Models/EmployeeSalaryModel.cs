using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class EmployeeSalaryModel
    {
        public int id { get; set; }
        public string? entitystate { get; set; }
        public string employeeid { get; set; }
        public int paygroupid { get; set; }
        public DateTime effectivedate { get; set; }
        public DateTime? enddate { get; set; }
        public string? typeofsalary { get; set; }
        public float? hourlyrate { get; set; }
        public float? annualpay { get; set; }
        public float? periodicsalary { get; set; }
        public float? normalweeklyhours { get; set; }
        public float? noofinstallments { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
    }
}
