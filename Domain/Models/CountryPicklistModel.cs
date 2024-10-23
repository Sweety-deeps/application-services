using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CountryPicklistModel
    {
        public int id { get; set; }
        public int countryid { get; set; }
        public string tablename { get; set; }
        public string columnname { get; set; }
        public string jsonvalue { get; set; }
        public string displayvalue { get; set; }
        public string outputvalue { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
    }
}
