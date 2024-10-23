using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class JsonReviewModel
    {
        public int requestId { get; set; }
        public string requestType { get; set; }
        public string payGroup { get; set; }
        public string processStatus { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? modifiedAt { get; set; }
        public string? fileextension { get; set; }
        public string? s3objectid { get; set; }
        public string? processingtime { get; set; }
        public string? entityname { get; set; }
        public string? interfacetype { get; set; }
        public int? success { get; set; }
        public int? warning { get; set; }
        public int? failure { get; set; }
    }
}
