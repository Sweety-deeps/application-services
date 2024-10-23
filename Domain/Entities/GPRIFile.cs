using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class GPRIFile
    {
        public int id { get; set; }
        public int gpritableid { get; set; }
        public byte[] payrollfile { get; set; }        
        public string? createdby { get; set; }
        public DateTime? createdat { get; set; }
    }
}
