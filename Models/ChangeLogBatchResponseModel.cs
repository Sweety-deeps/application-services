using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ChangeLogBatchResponseModel
    {
        public long? id { get; set; }
        public DateTime? starttime { get; set; }
        public DateTime? finishtime { get; set; }
        public string status { get; set; }
        public string? errorlog { get; set; }
        public bool scheduled { get; set; }
        public string triggerdby { get; set; }
        public string createdby { get; set; }
        public DateTime? createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public decimal? progress { get; set; }
    }
}
