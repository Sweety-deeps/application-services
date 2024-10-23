using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PayCalendar
    {
        public int id { get; set; }
        public int paygroupid { get; set; }
        public int payperiod { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public DateTime? reminder {  get; set; }
        public DateTime? cuttoffdate { get; set; }
        public DateTime? payrollcalcdate { get; set; }
        public DateTime? clientapproval {  get; set; }
        public DateTime? commitdate { get; set; }
        public DateTime? paydate {  get; set; }
        //public string? notes { get; set; }
        public string taskid { get; set; }
        public string taskname { get; set; }
        public string tasknamelocal { get; set; }
        public DateTime? date { get; set; }
        public string cutoffhours { get; set; }
        public string months { get; set; }
        public string frequency { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public int year { get;set; }


    }
}
