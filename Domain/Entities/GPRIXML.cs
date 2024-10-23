using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class GPRIXML
    {
        public int id { get; set; }
        public int gpritableid { get; set; }
        public string? gprixml { get; set; }
        public string? createdby { get; set; }
        public string? createdat { get; set; }
        public string? modifiedby { get; set; }
        public string? modifiedat { get; set; }
        public string? s3objectid { get; set; }
    }
}
