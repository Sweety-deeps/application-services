using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class StartEndTimeRequestModel
    {
        public string? paygroup { get; set; }
        public int? year { get; set; }
        public int? payperiod { get; set; }
    }
}
