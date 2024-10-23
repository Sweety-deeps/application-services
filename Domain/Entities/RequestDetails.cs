using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RequestDetails
    {
        

        public int id { get; set; }
        public int payrollcompanyid { get; set; }
        public int requesttypeid { get; set; }
        public string paygroup { get; set; }
        public string? storagedetails { get; set; }
        public string processstatus { get; set; }
        public string createdby { get; set; }
        public DateTime? createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public string? s3objectid { get; set; }
        public string? interfacetype { get; set; }
        public string? additionalinfo { get; set; }
        public int? success { get; set; }
        public int? warning { get; set; }
        public int? failure { get; set; }
    }
}
