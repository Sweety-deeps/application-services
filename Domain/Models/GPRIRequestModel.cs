using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class GPRIRequestModel
    {
        public string clientname { get; set; }
        public string filterby { get; set; }
        public string paygroup { get; set; }
        public string country { get; set; }
        public string single { get; set; }
        public string multiple { get; set; }
        public string payperiod { get; set; }
        public string startpp { get; set; }
        public string ppoption { get; set; }
        public string types { get; set; }
        public string endpp { get; set; }
        
    }
}
