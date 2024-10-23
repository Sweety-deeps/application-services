using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TaxAuthority
    {
        public int id { get; set; }
        public string code { get; set; }
        public string? namelocal { get; set; }
        public string? name { get; set; }
        public int? countryid { get; set; }
        public int? employeeliability { get; set; }
        public int? employerliability { get; set; }
        public string? comments { get; set; }
        public string? providercomments { get; set; }
        public string? status { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }

    }
}
