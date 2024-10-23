using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HistoryEmployeeSalary
    {
        public int id { get; set; }
        public int salarytableid { get; set; }
        public string? entitystate { get; set; }
        public string employeeid { get; set; }
        public int paygroupid { get; set; }
        public DateTime effectivedate { get; set; }
        public DateTime? enddate { get; set; }
        public string? typeofsalary { get; set; }
        public decimal? hourlyrate { get; set; }
        public decimal? annualpay { get; set; }
        public decimal? periodicsalary { get; set; }
        public decimal? normalweeklyhours { get; set; }
        public decimal? noofinstallments { get; set; }
        //public string? comments { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public DateTime? fixedenddate { get; set; }
        public bool changeprocessed { get; set; }
        public string? comments { get; set; }
        public DateTime? salaryeffectivedate { get; set; }
        public int? requestid { get; set; }

    }
}
