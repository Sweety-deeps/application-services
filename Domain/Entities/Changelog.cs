using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class ChangeLog
    {
        public long? id { get; set; }
        public string? paygroupcode { get; set; }
        public string? employeeid { get; set; }
        public string? tablename { get; set; }
        public string? fieldname { get; set; }
        public string? recordtype { get; set; }
        public string? oldvalue { get; set; }
        public DateTime? oldeffectivedate { get; set; }
        public string? newvalue { get; set; }
        public DateTime? neweffectivedate { get; set; }
        public DateTime? modifieddate { get; set; }
        public string? recordid { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
    }
}