using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RequestHighLevelDetails
    {
        public int id { get; set; }
        public int requestdetailsid { get; set; }
        public string paygroup { get; set; }
        public int receivedcount { get; set; }
        public int? processed { get; set; }
        public int? sucesscount { get; set; }
        public int? failurecount { get; set; }
        public string createdby { get; set; }
        public DateTime statedtime { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? endtime { get; set; }
        public int? executiontime { get; set; }
        public string? status { get; set; }
    }
}
