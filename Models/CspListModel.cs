using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CspListModel
    {
        public int Id { get; set; }
        [Required]
        public string countrycode { get; set; }
        [Required]
        public string tablename { get; set; }
        [Required]
        public string columnname { get; set; }
        [Required]
        public string jsonvalue { get; set; }
        [Required]
        public string displayvalue { get; set; }
        [Required]
        public string outputvalue { get; set; }
    }
}
