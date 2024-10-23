using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PayPeriods
    {
        public int period { get; set; }
        public int paygroupid { get; set; }
        public string periodtext { get; set; }
        public DateTime? date { get; set; }
    }
}
