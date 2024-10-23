using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ErrorlogModel
    {
        public int id { get; set; }
        public int? requestdetailsid { get; set; }
        public string? employeeid { get; set; }
        public int? paygroupid { get; set; }
        public string? eventtype { get; set; }
        public string? entitystate { get; set; }
        public string? log { get; set; }
        public string? tablename { get; set; }
        public string? field { get; set; }
        public string? description { get; set; }
        public string? type { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string status { get; set; }
    }
}
