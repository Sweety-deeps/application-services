using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ReportServiceDetails : BaseEntity
    {

        [Column("paygroupcode")]
        public string PayGroupCode { get; set; }
        [Column("s3objectid")]
        public string S3ObjectId { get; set; }
        [Column("filteroption")]
        public string? FilterOption { get; set; }
        [Column("year")]
        public int? Year { get; set; }
        [Column("payperiod")]
        public int? PayPeriod { get; set; }
        [Column("startdate")]
        public DateTime? StartDate { get; set; }
        [Column("enddate")]
        public DateTime? EndDate { get; set; }
        [Column("createdby")]
        public string CreatedBy { get; set; }
        [Column("reportfilter")]
        public string? ReportFilter { get; set; }
    }
}
