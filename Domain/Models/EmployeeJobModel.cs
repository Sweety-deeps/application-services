using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class EmployeeJobModel
    {
        public int id { get; set; }
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
        public string? job { get; set; }
        public string? costcenter { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }

        public DateTime? terminationpaymentdate { get; set; }
        public string? primaryassignment { get; set; }
        public int? averagenumofdays { get; set; }
        public int? hiringtype { get; set; }
    }
}
