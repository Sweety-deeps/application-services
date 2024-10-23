using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class DataImportUploadModel
    {
        public string excelfile { get; set; }
        public string entityname { get; set; }
        public string paygroup { get; set; }
    }
}
