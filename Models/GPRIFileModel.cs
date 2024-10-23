using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class GPRIFileModel
    {
        public int? id { get; set; }
        public int? gpritableid { get; set; }
        public byte payrollfile { get; set; }
        public string? createdby { get; set; }
        public string? createdat { get; set; }
    }
}
