﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HistoryEmployeePayDeduction
    {
        public int id { get; set; }
        public int paydeductiontableid { get; set; }
        public string? entitystate { get; set; }
        public string employeeid { get; set; }
        public int paygroupid { get; set; }
        public DateTime effectivedate { get; set; }
        public DateTime? enddate { get; set; }
        public string payelementtype { get; set; }
        public string? payelementcode { get; set; }
        public string? payelementname { get; set; }
        public decimal? amount { get; set; }
        public decimal? percentage { get; set; }
        public string? payperiodnumber { get; set; }
        public string? payperiodnumbersuffix { get; set; }
        public DateTime? paydate { get; set; }
        public string? overrides { get; set; }
        public string? payrollflag { get; set; }
        public string? modificationowner { get; set; }
        public DateTime? modificationdate { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public string? message { get; set; }
        public string? costcenter { get; set; }
        public string? recurrentschedule { get; set; }
        public DateTime? businessdate { get; set; }
        public bool changeprocessed { get; set; }
        public string? comments { get; set; }
        public string? recurrence { get; set;}
        public int? requestid { get; set; }
    }
}
