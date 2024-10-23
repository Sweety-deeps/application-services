using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class DataImport
    {
        [Key]
        public Guid id { get; set; }
        public int? entityid { get; set; }
        public string entityname { get; set; }
        public string? template { get; set; }
        public string? s3objectid { get; set; }
        public string? paygroupcode { get; set; }
        public string? status { get; set; }
        public string? errordetails { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        [Column("additionalinfo", TypeName = "jsonb")]
        public string? AdditionalInfo { get; set; }
        public long? filesize { get; set; }
    }
}
