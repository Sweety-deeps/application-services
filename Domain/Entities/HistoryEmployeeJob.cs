using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HistoryEmployeeJob
    {
        public int id { get; set; }
        public int jobtableid { get; set; }
        public string? entitystate { get; set; }
        public string employeeid { get; set; }
        public int paygroupid { get; set; }
        public DateTime effectivedate { get; set; }
        public DateTime? enddate { get; set; }
        public string? personaljobtitle { get; set; }
        public string? employeestatus { get; set; }
        public string? jobchangereason { get; set; }
        public string? ispositionchangereason { get; set; }
        public string? iscompensationreason { get; set; }
        public string? isterminationreason { get; set; }
        public string? isleavereason { get; set; }
        public string? department { get; set; }
        public string? location { get; set; }
        public string? orgunit1 { get; set; }
        public string? orgunit2 { get; set; }
        public string? orgunit3 { get; set; }
        public string? orgunit4 { get; set; }
        public string? payclass { get; set; }
        public string? employeetype { get; set; }
        public string? employeepackage { get; set; }
        public string? costcenter { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public int? averagenumofdays { get; set; }
        public string? hiringtype { get; set; }
        public int? workingdaysperweek { get; set; }
        public string? job { get; set; }
        public string? primaryassignment { get; set; }
        public DateTime? terminationpaymentdate { get; set; }
        public decimal? dailyworkinghours { get; set; }
        public bool changeprocessed { get; set; }
        public string? comments { get; set; }
        public string? worklocationcity { get; set; }
        public string? worklocationstate { get; set; }
        public string? position { get; set; }
        public int? requestid { get; set; }

    }
}
