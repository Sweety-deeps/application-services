using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TimeAndAttendance
    {
        public int id { get; set; }
        public int? employeeid { get; set; }
        public string? employees { get; set; }
        public int? paygroupid { get; set; }
        public string? paygroup { get; set; }
        public DateTime? businessdate { get; set; }
        public decimal? nethours { get; set; }
        public decimal? rate { get; set; }
        public decimal? amountvalue { get; set; }
        public string? costcenter { get; set; }
        public string? position { get; set; }
        public string? departmentxrefcode { get; set; }
        public string? paycodexrefcode { get; set; }
        public string? location { get; set; }
        public string? project { get; set; }
        public string? custom0 { get; set; }
        public string? custom1 { get; set; }
        public string? custom2 { get; set; }
        public string? custom3 { get; set; }
        public string? custom4 { get; set; }
        public string? custom5 { get; set; }
        public string? custom6 { get; set; }
        public string? custom7 { get; set; }
        public string? custom8 { get; set; }
        public string? custom9 { get; set; }
        public string? isretro { get; set; }
        public string? createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        [Column("startdate")]
        public DateTime? StartDate { get; set; }
        [Column("enddate")]
        public DateTime? EndDate { get; set; }
        [Column("effectivedate")]
        public DateTime? EffectiveDate { get; set; }
        [Column("payelementname")]
        public string? PayElementName { get; set; }
    }
}
