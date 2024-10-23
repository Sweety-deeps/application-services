using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class GenericListModel
    {
        public int id { get; set; }
        [Required]
        public string tablename { get; set; }
        [Required]
        public string columnname { get; set; }
        [Required]
        public string lookupvalue { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
    }
}
