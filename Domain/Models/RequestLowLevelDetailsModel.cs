using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RequestLowLevelDetailsModel
    {
        public int id { get; set; }
        public int requestdetailsid { get; set; } 
        public string? eventname { get; set; }
        public int? employeeid { get; set; }
        public string? failureentity { get; set; }
        public string? failurereason { get; set; }
        public int? processstatus { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string status { get; set; }
    }
}
