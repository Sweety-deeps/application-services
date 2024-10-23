using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PayrollElements
    {
        public int id { get; set; }
        public int paygroupid { get; set; }
        public string? code { get; set; }
        public string name { get; set; }
        public string? type { get; set; }
        public string namelocal { get; set; }
        public string? exportcode { get; set; }
        public int? taxauthorityid { get; set; }
        public string pestatus { get; set; }
        public string itemtype { get; set; }
        public string format { get; set; }
        public string? glcreditcode { get; set; }
        public string? gldebitcode { get; set; }
        public string? clientreported { get; set; }
        public string? payslipprint { get; set; }
        public string? comments { get; set; }
        public string froms { get; set; }
        public string tos { get; set; }
        public string contributetonetpay { get; set; }
        public string isemployertax { get; set; }
        public string isemployerdeduction { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }

    }
}
