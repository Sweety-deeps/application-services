using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ErrorDetailsResponseModel
    {
        public int? RowNumber { get; set; }
        public string? ColumnName { get; set; }
        public string? Message { get; set; }
        public string? LogType { get; set; }
        public string? PayGroup { get; set; } 
        public string? Status { get; set; } 
        public DateTime? ErrorReportedOn { get; set; } 
        public string? FileId { get; set; }
    }
}
