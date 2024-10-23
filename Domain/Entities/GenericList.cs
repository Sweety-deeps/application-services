using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class GenericList
    {
        public int id { get; set; }
        public string tablename { get; set; }
        public string columnname { get; set; }
        public string lookupvalue { get; set; }
        public string? createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set;}
        public string? status { get; set; }

    }
}
