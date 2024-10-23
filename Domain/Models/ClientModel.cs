using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ClientModel
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int providerid { get; set; }
        public int? nooflegalentity {  get; set; }
        public int? noofpaygroup {  get; set; }
        public string? createdby { get; set; }
        public DateTime? createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public string providercode { get; set; }

    }
}
