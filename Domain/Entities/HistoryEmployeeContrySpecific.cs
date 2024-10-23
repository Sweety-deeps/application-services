using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HistoryEmployeeContrySpecific
    {
        public int id { get; set; }
        public string entitystate { get; set; }
        public int contryspecifictableid { get; set; }
        public string employeeid { get; set; }
        public int paygroupid { get; set; }
        public DateTime effectivedate { get; set; }
        public DateTime? endate { get; set; }
        public string? country { get; set; }
        public string? fieldname { get; set; }
        public string? fieldvalue { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public bool changeprocessed { get; set; }
        public int? requestid { get; set; }
    }
}
