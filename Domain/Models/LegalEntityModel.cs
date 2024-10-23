using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class LegalEntityModel
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int clientid { get; set; }
        public int paygroupid { get; set; }
        public string createdby { get; set; }
        public DateTime? createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public string clientcode { get; set; }
        public int noofpaygroup { get; set; }
    }

    public class LegalEntityBaseModel
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int clientid { get; set; }
        public string createdby { get; set; }
        public DateTime? createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public string clientcode { get; set; }
        public int noofpaygroup { get; set; }
    }
}
