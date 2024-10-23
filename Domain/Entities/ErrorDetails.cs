using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ErrorDetails
    {
        public int id { get; set; }
        public int? requestid { get; set; }
        public string? project { get; set; }
        public string code { get; set; }
        public string shortdescription { get; set; }
        public string longdescription { get; set; }
    }
}
