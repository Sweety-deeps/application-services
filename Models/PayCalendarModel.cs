using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PayCalendarModel
    {
        public int id { get; set; }
        public int paygroupid { get; set; }
        public int legalentityid { get; set; }
        public string legalentitycode { get; set; }
        public string paygroupcode { get; set; }
        public string months { get; set; }
        public int payperiod { get; set; }
        public DateTime? date { get; set; }
        public int year { get; set; }
        public string cutoffhours { get; set; }
        public string taskid { get; set; }
        public string tasknamelocal { get; set; }
        public string taskname { get; set; }
        public string frequency { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }


    }
}
